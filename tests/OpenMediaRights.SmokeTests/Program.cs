using OpenMediaRights;

var assetId = Guid.NewGuid();
var grantId = Guid.NewGuid();
var asOf = new DateTimeOffset(2026, 10, 21, 12, 0, 0, TimeSpan.Zero);
var activeAsset = new MediaAsset(assetId, AssetLifecycle.Active, RightsStanding.Verified,
    MediaRights.Stream | MediaRights.Download | MediaRights.Share | MediaRights.Monetize | MediaRights.Resell);
var goodGrant = new LicenseGrant(grantId, assetId, "NG", GrantVerification.Verified,
    MediaRights.Stream | MediaRights.Download, asOf.AddDays(-1), asOf.AddDays(1));

int passed = 0, failed = 0;
void Check(string title, DecisionCode expected, MediaAsset? asset = null,
    IEnumerable<LicenseGrant>? grants = null, PermissionRequest? request = null)
{
    var decision = MediaRightsPolicy.Evaluate(asset ?? activeAsset, grants ?? new[] { goodGrant },
        request ?? new PermissionRequest(assetId, MediaAction.Stream, "NG", asOf));
    if (decision.Code == expected && decision.Allowed == (expected == DecisionCode.Allowed))
    {
        Console.WriteLine("PASS " + title); passed++;
    }
    else
    {
        Console.Error.WriteLine("FAIL " + title + ": expected " + expected + ", got " + decision.Code); failed++;
    }
}
Check("verified Nigeria stream", DecisionCode.Allowed);
Check("case-insensitive country", DecisionCode.Allowed, request: new(assetId, MediaAction.Stream, " ng ", asOf));
Check("verified download", DecisionCode.Allowed, request: new(assetId, MediaAction.Download, "NG", asOf));
Check("no share right", DecisionCode.ActionNotGranted, request: new(assetId, MediaAction.Share, "NG", asOf));
Check("unknown territory", DecisionCode.NoMatchingGrant, request: new(assetId, MediaAction.Stream, "GH", asOf));
Check("missing country", DecisionCode.InvalidCountryCode, request: new(assetId, MediaAction.Stream, null, asOf));
Check("invalid country", DecisionCode.InvalidCountryCode, request: new(assetId, MediaAction.Stream, "NGA", asOf));
Check("ascii only country", DecisionCode.InvalidCountryCode, request: new(assetId, MediaAction.Stream, "NÉ", asOf));
Check("asset pending", DecisionCode.AssetRightsPending, asset: activeAsset with { Standing = RightsStanding.Pending });
Check("asset disputed", DecisionCode.AssetRightsDisputed, asset: activeAsset with { Standing = RightsStanding.Disputed });
Check("asset revoked", DecisionCode.AssetRightsRevoked, asset: activeAsset with { Standing = RightsStanding.Revoked });
Check("asset suspended", DecisionCode.AssetSuspended, asset: activeAsset with { Lifecycle = AssetLifecycle.Suspended });
Check("asset removed", DecisionCode.AssetRemoved, asset: activeAsset with { Lifecycle = AssetLifecycle.Removed });
Check("asset draft", DecisionCode.AssetNotActive, asset: activeAsset with { Lifecycle = AssetLifecycle.Draft });
Check("operational gate closed", DecisionCode.OperationDisabled, asset: activeAsset with { EnabledActions = MediaRights.None });
Check("grant pending", DecisionCode.GrantNotVerified, grants: new[] { goodGrant with { Verification = GrantVerification.Pending } });
Check("grant starts tomorrow", DecisionCode.GrantNotInEffect, grants: new[] { goodGrant with { StartsAt = asOf.AddDays(1) } });
Check("grant ends at evaluation time", DecisionCode.GrantNotInEffect, grants: new[] { goodGrant with { EndsAt = asOf } });
Check("malformed validity interval", DecisionCode.GrantNotInEffect, grants: new[] { goodGrant with { StartsAt = asOf, EndsAt = asOf } });
Check("wrong asset grant", DecisionCode.NoMatchingGrant, grants: new[] { goodGrant with { AssetId = Guid.NewGuid() } });
Check("worldwide grant", DecisionCode.Allowed, grants: new[] { goodGrant with { Territory = "WORLDWIDE" } },
    request: new(assetId, MediaAction.Stream, "KE", asOf));
Check("country not required to be grant-specific", DecisionCode.Allowed,
    grants: new[] { goodGrant with { Territory = " worldwide " } });
Check("grant not valid for missing scope", DecisionCode.NoMatchingGrant, grants: new[] { goodGrant with { Territory = null } });
Check("empty grant set", DecisionCode.NoMatchingGrant, grants: Array.Empty<LicenseGrant>());
Check("unknown action", DecisionCode.UnknownAction, request: new(assetId, (MediaAction)999, "NG", asOf));
Check("different request asset", DecisionCode.AssetNotFound, request: new(Guid.NewGuid(), MediaAction.Stream, "NG", asOf));
Check("permission union across verified grants", DecisionCode.Allowed,
    grants: new[] { goodGrant with { GrantedActions = MediaRights.None }, goodGrant with { Id = Guid.NewGuid() } });
Check("nonverified permissions cannot grant", DecisionCode.ActionNotGranted,
    grants: new[] { goodGrant with { GrantedActions = MediaRights.None },
        goodGrant with { Id = Guid.NewGuid(), Verification = GrantVerification.Unverified } });
Check("past grant expired", DecisionCode.GrantNotInEffect, grants: new[] { goodGrant with { EndsAt = asOf.AddMinutes(-1) } });
Check("future grant not started", DecisionCode.GrantNotInEffect, grants: new[] { goodGrant with { StartsAt = asOf.AddMinutes(1) } });
Check("unverified asset", DecisionCode.AssetRightsUnverified, asset: activeAsset with { Standing = RightsStanding.Unverified });
Check("wrong grant country", DecisionCode.NoMatchingGrant, grants: new[] { goodGrant with { Territory = "US" } });
Check("resale not granted", DecisionCode.ActionNotGranted, request: new(assetId, MediaAction.Resell, "NG", asOf));
Check("monetization not granted", DecisionCode.ActionNotGranted, request: new(assetId, MediaAction.Monetize, "NG", asOf));
Check("no action flag on asset", DecisionCode.OperationDisabled,
    asset: activeAsset with { EnabledActions = MediaRights.Download });
Check("malformed grant country", DecisionCode.NoMatchingGrant, grants: new[] { goodGrant with { Territory = "A!" } });
IEnumerable<LicenseGrant> ThrowAfterOne()
{
    yield return goodGrant;
    throw new InvalidOperationException("storage failed");
}
Check("throwing enumeration denies even after matching grant", DecisionCode.EvaluationFailed,
    grants: ThrowAfterOne());
Check("start inclusive", DecisionCode.Allowed, grants: new[] { goodGrant with { StartsAt = asOf } });
Check("end exclusive", DecisionCode.GrantNotInEffect, grants: new[] { goodGrant with { EndsAt = asOf } });
Check("invalid reversed validity window", DecisionCode.GrantNotInEffect,
    grants: new[] { goodGrant with { StartsAt = asOf.AddMinutes(2), EndsAt = asOf.AddMinutes(1) } });
Check("matching across time-zone offsets", DecisionCode.Allowed,
    request: new(assetId, MediaAction.Stream, "NG", asOf.ToOffset(TimeSpan.FromHours(1))));
Check("nonexistent empty asset request", DecisionCode.InvalidRequest,
    request: new(Guid.Empty, MediaAction.Stream, "NG", asOf));
Check("unknown lifecycle enum", DecisionCode.AssetNotActive,
    asset: activeAsset with { Lifecycle = (AssetLifecycle)999 });
Check("unknown rights standing enum", DecisionCode.AssetRightsUnverified,
    asset: activeAsset with { Standing = (RightsStanding)999 });
Check("unknown grant verification enum", DecisionCode.GrantNotVerified,
    grants: new[] { goodGrant with { Verification = (GrantVerification)999 } });
Check("empty grant ID ignored", DecisionCode.NoMatchingGrant,
    grants: new[] { goodGrant with { Id = Guid.Empty } });
Check("empty territory ignored", DecisionCode.NoMatchingGrant,
    grants: new[] { goodGrant with { Territory = "  " } });
Check("overlong request country denied", DecisionCode.InvalidCountryCode,
    request: new(assetId, MediaAction.Stream, new string('N', 100), asOf));
Check("overlong grant country ignored", DecisionCode.NoMatchingGrant,
    grants: new[] { goodGrant with { Territory = new string('G', 100) } });
Check("share explicitly granted", DecisionCode.Allowed,
    grants: new[] { goodGrant with { GrantedActions = MediaRights.Share } },
    request: new(assetId, MediaAction.Share, "NG", asOf));
Check("mixed verified and unverified, authorized by verified", DecisionCode.Allowed,
    grants: new[] { goodGrant with { Verification = GrantVerification.Pending, GrantedActions = MediaRights.Stream },
        goodGrant with { Id = Guid.NewGuid() } });

void CheckRaw(string title, PermissionDecision result, DecisionCode expected)
{
    if (result.Code == expected && result.Allowed == (expected == DecisionCode.Allowed))
    {
        Console.WriteLine("PASS " + title); passed++;
    }
    else
    {
        Console.Error.WriteLine("FAIL " + title + ": expected " + expected + ", got " + result.Code); failed++;
    }
}
CheckRaw("null request denied", MediaRightsPolicy.Evaluate(activeAsset, new[] { goodGrant }, null), DecisionCode.InvalidRequest);
CheckRaw("null asset denied", MediaRightsPolicy.Evaluate(null, new[] { goodGrant },
    new PermissionRequest(assetId, MediaAction.Stream, "NG", asOf)), DecisionCode.AssetNotFound);
CheckRaw("null grant set denied", MediaRightsPolicy.Evaluate(activeAsset, null,
    new PermissionRequest(assetId, MediaAction.Stream, "NG", asOf)), DecisionCode.NoMatchingGrant);
var matched = MediaRightsPolicy.Evaluate(activeAsset, new[] { goodGrant },
    new PermissionRequest(assetId, MediaAction.Stream, "NG", asOf));
if (matched.Allowed && matched.MatchedGrantId == grantId && matched.Code == DecisionCode.Allowed)
{
    Console.WriteLine("PASS authorizing grant provenance"); passed++;
}
else { Console.Error.WriteLine("FAIL authorizing grant provenance"); failed++; }
var denied = MediaRightsPolicy.Evaluate(activeAsset, Array.Empty<LicenseGrant>(),
    new PermissionRequest(assetId, MediaAction.Stream, "NG", asOf));
if (!denied.Allowed && denied.MatchedGrantId is null)
{
    Console.WriteLine("PASS denied decisions have no matched grant"); passed++;
}
else { Console.Error.WriteLine("FAIL denied decisions have no matched grant"); failed++; }
Console.WriteLine(passed + " passed, " + failed + " failed");
return failed == 0 ? 0 : 1;
