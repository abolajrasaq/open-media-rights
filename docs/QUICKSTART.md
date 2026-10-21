# Start here (10-minute developer walkthrough)

**Requirements:** .NET 8 SDK and Git. No API keys, database, storage or external media files needed.

```bash
git clone https://github.com/abolajrasaq/open-media-rights.git
cd open-media-rights
dotnet build src/OpenMediaRights/OpenMediaRights.csproj -c Release
dotnet run --project tests/OpenMediaRights.SmokeTests/OpenMediaRights.SmokeTests.csproj -c Release
dotnet run --project examples/BasicUsage/BasicUsage.csproj
```

Next, run the [local HTTP demonstration](../examples/PolicyApi/README.md) to see independent operational and grant controls in an ASP.NET Core host.

## Model

A decision needs **all** these conditions: active asset, verified asset standing, enabled operational action, syntactically valid request country, and a verified, effective grant for the asset, territory and action.

A `RightsStanding.Verified` flag alone is never enough. And a `GrantVerification.Verified` flag is an assertion from your *trusted upstream workflow*, not something the library authenticates.

## Try denial cases

- Change `RightsStanding.Verified` to `RightsStanding.Disputed`: denial.
- Change country from `NG` to `GH`: denial if the only grant is Nigeria-specific.
- Set `EnabledActions` to `MediaRights.None`: denial.
- Keep stream enabled on the asset but grant only download: streaming is denied.

## Ready to contribute?

Read [CONTRIBUTING](../CONTRIBUTING.md), then propose a focused test, documentation improvement or bug fix. Sensitive security issues must follow [SECURITY](../SECURITY.md).
