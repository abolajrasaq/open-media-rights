namespace OpenMediaRights;
public enum MediaAction { Stream, Download, Share, Monetize, Resell }
[Flags] public enum MediaRights { None=0, Stream=1, Download=2, Share=4, Monetize=8, Resell=16 }
public enum AssetLifecycle { Draft, Active, Suspended, Removed }
public enum RightsStanding { Unverified, Pending, Verified, Disputed, Revoked }
public enum GrantVerification { Unverified, Pending, Verified, Rejected }
public sealed record MediaAsset(Guid Id, AssetLifecycle Lifecycle, RightsStanding Standing, MediaRights EnabledActions = MediaRights.None);
public sealed record LicenseGrant(Guid Id, Guid AssetId, string? Territory, GrantVerification Verification, MediaRights GrantedActions, DateTimeOffset? StartsAt=null, DateTimeOffset? EndsAt=null);
public sealed record PermissionRequest(Guid AssetId, MediaAction Action, string? CountryCode, DateTimeOffset? At=null);
public enum DecisionCode { Allowed, InvalidRequest, AssetNotFound, AssetNotActive, AssetSuspended, AssetRemoved, AssetRightsUnverified, AssetRightsPending, AssetRightsDisputed, AssetRightsRevoked, OperationDisabled, InvalidCountryCode, UnknownAction, NoMatchingGrant, GrantNotVerified, GrantNotInEffect, ActionNotGranted, EvaluationFailed }
public sealed record PermissionDecision(bool Allowed, DecisionCode Code, Guid? MatchedGrantId=null) {
 public static PermissionDecision Deny(DecisionCode code)=>new(false,code);
 public static PermissionDecision Permit(Guid grantId)=>new(true,DecisionCode.Allowed,grantId);
}