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

[assembly: InternalsVisibleTo("Bullish.Signer.Tests")]

namespace Bullish.Signer;

internal static class RequestSignerImpl
{
    private const int ChecksumBytes = 4;
    private const int ChecksumLength = 8;
    private const int FirstTwoBytesOfKey = 4;
    private const int ExpectedROrSLength = 32;
    private const int ValueToAddToSignatureHeader = 31;
    private const int DataSequenceLengthBytePosition = 2;
    private const byte UncompressedPublicKeyByteIndicator = 0x04;
    private const byte CompressedPublicKeyByteIndicatorPositiveY = 0x02;
    private const byte CompressedPublicKeyByteIndicatorNegativeY = 0x03;
    private const string PatternStringPemPrefixPublicKeySecp256R1Compressed = "3039301306072a8648ce3d020106082a8648ce3d030107032200";

    private static readonly byte[] KeyTypeBytes;
    private static readonly ECDomainParameters CurveR1;
    private static readonly BigInteger HalfCurveOrderR1;
    private static readonly X9ECParameters X9EcParameters;
    private static readonly ECDomainParameters EcDomainParameters;

    static RequestSignerImpl()
    {
        KeyTypeBytes = "R1"u8.ToArray();
        
        X9EcParameters = ECNamedCurveTable.GetByName("secp256r1");
        EcDomainParameters = new ECDomainParameters(X9EcParameters);

        FixedPointUtilities.Precompute(X9EcParameters.G);

        CurveR1 = new ECDomainParameters(X9EcParameters.Curve, X9EcParameters.G, X9EcParameters.N, X9EcParameters.H);

        HalfCurveOrderR1 = X9EcParameters.N.ShiftRight(1);
    }

    #region Verification

    internal static bool VerifySignature(byte[] request, string sigBase58, EosPublicKey eosPublicKey)
    {
        var sig = sigBase58[(sigBase58.LastIndexOf("_", StringComparison.Ordinal) + 1)..];
        var encodedSig = Base58.Bitcoin.Decode(sig);

        var r = new BigInteger(1, encodedSig, 1, 32);
        CheckArgument(r.SignValue >= 1, "R must be positive");

        var s = new BigInteger(1, encodedSig, 33, 32);
        CheckArgument(s.SignValue >= 1, "S must be positive");

        var decoderData = KeyTypeBytes;
        var newChecksumData = new byte[65 + decoderData.Length];
        Buffer.BlockCopy(encodedSig, 0, newChecksumData, 0, 65);
        Buffer.BlockCopy(decoderData, 0, newChecksumData, 65, decoderData.Length);
        var newChecksum = CalculateChecksum(newChecksumData);

        // Verify the checksum, which is 4 bytes
        if (!(newChecksum[0] == encodedSig[65] ||
              newChecksum[1] == encodedSig[66] ||
              newChecksum[2] == encodedSig[67] ||
              newChecksum[3] == encodedSig[68]))
            return false;

        var signer = new ECDsaSigner();
        var parameters = X9EcParameters;

        CheckArgument(!parameters.N.Equals(r), "R cannot equal N");
        CheckArgument(!parameters.N.Equals(s), "S cannot equal N");

        var point = eosPublicKey.PublicKey.Q;
        var ecDomainParameters = new ECDomainParameters(parameters.Curve, parameters.G, parameters.N, parameters.H);
        var cipherParameters = new ECPublicKeyParameters(point, ecDomainParameters);

        signer.Init(false, cipherParameters);

        return signer.VerifySignature(request, r, s);
    }

    private static byte[] CalculateChecksum(byte[] publicKey)
    {
        RipeMD160Digest ripemd160Digest = new RipeMD160Digest();
        ripemd160Digest.BlockUpdate(publicKey, 0, publicKey.Length);
        byte[] actualChecksum = new byte[ripemd160Digest.GetDigestSize()];
        ripemd160Digest.DoFinal(actualChecksum, 0);
        return actualChecksum;
    }

    #endregion

    #region Signing

    internal static string SignRequest(byte[] request, EosPrivateKey eosPrivateKey, EosPublicKey eosPublicKey)
    {
        var cipherParameters = eosPrivateKey.PrivateKey;

        GenerateCanonicalSignature(request, cipherParameters, out var rAndS);

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
            throw new Exception();

        //Add RecoveryID + 27 + 4 to create the header byte
        recoveryId += ValueToAddToSignatureHeader;
        byte headerByte = (byte)recoveryId; //((int)recoverId).byteValue();

        var rBytes = r.ToByteArrayUnsigned();
        if (rBytes.Length != ExpectedROrSLength)
            throw new Exception("R is not expected length");

        var sBytes = s.ToByteArrayUnsigned();
        if (sBytes.Length != ExpectedROrSLength)
            throw new Exception("S is not expected length");

        var rAndS = Combine(rBytes, sBytes);
        var decodedSignature = Combine(new[] { headerByte }, rAndS);

        if (!IsCanonical(decodedSignature))
            throw new Exception();

        //Add checksum to signature
        var signatureWithCheckSum = AddCheckSumToSignature(decodedSignature, KeyTypeBytes);

        //Base58 encode signature and add pertinent EOS prefix
        var eosFormattedSignature = $"SIG_R1_{Base58.Bitcoin.Encode(signatureWithCheckSum)}";

        return eosFormattedSignature;
    }

    private static byte[] AddCheckSumToSignature(byte[] signature, byte[] keyTypeByteArray)
    {
        var signatureWithKeyType = Combine(signature, keyTypeByteArray);
        var signatureRipemd160 = DigestRipeMd160(signatureWithKeyType);
        var checkSum = Arrays.CopyOfRange(signatureRipemd160, 0, ChecksumBytes);

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
        var ecParamsR1 = X9EcParameters;

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

        //   1.2. Convert the integer x to an octet string X of length mLen using the conversion routine
        //        specified in Section 2.3.7, where mLen = ceiling((log2 p)/8) or mLen equals ceiling(m/8).
        //   1.3. Convert the octet string (16 set binary digits)||X to an elliptic curve point R using the
        //        conversion routine specified in Section 2.3.4. If this conversion routine outputs 'invalid', then
        //        do another iteration of Step 1.
        //
        // More concisely, what these points mean is to use X as a compressed public key.
        BigInteger prime = curve.Q;

        // Cannot have point co-ordinates larger than this as everything takes place modulo Q.
        if (x.CompareTo(prime) >= 0)
            return Array.Empty<byte>();

        // Compressed keys require you to know an extra bit of data about the y-coordinates as there are two possibilities.
        // So it's encoded in the recId.
        var ecPointR = DecompressKey(x, (recId & 1) == 1);

        //   1.4. If nR != point at infinity, then do another iteration of Step 1 (callers responsibility).
        if (!ecPointR.Multiply(n).IsInfinity)
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
        var q = ECAlgorithms.SumOfTwoMultiplies(g, eInvRInv, ecPointR, srInv);

        return q.GetEncoded(true);
    }

    private static ECPoint DecompressKey(BigInteger xBn, bool yBit)
    {
        var ecParamsR1 = X9EcParameters;

        var compEnc = X9IntegerConverter.IntegerToBytes(xBn, 1 + X9IntegerConverter.GetByteLength(ecParamsR1.Curve));

        compEnc[0] = yBit ? CompressedPublicKeyByteIndicatorNegativeY : CompressedPublicKeyByteIndicatorPositiveY;

        return ecParamsR1.Curve.DecodePoint(compEnc);
    }

    private static void CheckArgument(bool condition, string message)
    {
        if (!condition)
            throw new ArgumentException(message);
    }

    private static BigInteger CheckAndHandleLowS(BigInteger s)
    {
        return !IsLowS(s) ? CurveR1.N.Subtract(s) : s;
    }

    private static bool IsLowS(BigInteger s)
    {
        var compareResult = s.CompareTo(HalfCurveOrderR1);
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
        pemFormattedPublicKey = pemFormattedPublicKey.Replace("PUB_R1_", "");

        //Base58 decode the key
        var base58DecodedPublicKey = DecodePublicKey(pemFormattedPublicKey);

        //Convert decoded array to string
        pemFormattedPublicKey = Hex.ToHexString(base58DecodedPublicKey);

        /*
        Compress the public key if necessary.
        Compression is only necessary if the key has a value of 0x04 for the first byte, which
        indicates it is uncompressed.
         */
        if (base58DecodedPublicKey[0] == UncompressedPublicKeyByteIndicator)
            pemFormattedPublicKey = Hex.ToHexString(CompressPublicKey(Hex.Decode(pemFormattedPublicKey)));

        pemFormattedPublicKey = PatternStringPemPrefixPublicKeySecp256R1Compressed + pemFormattedPublicKey;

        /*
        Correct the sequence length value.  According to the ASN.1 specification.  For a DER encoded public key the second byte reflects the number of bytes following
        the second byte.  Here we take the length of the entire string, subtract 4 to remove the first two bytes, divide by 2 (i.e. two characters per byte) and replace
        the second byte in the string with the corrected length.
         */
        var i = (pemFormattedPublicKey.Length - FirstTwoBytesOfKey) / 2;

        var correctedLength = i.ToString("X2"); // Integer.toHexString(i);

        pemFormattedPublicKey = pemFormattedPublicKey.Substring(0, DataSequenceLengthBytePosition)
                                + correctedLength
                                + pemFormattedPublicKey.Substring(FirstTwoBytesOfKey);


        pemFormattedPublicKey = DerToPem(Hex.Decode(pemFormattedPublicKey));

        return pemFormattedPublicKey;
    }

    private static byte[] CompressPublicKey(byte[] compressedPublicKey)
    {
        var parameterSpec = X9EcParameters;
        var ecPoint = parameterSpec.Curve.DecodePoint(compressedPublicKey);

        var x = ecPoint.XCoord.GetEncoded();
        var y = ecPoint.YCoord.GetEncoded();

        //Check whether y is negative(odd in field) or positive(even in field) and assign compressionPrefix
        var bigIntegerY = new BigInteger(Hex.ToHexString(y), 16);
        var bigIntegerTwo = BigInteger.ValueOf(2);
        var remainder = bigIntegerY.Mod(bigIntegerTwo);

        var compressionPrefix = remainder.Equals(BigInteger.Zero) ? CompressedPublicKeyByteIndicatorPositiveY : CompressedPublicKeyByteIndicatorNegativeY;

        return Combine(new[] { compressionPrefix }, x);
    }

    private static string DerToPem(byte[] derEncodedByteArray)
    {
        var pemForm = new StringBuilder();

        pemForm.Append("-----BEGIN PUBLIC KEY-----");
        pemForm.Append('\n');

        //Base64 Encode DER Encoded Byte Array And Add to PEM Object
        var base64EncodedByteArray = Base64.ToBase64String(derEncodedByteArray);
        pemForm.Append(base64EncodedByteArray);

        pemForm.Append('\n');
        //Build Footer
        pemForm.Append("-----END PUBLIC KEY-----");

        return pemForm.ToString();
    }

    #endregion

    #region Public Key

    internal static EosPublicKey GetEosPublicKey(string eosAddress)
    {
        var payload = Base58.Bitcoin.Decode(StripKeyPrefix(eosAddress));

        var hex = Hex.ToHexString(payload);
        hex = hex[..^ChecksumLength];

        return DecodePublicKey(Hex.Decode(hex));
    }

    private static byte[] DecodePublicKey(string strKey)
    {
        if (string.IsNullOrWhiteSpace(strKey))
            throw new Exception("Input key to decode can't be empty.");

        var base58Decoded = Base58.Bitcoin.Decode(strKey);
        var firstCheckSum = Arrays.CopyOfRange(base58Decoded, base58Decoded.Length - ChecksumBytes, base58Decoded.Length);
        var decodedKey = Arrays.CopyOfRange(base58Decoded, 0, base58Decoded.Length - ChecksumBytes);

        if (InvalidRipeMd160CheckSum(decodedKey, firstCheckSum, KeyTypeBytes))
            throw new Exception();

        return decodedKey;
    }

    private static EosPublicKey DecodePublicKey(byte[] pointData)
    {
        if (pointData.Length != 33)
            throw new Exception("PointData Invalid");

        var dp = EcDomainParameters;

        var publicKeyParameters = new ECPublicKeyParameters(dp.Curve.DecodePoint(pointData), dp);

        var encodedPublicKey = EncodePublicKey(pointData);

        return new EosPublicKey(encodedPublicKey, publicKeyParameters);
    }

    private static string EncodePublicKey(byte[] data)
    {
        if (data.Length == 0)
            throw new Exception();

        var checkSum = ExtractCheckSumRipemd160(data, KeyTypeBytes);
        var base58Key = Base58.Bitcoin.Encode(Combine(data, checkSum));

        return $"PUB_R1_{base58Key}";
    }

    #endregion

    #region Private Key

    internal static EosPrivateKey GetEosPrivateKey(string encodedPrivateKey)
    {
        var payload = Base58.Bitcoin.Decode(StripKeyPrefix(encodedPrivateKey));

        var hex = Hex.ToHexString(payload)[..^ChecksumLength];

        var privateKeyParameters = new ECPrivateKeyParameters(new BigInteger(hex, 16), EcDomainParameters);

        return new EosPrivateKey(privateKeyParameters);
    }

    #endregion

    #region Utilities

    private static string StripKeyPrefix(string key) => key[7..];

    private static byte[] ExtractCheckSumRipemd160(byte[] pemKey, byte[] keyTypeByteArray)
    {
        var digest = DigestRipeMd160(Combine(pemKey, keyTypeByteArray));
        return Arrays.CopyOfRange(digest, 0, ChecksumBytes);
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
            throw new Exception();

        var keyWithType = Combine(inputKey, keyTypeByteArray);
        var digestRipemd160 = DigestRipeMd160(keyWithType);
        var checkSumFromInputKey = Arrays.CopyOfRange(digestRipemd160, 0, ChecksumBytes);

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