using Af.Core.Common.Converter;
using Af.Core.Common.Helper;
using log4net;
using Microsoft.AspNetCore.Builder;
using System;

namespace Af.Core.Extensions
{
    /// <summary>
    /// ip 限流
    /// </summary>
    public static class IpLimitMidd
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(IpLimitMidd));

        public static void UseIpLimitMidd(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            try
            {
                if (Appsettings.app(new string[] { "Middleware", "IpRateLimit", "Enabled" }).ObjToBool())
                {

                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error occured limiting ip rate.\n{ ex.Message}");
                throw;
            }
        }
    }
}
