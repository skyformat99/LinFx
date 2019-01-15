﻿using LinFx;
using System.Linq;
using System.Security.Claims;
using ClaimTypes = LinFx.Security.Claims.ClaimTypes;

namespace System.Security.Principal
{
    public static class ClaimsIdentityExtensions
    {
        public static Guid? FindUserId([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.UserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(userIdOrNull.Value);
        }

        public static Guid? FindTenantId([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var tenantIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.TenantId);
            if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(tenantIdOrNull.Value);
        }

        public static Guid? FindUserId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var userIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.UserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(userIdOrNull.Value);
        }

        public static Guid? FindTenantId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var tenantIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.TenantId);
            if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(tenantIdOrNull.Value);
        }
    }
}
