using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO.Pem;

// Use the OpenSSL PemReader to for parsing the PEM Object Type (IO.Pem.PemReader doesn't support parsing)
using PemReader = Org.BouncyCastle.OpenSsl.PemReader; 

namespace Bullish.Signer;

/// <summary>
/// This is a wrapper class for PemObjects that throws an Exception if an invalid
/// PEMObject is passed into the constructor. Once initialized the PemProcessor can be used to
/// return the type, DER format, or algorithm used to create the PemObject.
/// </summary>
public class PemProcessor
{
    private const int PrivateKeyStartIndex = 2;
    private const string DerToPemConversion = "Error converting DER encoded key to PEM format!";
    private const string ErrorReadingPemObject = "Error reading PEM object!";
    private const string ErrorParsingPemObject = "Error parsing PEM object!";
    private const string KeyDataNotFound = "Key data not found in PEM object!";
    private const string InvalidPemObject = "Cannot read PEM object!";

    private readonly PemObject _pemObject;
    private readonly string _pemObjectString;

    /// <summary>
    /// Initialize PEMProcessor with PEM content in String format.
    /// </summary>
    /// <param name="pemObject">Input PEM content in String format.</param>
    /// <exception cref="Exception">Exception when failing to read pem data from the input.</exception>
    public PemProcessor(string pemObject)
    {
        _pemObjectString = pemObject;
        
        try
        {
            using var reader = new StringReader(_pemObjectString);

            using var pemReader = new PemReader(reader);

            _pemObject = pemReader.ReadPemObject();

            if (_pemObject == null)
                throw new Exception(InvalidPemObject);
        }
        catch (Exception ex)
        {
            throw new Exception(ErrorParsingPemObject, ex);
        }
    }

    /// <summary>
    /// Gets the PEM Object key type (i.e. PRIVATE KEY, PUBLIC KEY).
    /// </summary>
    public string Type => _pemObject.Type;

    /// <summary>
    /// Gets the DER encoded format of the key from its PEM format.
    /// </summary>
    public string DerFormat => Hex.ToHexString(_pemObject.Content);

    public byte[] GetKeyData()
    {
        var pemObjectParsed = ParsePemObject();

        if (pemObjectParsed is SubjectPublicKeyInfo publicKeyInfo)
        {
            return publicKeyInfo.PublicKeyData.GetOctets();
        }

        if (pemObjectParsed is ECPrivateKeyParameters)
        {
            try
            {
                var derFormatBytes = Hex.Decode(DerFormat);

                using var asn1InputStream = new Asn1InputStream(derFormatBytes);

                var sequence = (Asn1Sequence)asn1InputStream.ReadObject();

                foreach (var obj in sequence)
                {
                    if (obj is DerOctetString octetString)
                    {
                        var key = octetString.GetEncoded();

                        return Arrays.CopyOfRange(key, PrivateKeyStartIndex, key.Length);
                    }
                }

                throw new Exception(KeyDataNotFound);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        throw new InvalidOperationException(DerToPemConversion);
    }

    private object ParsePemObject()
    {
        try
        {
            using var reader = new StringReader(_pemObjectString);

            using var pemParser = new PemReader(reader);

            return pemParser.ReadObject();
        }
        catch (IOException ex)
        {
            throw new Exception(ErrorReadingPemObject, ex);
        }
    }
}