using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace UserApi.Authorization
{
    public static class ScopeAuthorizationRequirementExtensions
    {
        public static AuthorizationPolicyBuilder RequirePermission(
            this AuthorizationPolicyBuilder authorizationPolicyBuilder,
            params string[] requiredScopes)
        {
            authorizationPolicyBuilder.RequirePermission((IEnumerable<string>) requiredScopes);
            return authorizationPolicyBuilder;
        }
 
        public static AuthorizationPolicyBuilder RequirePermission(
            this AuthorizationPolicyBuilder authorizationPolicyBuilder,
            IEnumerable<string> requiredScopes)
        {
            authorizationPolicyBuilder.AddRequirements(new PermissionsAuthorizationRequirement(requiredScopes));
            return authorizationPolicyBuilder;
        }
    }
}