using System.Text;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using SimpleBase;

namespace Bullish.Signer;

public class RequestSigner
{
    public record EosPrivateKey(ECPrivateKeyParameters PrivateKey);

    public record EosPublicKey(string EncodedPublicKey, ECPublicKeyParameters PublicKey);

    private static readonly ECDomainParameters CURVE_R1;
    private static readonly BigInteger HALF_CURVE_ORDER_R1;
    private static readonly ECDomainParameters ecParamsR1;
    // private static KeyFactory KEY_FACTORY;

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

    public RequestSigner()
    {
    }

    #region Public Key

    public static EosPublicKey DecodePublicKey(string eosAddress)
    {
        var payload = Base58.Bitcoin.Decode(StripKeyPrefix(eosAddress));

        var hex = Hex.ToHexString(payload);
        hex = hex.Substring(0, hex.Length - CHECKSUM_LENGTH);

        return DecodePublicKey(Hex.Decode(hex));
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

    public static EosPrivateKey DecodePrivateKey(string encodedPrivateKey)
    {
        var payload = Base58.Bitcoin.Decode(StripKeyPrefix(encodedPrivateKey));

        var hex = Hex.ToHexString(payload)[..^CHECKSUM_LENGTH];

        var privateKeyParameters = new ECPrivateKeyParameters(new BigInteger(hex, 16), GetEcDomainParameters());

        return new EosPrivateKey(privateKeyParameters);
    }

    #endregion

    #region Utilities

    private static X9ECParameters GetEcParameterSpec() => ECNamedCurveTable.GetByName("secp256r1");

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

    private static byte[] Combine(byte[] first, byte[] second)
    {
        var ret = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        return ret;
    }

    #endregion
}