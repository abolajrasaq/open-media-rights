# Open Media Rights Toolkit

> **Published GitHub pre-release:** [v0.1.0-preview.1](https://github.com/abolajrasaq/open-media-rights/releases/tag/v0.1.0-preview.1) · MIT · .NET 8 · **not audited or published on NuGet.org.**

A dependency-free, .NET 8 media rights permission evaluator. Given a media asset, independently verified licence grants and a request, it returns a typed allow/deny decision for streaming, downloading, sharing, monetization or resale.

**This software does not establish copyright ownership or verify grants.** Your backend must validate rights documents, contractual conflicts and user identity, determine trustworthy territory, and enforce every decision at the point of delivery or transaction.

## Get the preview package

You can download the [compiled `.nupkg` from GitHub Releases](https://github.com/abolajrasaq/open-media-rights/releases/download/v0.1.0-preview.1/OpenMediaRights.0.1.0-preview.1.nupkg), or build it directly from source. The release is **not** a NuGet.org listing and is **not** independently security-audited. For local installation, see the [packaged consumer example](examples/PackageConsumer/README.md).

## New developer? Start here

Follow the [10-minute quickstart](docs/QUICKSTART.md), review the [trust boundary and architecture](docs/ARCHITECTURE.md), and run the [local HTTP integration example](examples/PolicyApi/README.md). You can also [install the locally generated NuGet package in an independent consumer app](examples/PackageConsumer/README.md) to test the real package boundary. **The preview is not published on NuGet.org.**

Read the [roadmap](docs/ROADMAP.md) to find focused contribution ideas, and the [threat model](docs/THREAT_MODEL.md) to understand security assumptions and host responsibilities.

## Quick start

Requires the .NET 8 SDK.

```bash
dotnet build src/OpenMediaRights/OpenMediaRights.csproj -c Release
dotnet run --project tests/OpenMediaRights.SmokeTests/OpenMediaRights.SmokeTests.csproj -c Release
dotnet pack src/OpenMediaRights/OpenMediaRights.csproj -c Release
```

## Example

```csharp
using OpenMediaRights;

var assetId = Guid.NewGuid();
var asset = new MediaAsset(assetId, AssetLifecycle.Active, RightsStanding.Verified, MediaRights.Stream);
var grant = new LicenseGrant(Guid.NewGuid(), assetId, "NG", GrantVerification.Verified, MediaRights.Stream);
var decision = MediaRightsPolicy.Evaluate(asset, new[] { grant },
    new PermissionRequest(assetId, MediaAction.Stream, "NG"));
Console.WriteLine(decision.Allowed); // true
```

The host must verify the grant before setting `GrantVerification.Verified`. Everything else is denied by default: suspended assets, pending/disputed/revoked rights, disabled actions, absent or unverified grants, incorrect territory, and invalid or expired date windows.

## Current limitations

- No persistence, DRM, identity, rights document review, billing or takedown management.
- A verified grant alone cannot override a disabled operational action gate.
- Country codes are ASCII two-letter *syntax* checks, not authoritative ISO country assignments.
- Multiple verified grants combine by union. Conflicting exclusive grants and exclusions **must** be resolved upstream.
- Grants must be supplied as a bounded authoritative snapshot; the evaluator denies if enumeration throws.
- Development preview only: compile, run CI and independently review licensing/provenance before release.

See [SECURITY.md](SECURITY.md), [CONTRIBUTING.md](CONTRIBUTING.md) and [LICENSE](LICENSE).

Maintained by [abolajrasaq](https://github.com/abolajrasaq). MIT-licensed development preview. No independent security certification or external adoption is claimed.
