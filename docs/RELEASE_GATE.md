# Preview release gate — v0.1.0-preview.1

## Owner approval
On 2026-10-21, the maintainer authorized publication of the standalone toolkit under MIT. This is authorization to publish a **development preview**, not a claim of independent security certification.

## Verified automated checks
- .NET 8 build, executable synthetic authorization tests, demo, and NuGet packaging run in GitHub Actions.
- The latest commit's checks must pass before merge.

## Review performed
- Checked repository diff and documentation for obvious proprietary SPRED code and secrets; no such material identified in the public package.
- Reviewed fail-closed asset checks, action switches, territory normalization, grant verification, time-window semantics, and exceptional input handling.
- Added bounded grant enumeration and negative coverage for Unicode confusables.

## Unresolved release limitations
- **No independent security audit or formal threat model.** Not production-certified.
- Upstream systems must verify rights, settle exclusive-grant/exclusion conflicts and obtain trusted territorial information.
- No authentication, persistence, DRM, contract validation or enforcement endpoints are bundled.
- No demonstrated external users or adoption yet.

Do not represent this package as a legal rights determination or deploy it as the only gate for a commercial media service.

## Shipping
- [x] Maintainer approval for MIT preview
- [x] Repository separation from SPRED
- [x] Preliminary source/security inspection
- [ ] Latest head GitHub Actions succeeds
- [ ] Independent audit and external integration (future production milestone)
