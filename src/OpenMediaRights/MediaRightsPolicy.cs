namespace OpenMediaRights;

/// <summary>Fail-closed, dependency-free evaluation against trusted upstream rights inputs.</summary>
public static class MediaRightsPolicy
{
    private const string Worldwide = "WORLDWIDE";
    public static PermissionDecision Evaluate(MediaAsset? asset, IEnumerable<LicenseGrant>? grants, PermissionRequest? request)
    {
        if (request is null || request.AssetId == Guid.Empty) return PermissionDecision.Deny(DecisionCode.InvalidRequest);
        if (asset is null || asset.Id == Guid.Empty || asset.Id != request.AssetId) return PermissionDecision.Deny(DecisionCode.AssetNotFound);
        var lifecycle = asset.Lifecycle switch {
            AssetLifecycle.Active => DecisionCode.Allowed, AssetLifecycle.Suspended => DecisionCode.AssetSuspended,
            AssetLifecycle.Removed => DecisionCode.AssetRemoved, _ => DecisionCode.AssetNotActive
        };
        if (lifecycle != DecisionCode.Allowed) return PermissionDecision.Deny(lifecycle);
        var standing = asset.Standing switch {
            RightsStanding.Verified => DecisionCode.Allowed, RightsStanding.Pending => DecisionCode.AssetRightsPending,
            RightsStanding.Disputed => DecisionCode.AssetRightsDisputed, RightsStanding.Revoked => DecisionCode.AssetRightsRevoked,
            _ => DecisionCode.AssetRightsUnverified
        };
        if (standing != DecisionCode.Allowed) return PermissionDecision.Deny(standing);
        var required = request.Action switch {
            MediaAction.Stream => MediaRights.Stream, MediaAction.Download => MediaRights.Download,
            MediaAction.Share => MediaRights.Share, MediaAction.Monetize => MediaRights.Monetize,
            MediaAction.Resell => MediaRights.Resell, _ => MediaRights.None
        };
        if (required == MediaRights.None) return PermissionDecision.Deny(DecisionCode.UnknownAction);
        if (!asset.EnabledActions.HasFlag(required)) return PermissionDecision.Deny(DecisionCode.OperationDisabled);
        var country = NormalizeCountryCode(request.CountryCode);
        if (country is null) return PermissionDecision.Deny(DecisionCode.InvalidCountryCode);
        if (grants is null) return PermissionDecision.Deny(DecisionCode.NoMatchingGrant);
        var at = request.At ?? DateTimeOffset.UtcNow;
        bool scoped = false, verified = false, active = false;
        Guid? matching = null;
        try {
            foreach (var grant in grants) {
                if (grant is null || grant.Id == Guid.Empty || grant.AssetId != asset.Id) continue;
                var scope = NormalizeGrantTerritory(grant.Territory);
                if (scope is null || (scope != Worldwide && scope != country)) continue;
                scoped = true;
                if (grant.Verification != GrantVerification.Verified) continue;
                verified = true;
                if (grant.StartsAt is not null && grant.EndsAt is not null && grant.StartsAt >= grant.EndsAt) continue;
                if (grant.StartsAt is not null && at < grant.StartsAt) continue;
                if (grant.EndsAt is not null && at >= grant.EndsAt) continue;
                active = true;
                if ((grant.GrantedActions & required) == required) matching ??= grant.Id;
            }
        } catch (Exception) { return PermissionDecision.Deny(DecisionCode.EvaluationFailed); }
        if (matching is not null) return PermissionDecision.Permit(matching.Value);
        if (!scoped) return PermissionDecision.Deny(DecisionCode.NoMatchingGrant);
        if (!verified) return PermissionDecision.Deny(DecisionCode.GrantNotVerified);
        if (!active) return PermissionDecision.Deny(DecisionCode.GrantNotInEffect);
        return PermissionDecision.Deny(DecisionCode.ActionNotGranted);
    }
    private static string? NormalizeCountryCode(string? value) {
        if (value is null || value.Length > 32) return null;
        var candidate = value.Trim().ToUpperInvariant();
        return candidate.Length == 2 && candidate.All(c => c is >= 'A' and <= 'Z') ? candidate : null;
    }
    private static string? NormalizeGrantTerritory(string? value) {
        if (value is null || value.Length > 32) return null;
        var candidate = value.Trim().ToUpperInvariant();
        return candidate == Worldwide ? Worldwide : NormalizeCountryCode(candidate);
    }
}