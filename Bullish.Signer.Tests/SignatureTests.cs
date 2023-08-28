using System.Security.Cryptography;
using System.Text;

namespace Bullish.Signer.Tests;

public class SignatureTests
{
    // The private key in this test is coded into the source.
    // In production settings please retrieve the private key in a more secure manner, such as through an environment variable
    private const string PrivateKeyString = "PVT_R1_2qZH5Pi9MJ7P3AB8Q4es6Mv56q54omL5xbpYZG4CC75GUPSEe";
    private const string PublicKeyString = "PUB_R1_6ZNjnsuzXsdhgMzP2JkfWYtWVPfajpzvgA7xn8ytaTCEJoXkYk";
    
    [Fact]
    public void TestSigning()
    {
        var request = "{\"accountId\":\"222000000000000\",\"nonce\":1639393131,\"expirationTime\":1639393731,\"biometricsUsed\":false,\"sessionKey\":null}";
        
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(request));
        
        var privateKey = RequestSigner.DecodePrivateKey(PrivateKeyString);
        var publicKey = RequestSigner.DecodePublicKey(PublicKeyString);

        // String signature = requestSigner.signRequest(digest, privateKey, publicKey);
        // assertTrue(requestSigner.verifySignature(digest, signature, publicKey));
    }

    [Fact]
    public void TestDecodeEosPrivateKey()
    {
        var eosPrivateKey = RequestSigner.DecodePrivateKey(PrivateKeyString);

        var ecPrivateKeyParameters = eosPrivateKey.PrivateKey;

        var d = ecPrivateKeyParameters.D.ToString();
        Assert.Equal("1886843130467617647594287703515141520168242674442259854791134759367935457281", d);
    }

    [Fact]
    public void TestDecodeEosPublicKey()
    {
        var eosPublicKey = RequestSigner.DecodePublicKey(PublicKeyString);

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
        var privateKey = 
            """
            -----BEGIN PRIVATE KEY-----
            MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgYirTZSx+5O8Y6tlG
            cka6W6btJiocdrdolfcukSoTEk+hRANCAAQkvPNu7Pa1GcsWU4v7ptNfqCJVq8Cx
            zo0MUVPQgwJ3aJtNM1QMOQUayCrRwfklg+D/rFSUwEUqtZh7fJDiFqz3
            -----END PRIVATE KEY-----
            """;

        var pemProcessor = new PemProcessor(privateKey);

        Assert.Equal("PRIVATE KEY", pemProcessor.Type);

        var pkBytes = pemProcessor.GetKeyData();
    }
}