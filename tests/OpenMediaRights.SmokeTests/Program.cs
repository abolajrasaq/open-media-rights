using OpenMediaRights;
var id=Guid.NewGuid();
var asset=new MediaAsset(id,AssetLifecycle.Active,RightsStanding.Verified,MediaRights.Stream);
var grant=new LicenseGrant(Guid.NewGuid(),id,"NG",GrantVerification.Verified,MediaRights.Stream);
var scenarios=new (string Name, MediaAsset Asset,LicenseGrant[] Grants, PermissionRequest Request,DecisionCode Expected)[] {
 ("allowed",asset,[grant],new(id,MediaAction.Stream,"NG"),DecisionCode.Allowed),
 ("country mismatch",asset,[grant],new(id,MediaAction.Stream,"GH"),DecisionCode.NoMatchingGrant),
 ("pending",asset with {Standing=RightsStanding.Pending},[grant],new(id,MediaAction.Stream,"NG"),DecisionCode.AssetRightsPending),
 ("disputed",asset with {Standing=RightsStanding.Disputed},[grant],new(id,MediaAction.Stream,"NG"),DecisionCode.AssetRightsDisputed),
 ("suspended",asset with {Lifecycle=AssetLifecycle.Suspended},[grant],new(id,MediaAction.Stream,"NG"),DecisionCode.AssetSuspended),
 ("disabled",asset with {EnabledActions=MediaRights.None},[grant],new(id,MediaAction.Stream,"NG"),DecisionCode.OperationDisabled),
 ("unverified grant",asset,[grant with {Verification=GrantVerification.Pending}],new(id,MediaAction.Stream,"NG"),DecisionCode.GrantNotVerified),
 ("unauthorized download",asset,[grant],new(id,MediaAction.Download,"NG"),DecisionCode.OperationDisabled),
 ("missing grants",asset,[],new(id,MediaAction.Stream,"NG"),DecisionCode.NoMatchingGrant),
 ("worldwide",asset,[grant with {Territory="WORLDWIDE"}],new(id,MediaAction.Stream,"GH"),DecisionCode.Allowed),
 ("expired",asset,[grant with {EndsAt=DateTimeOffset.UtcNow.AddDays(-1)}],new(id,MediaAction.Stream,"NG"),DecisionCode.GrantNotInEffect)
};
var failures=0;
foreach(var c in scenarios) {
 var result=MediaRightsPolicy.Evaluate(c.Asset,c.Grants,c.Request);
 if(result.Code!=c.Expected) { Console.Error.WriteLine($"FAIL {c.Name}: {result.Code}, expected {c.Expected}");failures++; }
 else Console.WriteLine($"PASS {c.Name}");
}
return failures==0?0:1;