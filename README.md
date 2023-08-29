# Bullish API Client for .NET

A .NET API client and signer for the Bullish Exchange API. Documentation for the API can be found [here](https://api.exchange.bullish.com/docs/api/rest/)

## Installation

Add a reference to Bullish.Api.Client and instantiate the BxHttpClient with your API key and metadata.

```csharp
using Bullish.Api.Client;

const string Metadata = "eyJhY2Nv...";
const string PublicKey = "PUB_R1_8aUu...";
const string PrivateKey = "PVT_R1_2Yuu....";

var bxHttpClient = new BxHttpClient(PublicKey, PrivateKey, Metadata);
```

Now call `Login` to initialize the API client with the JWT and nonce.

```csharp
await bxHttpClient.Login();
```

## Using the Signer
Bullish uses the EOS WIF key format for public and private keys. For information about the format of keys you can review the [EOS Wallet Specification](https://developers.eos.io/manuals/eos/v2.0/keosd/wallet-specification)

Luckily all of this is taken care of by `Bullish.Signer`. 

To directly sign you own requests and bypass the API client, simply use the `RequestSigner`.


Creating the EOS key objects and signing:
```csharp
var jsonPayload = "{ foo: 42 }";
var privateKey = RequestSigner.GetEosPrivateKey(eosWifPrivateKey);
var publicKey = RequestSigner.GetEosPublicKey(eosWifPublicKey);

var signature = RequestSigner.Sign(privateKey, publicKey, jsonPayload);
```

Signing directly using the EOS WIF formatted keys:
```csharp
var jsonPayload = "{ foo: 42 }";
var privateKey = "PVT_R1_2Yuu....";
var publicKey = "PUB_R1_8aUu...";

var signature = RequestSigner.Sign(privateKey, publicKey, jsonPayload);
```