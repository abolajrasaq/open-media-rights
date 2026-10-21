using OpenMediaRights;

// This is a deliberately local-only, read-only example, NOT a secured streaming API.
// Production systems must authenticate callers, resolve countries from trusted sources,
// and read verified grants from an authoritative datastore.
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5187");
var app = builder.Build();

var assetId = Guid.Parse("11111111-1111-1111-1111-111111111111");
var asset = new MediaAsset(assetId, AssetLifecycle.Active, RightsStanding.Verified,
    MediaRights.Stream | MediaRights.Download);
var licenses = new[]
{
    new LicenseGrant(
        Guid.Parse("22222222-2222-2222-2222-222222222222"),
        assetId, "NG", GrantVerification.Verified, MediaRights.Stream,
        DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30))
};

// Fixed server-owned territory for the demo. URL parameters or headers MUST NOT decide
// geographical licensing permissions.
const string serverResolvedTerritory = "NG";

app.MapGet("/demo/info", () => Results.Ok(new
{
    assetId,
    note = "Local-only mock rights data; not a legal verifier or protected media endpoint."
}));

app.MapGet("/demo/rights/{action}", (string action) =>
{
    if (!Enum.TryParse<MediaAction>(action, ignoreCase: true, out var parsedAction) ||
        !Enum.IsDefined(parsedAction))
    {
        return Results.BadRequest(new { error = "Unknown media action." });
    }

    var decision = MediaRightsPolicy.Evaluate(
        asset, licenses, new PermissionRequest(assetId, parsedAction, serverResolvedTerritory));

    var response = new
    {
        decision.Allowed,
        code = decision.Code.ToString(),
        decision.MatchedGrantId
    };

    return decision.Allowed ? Results.Ok(response) : Results.Json(response, statusCode: 403);
});

app.Run();
