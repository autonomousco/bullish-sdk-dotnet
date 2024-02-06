using Microsoft.Extensions.Configuration;

namespace Bullish.Tests;

public static class TestConfiguration
{
    public static readonly string HmacPublicKey;
    public static readonly string HmacPrivateKey;

    public static readonly string EcdsaPublicKey;
    public static readonly string EcdsaPrivateKey;
    public static readonly string EcdsaMetadata;

    static TestConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        HmacPublicKey = config["BullishHmac:PublicKey"] ?? string.Empty;
        HmacPrivateKey = config["BullishHmac:PrivateKey"] ?? string.Empty;

        EcdsaPublicKey = config["BullishEcdsa:PublicKey"] ?? string.Empty;
        EcdsaPrivateKey = config["BullishEcdsa:PrivateKey"] ?? string.Empty;
        EcdsaMetadata = config["BullishEcdsa:Metadata"] ?? string.Empty;
    }

    public static void RequiresHmac()
    {
        Skip.If(string.IsNullOrWhiteSpace(HmacPublicKey), "Bullish HMAC Public Key was not found. Please add it to appsettings.json");
        Skip.If(string.IsNullOrWhiteSpace(HmacPrivateKey), "Bullish HMAC Private Key was not found. Please add it to appsettings.json");
    }

    public static void RequiresEcdsa()
    {
        Skip.If(string.IsNullOrWhiteSpace(EcdsaPublicKey), "Bullish ECDSA Public Key was not found. Please add it to appsettings.json");
        Skip.If(string.IsNullOrWhiteSpace(EcdsaPrivateKey), "Bullish ECDSA Private Key was not found. Please add it to appsettings.json");
        Skip.If(string.IsNullOrWhiteSpace(EcdsaMetadata), "Bullish ECDSA Metadata was not found. Please add it to appsettings.json");
    }
}