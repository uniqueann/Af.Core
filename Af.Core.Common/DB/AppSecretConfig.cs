using Af.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Af.Core.Common
{
    public class AppSecretConfig
    {
        private static string AudienceSecret = Appsettings.app(new string[] { "Audience","Secret" });
        private static string AudienceSecretFile = Appsettings.app(new string[] { "Audience", "SecretFile" });

        public static string AudienceSecretString => InitAudienceSecret();

        private static string InitAudienceSecret()
        {
            var securityString = DifDBConnOfSecurity(AudienceSecretFile);
            if (!string.IsNullOrEmpty(AudienceSecretFile)&& !string.IsNullOrEmpty(securityString))
            {
                return securityString;
            }
            else
            {
                return AudienceSecret;
            }
        }

        private static string DifDBConnOfSecurity(params string[] conn)
        {
            foreach (var item in conn)
            {
                try
                {
                    if (File.Exists(item))
                    {
                        return File.ReadAllText(item).Trim();
                    }

                }
                catch (Exception) { }
            }
            return "";
        }
    }
}
