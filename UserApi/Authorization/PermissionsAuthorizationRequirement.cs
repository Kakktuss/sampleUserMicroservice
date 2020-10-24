using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace UserApi.Authorization
{
    public class PermissionsAuthorizationRequirement : AuthorizationHandler<PermissionsAuthorizationRequirement>, IAuthorizationRequirement
    {
        public IEnumerable<string> RequiredPermissions { get; }

        public PermissionsAuthorizationRequirement(IEnumerable<string> requiredPermissions)
        {
            if (requiredPermissions == null || !requiredPermissions.Any())
            {
                throw new ArgumentException($"{nameof(requiredPermissions)} must contain at least one value.", nameof(requiredPermissions));
            }
 
            RequiredPermissions = requiredPermissions;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsAuthorizationRequirement requirement)
        {
            if (context.User != null)
            {
                var permissionsClaim = context.User.Claims.Where(c =>
                    string.Equals(c.Type, "permissions", StringComparison.OrdinalIgnoreCase));

                if (permissionsClaim.Any())
                {
                    var permissions = permissionsClaim.Select(e => e.Value);

                    if (requirement.RequiredPermissions.All(requiredPermission => permissions.Contains(requiredPermission)))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}