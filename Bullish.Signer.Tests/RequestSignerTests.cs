using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace Bullish.Signer.Tests;

public class RequestSignerTests
{
    private const string PrivateKeyString = "PVT_R1_2qZH5Pi9MJ7P3AB8Q4es6Mv56q54omL5xbpYZG4CC75GUPSEe";
    private const string PublicKeyString = "PUB_R1_6ZNjnsuzXsdhgMzP2JkfWYtWVPfajpzvgA7xn8ytaTCEJoXkYk";

    private const string Payload = "{\"accountId\":\"222000000000000\",\"nonce\":1639393131,\"expirationTime\":1639393731,\"biometricsUsed\":false,\"sessionKey\":null}";

    private const string PublicKeyPem =
        """
        -----BEGIN PUBLIC KEY-----
        MDkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDIgAC2/3MIbg4ZXSZlz14bdIBYMy/+2uGF24afvHuMrrgR8U=
        -----END PUBLIC KEY-----
        """;

    private const string PrivateKeyPem =
        """
        -----BEGIN PRIVATE KEY-----
        MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgYirTZSx+5O8Y6tlG
        cka6W6btJiocdrdolfcukSoTEk+hRANCAAQkvPNu7Pa1GcsWU4v7ptNfqCJVq8Cx
        zo0MUVPQgwJ3aJtNM1QMOQUayCrRwfklg+D/rFSUwEUqtZh7fJDiFqz3
        -----END PRIVATE KEY-----
        """;

    [Fact]
    public void TestSigning()
    {
        var signature = RequestSigner.Sign(PrivateKeyString, PublicKeyString, Payload);

        var isVerified = RequestSigner.Verify(signature, PublicKeyString, Payload);

        Assert.True(isVerified);
    }

    [Fact]
    public void TestDeterministicSigning()
    {
        // Test signing in a deterministic manner, by providing R and S for signature
        var r = new BigInteger("20261800083856547382386090746389798364518017217430138826487956475158205104095");
        var s = new BigInteger("34952731132222952690996141802334990093291568642697605774462779655400653903310");

        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(Payload));

        var signature = RequestSignerImpl.ConvertRawRandSofSignatureToEosFormat(r, s, digest, PublicKeyPem);

        var expectedSignature = "SIG_R1_KacEPz6SXLCa2qf5kukNEbGUTeps5Ht2rfdiJ71oqxWT21MsS7Po2jY9Dv1CfNZ1BMtD68YpbsxuchKmLA1rtvQzTsE9jx";

        Assert.Equal(expectedSignature, signature);
    }

    [Fact]
    public void TestPublicKeyToPemConversion()
    {
        var eosPublicKey = RequestSignerImpl.GetEosPublicKey(PublicKeyString);
        var publicKeyPem = RequestSignerImpl.ConvertEosPublicKeyToPemFormat(eosPublicKey.EncodedPublicKey);

        Assert.Equal(PublicKeyPem, publicKeyPem);
    }

    [Fact]
    public void TestIsCanonical()
    {
        // Not canonical
        var r = new BigInteger("77084086505121098653360175765105376663257513089363980947379235255146470098464");
        var s = new BigInteger("74458908016279476045337701921353349444661569328555421978725854884969880525517");
        Assert.False(RequestSignerImpl.IsCanonical(r, s));

        // Is Canonical
        r = new BigInteger("21439606586086916919810396936196675864288160958535490438215717714346686570448");
        s = new BigInteger("11851666679188574045777565458485980702689528428544056372056374378522940762346");
        Assert.True(RequestSignerImpl.IsCanonical(r, s));
    }

    [Fact]
    public void TestDecodeEosPrivateKey()
    {
        var eosPrivateKey = RequestSignerImpl.GetEosPrivateKey(PrivateKeyString);

        var ecPrivateKeyParameters = eosPrivateKey.PrivateKey;

        var d = ecPrivateKeyParameters.D.ToString();
        Assert.Equal("1886843130467617647594287703515141520168242674442259854791134759367935457281", d);
    }

    [Fact]
    public void TestDecodeEosPublicKey()
    {
        var eosPublicKey = RequestSignerImpl.GetEosPublicKey(PublicKeyString);

        // Ensure the re-encoded key matches the original
        Assert.Equal(PublicKeyString, eosPublicKey.EncodedPublicKey);

        // Ensure Point Q matches
        var x = eosPublicKey.PublicKey.Q.XCoord.ToString();
        var y = eosPublicKey.PublicKey.Q.YCoord.ToString();

        Assert.Equal("dbfdcc21b838657499973d786dd20160ccbffb6b86176e1a7ef1ee32bae047c5", x);
        Assert.Equal("73a86c61d05e44c154195ac46973fa861c5843efb107a6bba183ea8756bc635a", y);
    }

    [Fact]
    public void TestPemProcessorWithEcPrivateKey()
    {
        var pemProcessor = new PemProcessor(PrivateKeyPem);

        Assert.Equal("PRIVATE KEY", pemProcessor.Type);

        var derBytes = pemProcessor.GetKeyData();

        var asn1 = Asn1Object.FromByteArray(derBytes);

        Assert.IsAssignableFrom<Asn1Object>(asn1);
    }
}