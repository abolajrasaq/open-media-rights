# Consumer installation test (from a local NuGet package)

This example simulates a third-party .NET 8 application consuming **OpenMediaRights as a compiled package**, rather than referencing the repository's library source.

No OpenMediaRights package is claimed to be on NuGet.org. You can build and install the preview locally:

```bash
dotnet pack src/OpenMediaRights/OpenMediaRights.csproj -c Release -o ./artifacts
dotnet restore examples/PackageConsumer/PackageConsumer.csproj --source "$(pwd)/artifacts"
dotnet run --no-restore --project examples/PackageConsumer/PackageConsumer.csproj -c Release
```

Expected result: six sample assertions pass, including allowed streaming, denied download, country restriction, licence expiry, and authorizing-grant provenance.

The example uses only synthetic data. It neither issues playable media entitlements nor verifies actual contracts. Never rely on sample grants to authorize protected content.

For a real integration consult [the security boundaries](../../docs/ARCHITECTURE.md).
