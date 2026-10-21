namespace OpenMediaRights;
public static class MediaRightsPolicy {
 public static PermissionDecision Evaluate(MediaAsset? asset, IEnumerable<LicenseGrant>? grants, PermissionRequest? request) {
  if (request is null || request.AssetId==Guid.Empty) return PermissionDecision.Deny(DecisionCode.InvalidRequest);
  if (asset is null || asset.Id==Guid.Empty || asset.Id!=request.AssetId) return PermissionDecision.Deny(DecisionCode.AssetNotFound);
  var lifecycle=asset.Lifecycle switch { AssetLifecycle.Active=>DecisionCode.Allowed, AssetLifecycle.Suspended=>DecisionCode.AssetSuspended, AssetLifecycle.Removed=>DecisionCode.AssetRemoved, _=>DecisionCode.AssetNotActive };
  if (lifecycle!=DecisionCode.Allowed) return PermissionDecision.Deny(lifecycle);
  var standing=asset.Standing switch { RightsStanding.Verified=>DecisionCode.Allowed, RightsStanding.Pending=>DecisionCode.AssetRightsPending, RightsStanding.Disputed=>DecisionCode.AssetRightsDisputed, RightsStanding.Revoked=>DecisionCode.AssetRightsRevoked, _=>DecisionCode.AssetRightsUnverified };
  if (standing!=DecisionCode.Allowed) return PermissionDecision.Deny(standing);
  var required=request.Action switch { MediaAction.Stream=>MediaRights.Stream, MediaAction.Download=>MediaRights.Download, MediaAction.Share=>MediaRights.Share, MediaAction.Monetize=>MediaRights.Monetize, MediaAction.Resell=>MediaRights.Resell, _=>MediaRights.None };
  if (required==MediaRights.None) return PermissionDecision.Deny(DecisionCode.UnknownAction);
  if (!asset.EnabledActions.HasFlag(required)) return PermissionDecision.Deny(DecisionCode.OperationDisabled);
  var country=Normalize(request.CountryCode);
  if (country is null || country=="WORLDWIDE") return PermissionDecision.Deny(DecisionCode.InvalidCountryCode);
  if (grants is null) return PermissionDecision.Deny(DecisionCode.NoMatchingGrant);
  bool scoped=false,verified=false,active=false;
  Guid? authorizing=null;
  var at=request.At??DateTimeOffset.UtcNow;
  try {
   foreach(var g in grants) {
    if(g is null || g.Id==Guid.Empty || g.AssetId!=asset.Id) continue;
    var scope=Normalize(g.Territory);
    if(scope is null || (scope!="WORLDWIDE" && scope!=country)) continue;
    scoped=true;
    if(g.Verification!=GrantVerification.Verified) continue;
    verified=true;
    if(g.StartsAt is not null && g.EndsAt is not null && g.StartsAt>=g.EndsAt) continue;
    if(g.StartsAt is not null && at<g.StartsAt || g.EndsAt is not null && at>=g.EndsAt) continue;
    active=true;
    if((g.GrantedActions&required)==required) authorizing??=g.Id;
   }
  } catch(Exception) { return PermissionDecision.Deny(DecisionCode.EvaluationFailed); }
  if(authorizing is not null) return PermissionDecision.Permit(authorizing.Value);
  if(!scoped) return PermissionDecision.Deny(DecisionCode.NoMatchingGrant);
  if(!verified) return PermissionDecision.Deny(DecisionCode.GrantNotVerified);
  if(!active) return PermissionDecision.Deny(DecisionCode.GrantNotInEffect);
  return PermissionDecision.Deny(DecisionCode.ActionNotGranted);
 }
 private static string? Normalize(string? territory) {
  if (territory is null || territory.Length>32) return null;
  var s=territory.Trim().ToUpperInvariant();
  if(s=="WORLDWIDE") return s;
  return s.Length==2 && s.All(c=>c is >= 'A' and <= 'Z') ? s:null;
 }
}