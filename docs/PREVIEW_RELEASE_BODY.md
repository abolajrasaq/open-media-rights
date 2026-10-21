# Open Media Rights Toolkit v0.1.0-preview.1

First public **development preview** of a dependency-free, MIT-licensed .NET 8 media rights decision helper.

It evaluates whether a host-supplied asset and verified rights grants allow streaming, downloading, sharing, monetization or resale. It considers asset status, operational action switches, territorial scope, grant verification and effective dates.

## What's included

- A standalone .NET 8 rights policy library with typed allow/deny reasons
- **58 automated synthetic permission checks**
- A self-contained CLI usage example
- A local-only ASP.NET Core example with allow/deny HTTP tests
- A separate .NET consumer example that restores and exercises the generated NuGet package
- A quickstart, integration guide, architecture notes, contribution guidelines and community feedback issue

## Try it from source

```bash
git clone https://github.com/abolajrasaq/open-media-rights.git
cd open-media-rights
dotnet run --project tests/OpenMediaRights.SmokeTests/OpenMediaRights.SmokeTests.csproj -c Release
dotnet run --project examples/BasicUsage/BasicUsage.csproj -c Release
```

The attached `.nupkg` artifact is supplied for local evaluation. **It has not been published to NuGet.org.**

## Important limitations

This package does **not** prove copyright ownership, authenticate users, verify legal rights documents, resolve conflicting exclusive grants, issue DRM or entitlement tokens, or protect streaming endpoints by itself. Host systems must supply authoritative rights records and trusted territory information, then enforce denials at the point of content delivery. The project has **not undergone an independent security audit** and is not certified for production use.

See [the quickstart](https://github.com/abolajrasaq/open-media-rights/blob/main/docs/QUICKSTART.md), [architecture guide](https://github.com/abolajrasaq/open-media-rights/blob/main/docs/ARCHITECTURE.md), and [integration feedback request](https://github.com/abolajrasaq/open-media-rights/issues/5).

Maintained by [abolajrasaq](https://github.com/abolajrasaq). New project: no external adoption is claimed.
