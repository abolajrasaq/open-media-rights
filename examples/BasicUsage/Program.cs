using OpenMediaRights;
var id = Guid.NewGuid();
var asset = new MediaAsset(id, AssetLifecycle.Active, RightsStanding.Verified, MediaRights.Stream | MediaRights.Download);
// Only a trusted rights verification workflow should mark a grant as Verified.
var grants = new[] { new LicenseGrant(Guid.NewGuid(), id, "NG", GrantVerification.Verified,
    MediaRights.Stream, DateTimeOffset.UtcNow.AddDays(-2), DateTimeOffset.UtcNow.AddDays(30)) };
var decision = MediaRightsPolicy.Evaluate(asset, grants, new PermissionRequest(id, MediaAction.Stream, "NG"));
Console.WriteLine($"Allowed: {decision.Allowed}; Reason: {decision.Code}; Grant: {decision.MatchedGrantId}");