# Release gate: v0.1.0-preview.1

Status: **Draft / review required**. Green CI is necessary but not proof of legal correctness or production security.

## Verified in GitHub Actions
- .NET 8 library compiles.
- 56 deterministic synthetic checks pass (as of the last reviewed successful run).
- The example application executes.
- NuGet package construction succeeds. Embedded README and package metadata are being validated in subsequent CI.

## Must verify before merging or tagging
- [ ] Maintainer confirms they own or may license every public file under MIT.
- [ ] No proprietary SPRED source, client data, production configurations, credentials or private documentation were copied.
- [ ] A reviewer examines default-deny behavior, grant precedence, time windows, territory assumptions and potential denial-of-service through large enumerables.
- [ ] The documentation accurately distinguishes a rights decision from legal ownership or entitlement enforcement.
- [ ] Latest HEAD CI passes.
- [ ] Maintainer approves public release.

## Known limitations / open issues
- Two-letter territory validation is syntactic, not a complete ISO 3166-1 country list.
- Grants combine by union. Exclusions, revocations, cross-grant conflicts and exclusivity are not adjudicated by this evaluator.
- The library does not verify rights documents, authentication, DRM or license ownership.
- No formal threat model, independently conducted security audit, or consumer integration test has been completed.
- Consumer application must use trustworthy country information and authoritative grant snapshots.

## Publishing policy
The draft PR must remain unmerged until the maintainer's provenance and security checks are complete. No NuGet push or production-ready claim before those gates.
