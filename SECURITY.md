# Security policy

Open Media Rights Toolkit is an early development preview. The maintainers have **not commissioned an independent security audit** and do not represent it as production-certified copyright or authorization software.

Read the [threat model](docs/THREAT_MODEL.md) and [architecture/trust boundaries](docs/ARCHITECTURE.md) before using the evaluator in a media service. The library does not verify rights ownership, contracts, grants, identity, territories or entitlement tokens; an integrating host must do that separately.

## Reporting a vulnerability

Please **do not** disclose exploit details, production credentials, sensitive rights documents or personal data in public GitHub issues.

- If enabled on this repository, use GitHub's **Private vulnerability reporting** via the repository's Security tab.
- If that option is unavailable, open a public issue **without exploit specifics**, asking the maintainer to establish a private reporting channel. Do not include a proof of concept in the public issue.

Include the affected package version, environment, expected and observed behavior, and a synthetic non-sensitive reproduction once a private channel is established. We do not promise a particular response SLA at this early stage.

## Supported releases

| Version | Support level |
|---|---|
| v0.1.0-preview.1 | Best-effort development preview; no production security support guarantee |

Security fixes should preserve fail-closed behavior and add both positive and negative tests.
