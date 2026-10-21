# Local-only HTTP integration example

This sample demonstrates using the toolkit as a decision dependency inside an ASP.NET Core minimal API, not a streaming endpoint.

**Security:** It deliberately binds to `127.0.0.1:5187` and uses static synthetic data. It has no user authentication, licence-document verification, datastore integration or media delivery route. **Do not publish, proxy, containerize, or expose this example as a live public API.** It is not a deployable media service.

From the repository root, with the .NET 8 SDK:

```bash
dotnet run --project examples/PolicyApi/PolicyApi.csproj
```

In another terminal:

```bash
curl -i http://127.0.0.1:5187/demo/rights/stream
curl -i http://127.0.0.1:5187/demo/rights/download
curl -i http://127.0.0.1:5187/demo/rights/share
```

Expected statuses: `200` for streaming, `403` for downloading and sharing. Even though the asset's download operational switch is enabled, the licence only grants streaming. This demonstrates that **both the operational switch and a verified grant must permit the operation**.

Adding `?territory=US` to the URL cannot change the result: the demonstration supplies its territory within the server, not from user input. In production, replace the fixed country with a trustworthy server-side resolver.

## Production integration contract

An actual host service must: authenticate and authorize the caller, load the authoritative asset and all relevant grants in a consistent bounded snapshot, independently verify grants and resolve exclusivity/revocation conflicts, derive territory from trusted infrastructure, enforce deny decisions at the media delivery boundary, and audit the decision. See [the integration guide](../../docs/INTEGRATION.md) and [security guidance](../../SECURITY.md).

The `/demo/rights` routes are read-only checks and protect **no content**. Their JSON response is not an entitlement token.
