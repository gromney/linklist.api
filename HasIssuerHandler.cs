using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace link_list.api
{
    public class HasIssuerHandler : AuthorizationHandler<HasIssuerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasIssuerRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Issuer == requirement.Issuer))
            {
                return Task.CompletedTask;
            }
    
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
    public class HasIssuerRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; private set; }
        public HasIssuerRequirement(string issuer)
        {
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }

    }
}