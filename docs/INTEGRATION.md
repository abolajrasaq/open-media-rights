# Integration guide

This is a standalone, synchronous decision helper. It does not validate copyright ownership or contracts.

1. Authenticate the user and authorize their request at your application boundary.
2. Verify rights holder evidence, contractual conflicts, exclusions and exclusive arrangements upstream.
3. Obtain a bounded, consistent snapshot of asset state and grants from your authoritative datastore.
4. Determine the territory from trusted server-side information. Never rely on a user-supplied country code for territorial enforcement.
5. Call MediaRightsPolicy.Evaluate and enforce the decision on your stream, download, transfer or transaction endpoint.
6. Audit the action, decision code and (when allowed) the matching grant ID. Keep sensitive records private.
7. Re-evaluate on every critical transaction and after rights changes, disputes, expiries and revocations.

This policy unions verified, valid grants. Your application must reject contradictory exclusions, embargoes and incompatible exclusive grants before marking assets Verified. A fingerprint is not ownership evidence. Use this preview as one decision layer, not a complete entitlement or rights-management platform.