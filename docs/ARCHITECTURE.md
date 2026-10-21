# Architecture and trust boundaries

```text
Trusted host application
   | authenticates caller, verifies grants, resolves exclusivity,
   | locates rights records, resolves territory from trusted infrastructure
   v
MediaRightsPolicy.Evaluate(asset, grants, request)
   | checks lifecycle, rights standing, operational action gate,
   | country scope, verification, effective dates, permission flags
   v
PermissionDecision (allow/deny + reason + authorizing grant ID)
   |
   v
Host enforces at streaming/download/payment endpoint and audits decision
```

This library has no rights database or authoritative grant verification. It is intentionally a **pure decision helper**. Grant collections are capped at 10,000 inspected entries. The host must supply bounded, trusted and consistent inputs. Large or remote enumerables must not be exposed directly to it.

Grant date windows are **start-inclusive, end-exclusive**. Rights grants are additive, not an adjudication of contractual exclusivity or rights revocations. Contradictory grants must be resolved upstream before inputs are marked verified.

`PermissionDecision.Allowed` is **not** a content entitlement, media access token, ownership certificate or a contract interpretation. Use server-side enforcement for each protected operation. Do not trust client flags or territory queries.

See [Integration](INTEGRATION.md), [initial threat model](THREAT_MODEL.md), and [Policy API demo](../examples/PolicyApi/README.md).
