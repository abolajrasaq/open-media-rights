# Contributing to Open Media Rights Toolkit

Contributions from independent developers, educators and media-platform builders are welcome. This project is an early development preview, not a complete copyright-verification service.

## Start with a reproducible change

1. Read the [quickstart](docs/QUICKSTART.md), [architecture](docs/ARCHITECTURE.md) and [security boundaries](SECURITY.md).
2. Check the [roadmap](docs/ROADMAP.md) and existing issues; create an issue before large API changes.
3. Build and run the standalone test executable:
   ```bash
   dotnet build src/OpenMediaRights/OpenMediaRights.csproj -c Release
   dotnet run --project tests/OpenMediaRights.SmokeTests/OpenMediaRights.SmokeTests.csproj -c Release
   ```
4. Add positive **and negative** cases for all authorization-rule changes.
5. Demonstrate that the packed library still works in a consumer application:
   ```bash
   dotnet pack src/OpenMediaRights/OpenMediaRights.csproj -c Release -o ./artifacts
   dotnet restore examples/PackageConsumer/PackageConsumer.csproj --source "$(pwd)/artifacts"
   dotnet run --no-restore --project examples/PackageConsumer/PackageConsumer.csproj -c Release
   ```
6. Submit a focused pull request explaining test outcomes and any security implications.

## Good first contributions

- Improve explanations of allow/deny reasons and policy boundaries.
- Add edge-case tests involving country scopes and licence time windows.
- Add **synthetic-only** integration examples for common .NET host patterns.
- Improve documentation and cross-platform developer setup.

Never submit proprietary SPRED platform code, real legal documents, personal information, production secrets, or copyrighted samples. Do not publish exploit details in public issues; follow [SECURITY.md](SECURITY.md).

### Report an integration

If you try this library in a prototype, open an issue with your **public project or example only**, your .NET version, which feature you tested, and what was confusing. There is no requirement to share your private repository or sensitive rights data. We will not count exploratory feedback as production adoption.
