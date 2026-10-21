using OpenMediaRights;

// This example is compiled against the generated NuGet package, not the source project.
var assetId = Guid.NewGuid();
var grantId = Guid.NewGuid();
var now = new DateTimeOffset(2026, 10, 21, 10, 0, 0, TimeSpan.Zero);
var asset = new MediaAsset(assetId, AssetLifecycle.Active, RightsStanding.Verified,
    MediaRights.Stream | MediaRights.Download | MediaRights.Share);
var grants = new[]
{
    new LicenseGrant(grantId, assetId, "NG", GrantVerification.Verified,
        MediaRights.Stream, now.AddDays(-1), now.AddDays(1))
};

var outcomes = new (string Name, PermissionRequest Request, DecisionCode Expected)[]
{
    ("Nigeria streaming allowed", new(assetId, MediaAction.Stream, "NG", now), DecisionCode.Allowed),
    ("Download denied without licence grant", new(assetId, MediaAction.Download, "NG", now), DecisionCode.ActionNotGranted),
    ("Ghana streaming denied by territory", new(assetId, MediaAction.Stream, "GH", now), DecisionCode.NoMatchingGrant),
    ("Expired stream licence denied", new(assetId, MediaAction.Stream, "NG", now.AddDays(2)), DecisionCode.GrantNotInEffect),
    ("Sharing denied without licence grant", new(assetId, MediaAction.Share, "NG", now), DecisionCode.ActionNotGranted)
};

var failures = 0;
foreach (var scenario in outcomes)
{
    var decision = MediaRightsPolicy.Evaluate(asset, grants, scenario.Request);
    if (decision.Code != scenario.Expected || decision.Allowed != (scenario.Expected == DecisionCode.Allowed))
    {
        Console.Error.WriteLine($"FAIL {scenario.Name}: {decision.Code}");
        failures++;
    }
    else
    {
        Console.WriteLine($"PASS {scenario.Name}");
    }
}

var allow = MediaRightsPolicy.Evaluate(asset, grants, outcomes[0].Request);
if (allow.MatchedGrantId != grantId)
{
    Console.Error.WriteLine("FAIL authorizing grant provenance");
    failures++;
}
else
{
    Console.WriteLine("PASS authorizing grant provenance");
}

Console.WriteLine($"Packaged consumer: {6 - failures} passed, {failures} failed");
return failures == 0 ? 0 : 1;
