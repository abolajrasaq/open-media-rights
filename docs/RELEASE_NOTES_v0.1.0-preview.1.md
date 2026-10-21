# Release notes — v0.1.0-preview.1

**Status:** Development preview (MIT). No independent security certification, no NuGet publication.

## Included
- Dependency-free .NET 8 rights decision helper for streaming, downloading, sharing, monetisation and resale.
- Fail-closed asset state, verified rights standing, operational gates, territorial grant scope, and time-window evaluation.
- 58 synthetic smoke tests verified through GitHub Actions.
- Local-only ASP.NET Core example with HTTP allow/deny checks.
- Developer quickstart, architecture diagram, integration notes, contribution guide and roadmap.
- Independent packaged consumer sample: six synthetic checks against the compiled local NuGet artifact.
- GitHub Actions archives the locally built `.nupkg` as a downloadable workflow artifact.

## Install from source
```bash
git clone https://github.com/abolajrasaq/open-media-rights.git
cd open-media-rights
dotnet build src/OpenMediaRights/OpenMediaRights.csproj -c Release
dotnet run --project tests/OpenMediaRights.SmokeTests/OpenMediaRights.SmokeTests.csproj -c Release
dotnet run --project examples/BasicUsage/BasicUsage.csproj -c Release
```

## Release caveats
The library does not independently establish legal copyright ownership, verify documents, authenticate callers, adjudicate exclusive grants, issue entitlements or enforce access to protected content. Host applications must supply trusted territory and verified rights records and enforce the returned decision. **Not production audited.**

## For the maintainer
After verifying main branch CI, use GitHub Releases to create tag `v0.1.0-preview.1` at `main`, set it as a **pre-release**, and use this document as the release description. Do not claim a NuGet listing unless the package has actually been published.
