# Decision codes

| Code | Meaning |
|---|---|
| Allowed | An applicable verified, active grant permits the action |
| InvalidRequest | Request missing or asset ID empty |
| AssetNotFound | Asset absent or its identifier differs |
| AssetNotActive | Draft or unknown lifecycle |
| AssetSuspended | Suspended |
| AssetRemoved | Removed |
| AssetRightsUnverified | Rights unverified or unknown |
| AssetRightsPending | Rights pending review |
| AssetRightsDisputed | Rights disputed |
| AssetRightsRevoked | Rights revoked |
| OperationDisabled | Operational action flag disabled |
| InvalidCountryCode | Missing or syntactically invalid country code |
| UnknownAction | Unsupported action enum |
| NoMatchingGrant | No applicable asset/country grant |
| GrantNotVerified | Scoped grants lack verification |
| GrantNotInEffect | Verified scoped grants outside validity window |
| ActionNotGranted | Active verified grants do not allow this action |
| EvaluationFailed | Grant enumeration threw; fail closed |

Never use denial codes as fallback authorization. MatchedGrantId is populated only on allow.