# Threat model and integration security review

**Status:** maintainer-authored design review for the open-source development preview. This document is **not** an independent audit, a copyright opinion, a penetration test, or a claim of production certification.

**Scope:** `MediaRightsPolicy.Evaluate(MediaAsset, IEnumerable<LicenseGrant>, PermissionRequest)` and the trust boundary around the host. The sample HTTP app is a local-only demonstration and does not protect media.

## Security objective

An action must be **denied** unless the host has authenticated/authorized the caller *and* the engine is given a current, authoritative snapshot in which all these conditions hold:
1. The requested asset ID matches an existing active asset.
2. The asset's global rights standing is verified; it is neither disputed nor revoked.
3. The relevant operational action switch is enabled.
4. The host-supplied territory is valid and matches at least one applicable grant (or its worldwide scope).
5. A verified, currently effective, matching grant authorizes the requested action.
6. The host enforces the decision at the actual content/transaction boundary.

The engine evaluates (1)-(5) against **supplied inputs**, but does not independently establish their truth. It does not perform (6).

## Assets and trust boundaries

- **Protected assets:** copyright licenses, access to media, monetisation/payout permissions, sensitive user and creator rights information, integrity of the decision log.
- **Untrusted inputs:** public HTTP headers, query parameters, client timestamps, IP geolocation claims from the browser, uploader rights declarations, identifiers received from a caller, unreviewed copyright documents, and data from untrusted external services.
- **Trusted by contract:** asset state and operational switches from the host's authoritative source; grants verified by a separate review workflow; the host's authenticated caller identity; trusted server-side territory resolver; server-controlled UTC evaluation timestamp.
- **Output:** an allow/deny decision code and, for an allow, the matching grant's ID. This is **not** a cryptographic entitlement or playable media token.

## Threats, mitigations and remaining work

| Threat | Attack or failure | Present mitigation | Required host responsibility |
|---|---|---|---|
| Forged rights | Uploader marks their own licence as verified | Evaluator requires `Verified` | Validate and authorize verification transitions, documents, signers and ownership upstream |
| Territory spoofing | Caller submits a country of their choosing | Only syntactic country checks in library | Resolve territory from trusted server-side signals; never let request parameters choose it |
| Time travel | Caller asks policy to evaluate before an expired licence ended | Effective-date checks use `request.At` when provided | Use server-controlled UTC time; do not pass a client-supplied evaluation timestamp |
| Stale permissions | Dispute or takedown occurs after a grant snapshot | Asset status and grant gates deny when current | Read consistent authoritative state, invalidate caches, re-evaluate on delivery and payments |
| Cross-asset grant | Grant for title A used to access title B | Grant `AssetId` must match asset | Enforce asset ownership and user authorization in host |
| Malformed/grant-bomb input | Huge or throwing grant enumerable | Fail-closed on exceptions and after 10,000 items | Avoid unbounded, remote or slow enumerators; enforce input size and time budgets before calling policy |
| Contract contradictions | Exclusivity, embargo, holdback or exclusion contradicts additive grants | No exclusivity adjudication in preview | Resolve conflicts before marking rights Verified and before building the grant snapshot |
| Denial bypass | A consumer logs denial but still streams/downloads | Typed result is available | Enforce on the actual resource endpoint; never rely only on a read-only policy-check API |
| Privileged logging | Sensitive rights metadata appears in public logs | Decision includes reason and optional grant ID, not legal documents | Restrict log access; avoid documents, secrets or personal data |
| Missing territory | Host omits or sends invalid geography | Fail-closed invalid country | Establish an explicit safe policy for ambiguous/unknown territory; never fall back silently to worldwide |
| Non-ASCII confusables | Visually similar country codes bypass parsing | ASCII-only two-letter checks | Normalize country sources in a trusted service and validate actual ISO assignment |
| Revoked signed offline access | Policy changes after an offline token is issued | No offline entitlement system | Implement revocation, expiry and device/session logic in a separate enforcement layer |

## Policy characteristics verified by tests

- Unverified, disputed, revoked, suspended and removed assets deny.
- An enabled operational action alone does not grant legal permission.
- Verified grants must match the asset, time and territory.
- Invalid territory inputs and unknown enum actions deny.
- Throwing and oversized grant enumerations deny, including when a matching grant occurred earlier in the sequence.
- End dates are **exclusive**, so a licence ending at the evaluation time is expired.
- An allowed decision includes the authorizing grant's ID; a denied decision never includes one.

These are synthetic unit/smoke checks and do not establish correctness of an integrated service.

## Priorities before production use

1. Independent review of evaluator, callers, and real grant verification workflow.
2. Threat-model the full authenticated media request path and rights revocation latency.
3. Test true end-to-end denial enforcement for stream, download, P2P and monetization.
4. Verify timezone, territorial policy, conflicting licenses, and all business-specific exclusions.
5. Add rate limiting, monitoring, exception handling, bounds and timeouts in the host.
6. Review data retention, auditability and legal requirements with qualified counsel.

Report a vulnerability privately following [SECURITY.md](../SECURITY.md). Do not submit real rights documents or secret credentials to public issues.
