using System.Runtime.CompilerServices;
using System.Text;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using SimpleBase;

[assembly:InternalsVisibleTo("Bullish.Signer.Tests")]

namespace Bullish.Signer;

internal class RequestSignerImpl
{
    internal record EosPrivateKey(ECPrivateKeyParameters PrivateKey);

    internal record EosPublicKey(string EncodedPublicKey, ECPublicKeyParameters PublicKey);

    private static readonly ECDomainParameters CURVE_R1;
    private static readonly BigInteger HALF_CURVE_ORDER_R1;

    private const byte UNCOMPRESSED_PUBLIC_KEY_BYTE_INDICATOR = 0x04;
    private const byte COMPRESSED_PUBLIC_KEY_BYTE_INDICATOR_POSITIVE_Y = 0x02;
    private const byte COMPRESSED_PUBLIC_KEY_BYTE_INDICATOR_NEGATIVE_Y = 0x03;
    private const int CHECKSUM_LENGTH = 8;
    private const int CHECKSUM_BYTES = 4;
    private const int VALUE_TO_ADD_TO_SIGNATURE_HEADER = 31;
    private const int EXPECTED_R_OR_S_LENGTH = 32;
    private const int FIRST_TWO_BYTES_OF_KEY = 4;
    private const int DATA_SEQUENCE_LENGTH_BYTE_POSITION = 2;

    private const string SECP256R1 = "secp256r1";
    private static X9ECParameters CURVE_PARAMS_R1 => GetEcParameterSpec(); // TODO: Remove this
    private const string ECDSA = "ECDSA";
    private const string R1_KEY_TYPE = "R1";
    private const string PATTERN_STRING_EOS_PREFIX_SIG_R1 = "SIG_R1_";
    private const string PATTERN_STRING_EOS_PREFIX_PUB_R1 = "PUB_R1_";
    private const string PATTERN_STRING_PEM_PREFIX_PUBLIC_KEY_SECP256R1_COMPRESSED = "3039301306072a8648ce3d020106082a8648ce3d030107032200";
    private const string PEM_HEADER_PUBLIC_KEY = "-----BEGIN PUBLIC KEY-----";
    private const string PEM_FOOTER_PUBLIC_KEY = "-----END PUBLIC KEY-----";
    private const string GENERIC_ERROR = "error";

    static RequestSignerImpl()
    {
        FixedPointUtilities.Precompute(CURVE_PARAMS_R1.G);
        CURVE_R1 = new ECDomainParameters(
            CURVE_PARAMS_R1.Curve,
            CURVE_PARAMS_R1.G,
            CURVE_PARAMS_R1.N,
            CURVE_PARAMS_R1.H);
        HALF_CURVE_ORDER_R1 = CURVE_PARAMS_R1.N.ShiftRight(1);
    }
    
    #region Signing

    internal static string SignRequest(byte[] request, EosPrivateKey eosPrivateKey, EosPublicKey eosPublicKey)
    {
        var cipherParameters = eosPrivateKey.PrivateKey;

        GenerateCanonicalSignature(request,cipherParameters, out var rAndS);

        var publicKeyPem = ConvertEosPublicKeyToPemFormat(eosPublicKey.EncodedPublicKey);

        var sig = ConvertRawRandSofSignatureToEosFormat(rAndS[0], rAndS[1], request, publicKeyPem);

        return sig;
    }

    private static void GenerateCanonicalSignature(byte[] payload, ECPrivateKeyParameters cipherParameters, out BigInteger[] rAndS)
    {
        rAndS = GenerateSignature(payload, cipherParameters);
        if (!IsCanonical(rAndS[0], rAndS[1]))
            GenerateCanonicalSignature(payload, cipherParameters, out rAndS);
    }

    internal static string ConvertRawRandSofSignatureToEosFormat(BigInteger r, BigInteger s, byte[] signableTransaction, string publicKeyPem)
    {
        var publicKey = new PemProcessor(publicKeyPem);
        var keyData = publicKey.GetKeyData();

        s = CheckAndHandleLowS(s);

        /*
        Get recovery ID.  This is the index of the public key (0-3) that represents the
        expected public key used to sign the transaction.
         */
        var recoveryId = GetRecoveryId(r, s, signableTransaction, keyData);

        if (recoveryId < 0)
            throw new Exception(GENERIC_ERROR);

        //Add RecoveryID + 27 + 4 to create the header byte
        recoveryId += VALUE_TO_ADD_TO_SIGNATURE_HEADER;
        byte headerByte = (byte)recoveryId; //((int)recoverId).byteValue();

        var rBytes = r.ToByteArrayUnsigned();
        if (rBytes.Length != EXPECTED_R_OR_S_LENGTH)
            throw new Exception("R is not expected length");

        var sBytes = s.ToByteArrayUnsigned();
        if (sBytes.Length != EXPECTED_R_OR_S_LENGTH)
            throw new Exception("S is not expected length");

        var rAndS = Combine(rBytes, sBytes);
        var decodedSignature = Combine(new[] { headerByte }, rAndS);

        if (!IsCanonical(decodedSignature))
            throw new Exception(GENERIC_ERROR);

        //Add checksum to signature
        var signatureWithCheckSum = AddCheckSumToSignature(decodedSignature, Encoding.UTF8.GetBytes(R1_KEY_TYPE));

        //Base58 encode signature and add pertinent EOS prefix
        var eosFormattedSignature = $"{PATTERN_STRING_EOS_PREFIX_SIG_R1}{Base58.Bitcoin.Encode(signatureWithCheckSum)}";

        return eosFormattedSignature;
    }
    
    private static byte[] AddCheckSumToSignature(byte[] signature, byte[] keyTypeByteArray) 
    {
        var signatureWithKeyType = Combine(signature, keyTypeByteArray);
        var signatureRipemd160 = DigestRipeMd160(signatureWithKeyType);
        var checkSum = Arrays.CopyOfRange(signatureRipemd160, 0, CHECKSUM_BYTES);
        
        return Combine(signature, checkSum);
    }

    private static int GetRecoveryId(BigInteger r, BigInteger s, byte[] sha256HashMessage, byte[] publicKey)
    {
        for (var i = 0; i < 4; i++)
        {
            var recoveredPublicKey = RecoverPublicKeyFromSignature(i, r, s, sha256HashMessage);

            if (Arrays.AreEqual(publicKey, recoveredPublicKey))
                return i;
        }

        return -1;
    }

    private static byte[] RecoverPublicKeyFromSignature(int recId, BigInteger r, BigInteger s, byte[] message)
    {
        var ecParamsR1 = GetEcParameterSpec();
        
        CheckArgument(r.SignValue >= 1, "r must be positive");
        CheckArgument(s.SignValue >= 1, "s must be positive");
        CheckArgument(!ecParamsR1.N.Equals(r), "r cannot equal n");
        CheckArgument(!ecParamsR1.N.Equals(s), "s cannot equal n");

        // 1.0 For j from 0 to h   (h == recId here and the loop is outside this function)
        // 1.1 Let x = r + jn
        
        var n = ecParamsR1.N; // Curve order.
        var g = ecParamsR1.G;

        var curve = (FpCurve)ecParamsR1.Curve;

        var i = BigInteger.ValueOf((long)recId / 2);
        var x = r.Add(i.Multiply(n));

        //   1.2. Convert the integer x to an octet string X of length mlen using the conversion routine
        //        specified in Section 2.3.7, where mlen = ceiling((log2 p)/8) or mlen equals ceiling(m/8).
        //   1.3. Convert the octet string (16 set binary digits)||X to an elliptic curve point R using the
        //        conversion routine specified in Section 2.3.4. If this conversion routine outputs 'invalid', then
        //        do another iteration of Step 1.
        //
        // More concisely, what these points mean is to use X as a compressed public key.
        BigInteger prime = curve.Q;
        
        // Cannot have point co-ordinates larger than this as everything takes place modulo Q.
        if (x.CompareTo(prime) >= 0)
            return Array.Empty<byte>();

        // Compressed keys require you to know an extra bit of data about the y-coord as there are two possibilities.
        // So it's encoded in the recId.
        ECPoint R = DecompressKey(x, (recId & 1) == 1);
        
        //   1.4. If nR != point at infinity, then do another iteration of Step 1 (callers responsibility).
        if (!R.Multiply(n).IsInfinity)
            return Array.Empty<byte>();

        //   1.5. Compute e from M using Steps 2 and 3 of ECDSA signature verification.
        var e = new BigInteger(1, message);
        
        //   1.6. For k from 1 to 2 do the following.   (loop is outside this function via iterating recId)
        //   1.6.1. Compute a candidate public key as:
        //               Q = mi(r) * (sR - eG)
        //
        // Where mi(x) is the modular multiplicative inverse. We transform this into the following:
        //               Q = (mi(r) * s ** R) + (mi(r) * -e ** G)
        // Where -e is the modular additive inverse of e, that is z such that z + e = 0 (mod n). In the above equation
        // ** is point multiplication and + is point addition (the EC group operator).
        //
        // We can find the additive inverse by subtracting e from zero then taking the mod. For example the additive
        // inverse of 3 modulo 11 is 8 because 3 + 8 mod 11 = 0, and -3 mod 11 = 8.
        var eInv = BigInteger.Zero.Subtract(e).Mod(n);
        var rInv = r.ModInverse(n);
        var srInv = rInv.Multiply(s).Mod(n);
        var eInvRInv = rInv.Multiply(eInv).Mod(n);
        var q = ECAlgorithms.SumOfTwoMultiplies(g, eInvRInv, R, srInv);
        
        return q.GetEncoded(true);
    }
    
    private static ECPoint DecompressKey(BigInteger xBn, bool yBit) 
    {
        var ecParamsR1 = GetEcParameterSpec();

        var compEnc = X9IntegerConverter.IntegerToBytes(xBn, 1 + X9IntegerConverter.GetByteLength(ecParamsR1.Curve));
        
        compEnc[0] = yBit ? COMPRESSED_PUBLIC_KEY_BYTE_INDICATOR_NEGATIVE_Y : COMPRESSED_PUBLIC_KEY_BYTE_INDICATOR_POSITIVE_Y;
        
        return ecParamsR1.Curve.DecodePoint(compEnc);
    }

    private static void CheckArgument(bool condition, string message)
    {
        if (!condition)
            throw new ArgumentException(message);
    }

    private static BigInteger CheckAndHandleLowS(BigInteger s)
    {
        return !IsLowS(s) ? CURVE_R1.N.Subtract(s) : s;
    }

    private static bool IsLowS(BigInteger s)
    {
        var compareResult = s.CompareTo(HALF_CURVE_ORDER_R1);
        return compareResult == 0 || compareResult < 0;
    }

    internal static bool IsCanonical(BigInteger r, BigInteger s)
    {
        var sigData = new byte[69];
        sigData[0] = 31;
        Array.Copy(r.ToByteArrayUnsigned(), 0, sigData, 1, 32);
        Array.Copy(s.ToByteArrayUnsigned(), 0, sigData, 33, 32);
        return IsCanonical(sigData);
    }

    private static bool IsCanonical(byte[] signature) =>
        (signature[1] & unchecked((sbyte)128)) == 0 &&
        (signature[1] != 0 || (signature[2] & unchecked((sbyte)128)) != 0) &&
        (signature[33] & unchecked((sbyte)128)) == 0 &&
        (signature[33] != 0 || (signature[34] & unchecked((sbyte)128)) != 0);

    private static BigInteger[] GenerateSignature(byte[] payload, ECPrivateKeyParameters cipherParameters)
    {
        var signer = new ECDsaSigner();
        signer.Init(true, cipherParameters);
        return signer.GenerateSignature(payload);
    }

    internal static string ConvertEosPublicKeyToPemFormat(string publicKeyEos)
    {
        var pemFormattedPublicKey = publicKeyEos;
        pemFormattedPublicKey = pemFormattedPublicKey.Replace(PATTERN_STRING_EOS_PREFIX_PUB_R1, "");

        //Base58 decode the key
        var base58DecodedPublicKey = DecodePublicKey(pemFormattedPublicKey);

        //Convert decoded array to string
        pemFormattedPublicKey = Hex.ToHexString(base58DecodedPublicKey);

        /*
        Compress the public key if necessary.
        Compression is only necessary if the key has a value of 0x04 for the first byte, which
        indicates it is uncompressed.
         */
        if (base58DecodedPublicKey[0] == UNCOMPRESSED_PUBLIC_KEY_BYTE_INDICATOR)
            pemFormattedPublicKey = Hex.ToHexString(CompressPublicKey(Hex.Decode(pemFormattedPublicKey)));

        pemFormattedPublicKey = PATTERN_STRING_PEM_PREFIX_PUBLIC_KEY_SECP256R1_COMPRESSED + pemFormattedPublicKey;

        /*
        Correct the sequence length value.  According to the ASN.1 specification.  For a DER encoded public key the second byte reflects the number of bytes following
        the second byte.  Here we take the length of the entire string, subtract 4 to remove the first two bytes, divide by 2 (i.e. two characters per byte) and replace
        the second byte in the string with the corrected length.
         */
        var i = (pemFormattedPublicKey.Length - FIRST_TWO_BYTES_OF_KEY) / 2;

        var correctedLength = i.ToString("X2"); // Integer.toHexString(i);

        pemFormattedPublicKey = pemFormattedPublicKey.Substring(0, DATA_SEQUENCE_LENGTH_BYTE_POSITION)
                                + correctedLength
                                + pemFormattedPublicKey.Substring(FIRST_TWO_BYTES_OF_KEY);


        pemFormattedPublicKey = DerToPem(Hex.Decode(pemFormattedPublicKey));

        return pemFormattedPublicKey;
    }

    private static byte[] CompressPublicKey(byte[] compressedPublicKey)
    {
        var parameterSpec = GetEcParameterSpec();
        var ecPoint = parameterSpec.Curve.DecodePoint(compressedPublicKey);

        var x = ecPoint.XCoord.GetEncoded();
        var y = ecPoint.YCoord.GetEncoded();

        //Check whether y is negative(odd in field) or positive(even in field) and assign compressionPrefix
        var bigIntegerY = new BigInteger(Hex.ToHexString(y), 16);
        var bigIntegerTwo = BigInteger.ValueOf(2);
        var remainder = bigIntegerY.Mod(bigIntegerTwo);

        var compressionPrefix = remainder.Equals(BigInteger.Zero) ? COMPRESSED_PUBLIC_KEY_BYTE_INDICATOR_POSITIVE_Y : COMPRESSED_PUBLIC_KEY_BYTE_INDICATOR_NEGATIVE_Y;

        return Combine(new[] { compressionPrefix }, x);
    }

    private static string DerToPem(byte[] derEncodedByteArray)
    {
        var pemForm = new StringBuilder();

        pemForm.Append(PEM_HEADER_PUBLIC_KEY);
        pemForm.Append('\n');

        //Base64 Encode DER Encoded Byte Array And Add to PEM Object
        var base64EncodedByteArray = Base64.ToBase64String(derEncodedByteArray);
        pemForm.Append(base64EncodedByteArray);

        pemForm.Append('\n');
        //Build Footer
        pemForm.Append(PEM_FOOTER_PUBLIC_KEY);

        return pemForm.ToString();
    }

    #endregion

    #region Public Key

    internal static EosPublicKey GetEosPublicKey(string eosAddress)
    {
        var payload = Base58.Bitcoin.Decode(StripKeyPrefix(eosAddress));

        var hex = Hex.ToHexString(payload);
        hex = hex[..^CHECKSUM_LENGTH];

        return DecodePublicKey(Hex.Decode(hex));
    }
    
    private static byte[] DecodePublicKey(string strKey)
    {
        if (string.IsNullOrWhiteSpace(strKey))
            throw new Exception("Input key to decode can't be empty.");

        var base58Decoded = Base58.Bitcoin.Decode(strKey);
        var firstCheckSum = Arrays.CopyOfRange(base58Decoded, base58Decoded.Length - CHECKSUM_BYTES, base58Decoded.Length);
        var decodedKey = Arrays.CopyOfRange(base58Decoded, 0, base58Decoded.Length - CHECKSUM_BYTES);

        if (InvalidRipeMd160CheckSum(decodedKey, firstCheckSum, Encoding.UTF8.GetBytes(R1_KEY_TYPE)))
            throw new Exception(GENERIC_ERROR);

        return decodedKey;
    }

    private static EosPublicKey DecodePublicKey(byte[] pointData)
    {
        if (pointData.Length != 33)
            throw new Exception("PointData Invalid");

        var dp = GetEcDomainParameters();

        var publicKeyParameters = new ECPublicKeyParameters(dp.Curve.DecodePoint(pointData), dp);

        var encodedPublicKey = EncodePublicKey(pointData);

        return new EosPublicKey(encodedPublicKey, publicKeyParameters);
    }

    private static string EncodePublicKey(byte[] data)
    {
        if (data.Length == 0)
            throw new Exception(GENERIC_ERROR);

        var checkSum = ExtractCheckSumRipemd160(data, Encoding.UTF8.GetBytes(R1_KEY_TYPE));
        var base58Key = Base58.Bitcoin.Encode(Combine(data, checkSum));

        return PATTERN_STRING_EOS_PREFIX_PUB_R1 + base58Key;
    }

    #endregion

    #region Private Key

    internal static EosPrivateKey GetEosPrivateKey(string encodedPrivateKey)
    {
        var payload = Base58.Bitcoin.Decode(StripKeyPrefix(encodedPrivateKey));

        var hex = Hex.ToHexString(payload)[..^CHECKSUM_LENGTH];

        var privateKeyParameters = new ECPrivateKeyParameters(new BigInteger(hex, 16), GetEcDomainParameters());

        return new EosPrivateKey(privateKeyParameters);
    }

    #endregion

    #region Utilities

    private static X9ECParameters GetEcParameterSpec() => ECNamedCurveTable.GetByName(SECP256R1);

    private static ECDomainParameters GetEcDomainParameters() => new(GetEcParameterSpec());

    private static string StripKeyPrefix(string key) => key[7..];

    private static byte[] ExtractCheckSumRipemd160(byte[] pemKey, byte[] keyTypeByteArray)
    {
        var digest = DigestRipeMd160(Combine(pemKey, keyTypeByteArray));
        return Arrays.CopyOfRange(digest, 0, CHECKSUM_BYTES);
    }

    private static byte[] DigestRipeMd160(byte[] bytes)
    {
        var digest = new RipeMD160Digest();
        digest.BlockUpdate(bytes, 0, bytes.Length);
        var outArray = new byte[20];
        digest.DoFinal(outArray, 0);

        return outArray;
    }

    private static bool InvalidRipeMd160CheckSum(byte[] inputKey, byte[] checkSumToValidate, byte[] keyTypeByteArray)
    {
        if (inputKey.Length == 0 || checkSumToValidate.Length == 0)
            throw new Exception(GENERIC_ERROR);

        var keyWithType = Combine(inputKey, keyTypeByteArray);
        var digestRipemd160 = DigestRipeMd160(keyWithType);
        var checkSumFromInputKey = Arrays.CopyOfRange(digestRipemd160, 0, CHECKSUM_BYTES);

        //This checksum returns whether the checksum comparison was invalid.
        return !Arrays.AreEqual(checkSumToValidate, checkSumFromInputKey);
    }

    private static byte[] Combine(byte[] first, byte[] second)
    {
        var ret = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        return ret;
    }

    #endregion
}