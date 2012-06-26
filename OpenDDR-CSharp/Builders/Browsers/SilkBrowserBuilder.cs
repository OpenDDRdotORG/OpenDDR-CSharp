using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Oddr.Models;
using Oddr.Models.Browsers;

namespace Oddr.Builders.Browsers
{
    public class SilkBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String VERSION_REGEXP = ".*Version/([0-9\\.]+).*?";
        private const String SILK_VERSION_REGEXP = ".*Silk/([0-9a-z\\.\\-]+)";
        private static Regex versionRegex = new Regex(VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex silkVersionRegex = new Regex(SILK_VERSION_REGEXP, RegexOptions.Compiled);

        public override bool CanBuild(UserAgent userAgent)
        {
            return (userAgent.completeUserAgent.Contains("Silk-Accelerated"));
        }

        protected override Browser BuildBrowser(UserAgent userAgent, String layoutEngine, String layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!(userAgent.mozillaPattern))
            {
                return null;
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Amazon");
            identified.SetModel("Silk");
            identified.SetVersion("-");
            identified.majorRevision = "-";

            Match silkMatcher = silkVersionRegex.Match(userAgent.GetPatternElementsInside());
            GroupCollection groups = silkMatcher.Groups;

            if (silkMatcher.Success)
            {
                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetVersion(groups[1].Value);
                    string versionFullString = groups[1].Value;
                    String[] version = versionFullString.Split(".".ToCharArray());

                    if (version.Length > 0)
                    {
                        identified.majorRevision = version[0];
                        if (identified.majorRevision.Length == 0)
                        {
                            identified.majorRevision = "1";
                        }
                    }

                    if (version.Length > 1)
                    {
                        identified.minorRevision = version[1];
                        confidence += 10;
                    }

                    if (version[2] != null)
                    {
                        String[] subVersion = version[2].Split("-".ToCharArray());

                        if (subVersion.Length > 0)
                        {
                            identified.microRevision = subVersion[0];
                        }

                        if (subVersion.Length > 1)
                        {
                            identified.nanoRevision = subVersion[1];
                        }
                    }
                }

            }
            else
            {
                //fallback version
                identified.SetVersion("1.0");
                identified.majorRevision = "1";
            }

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.APPLEWEBKIT))
                {
                    confidence += 10;
                }
            }


            if (userAgent.containsAndroid)
            {
                identified.SetReferenceBrowser("Android Browser");
                Match androidMatcher = versionRegex.Match(userAgent.completeUserAgent);
                GroupCollection androidGroups = androidMatcher.Groups;

                if (androidMatcher.Success)
                {
                    if (androidGroups[1] != null && androidGroups[1].Value.Trim().Length > 0)
                    {
                        identified.SetReferenceBrowserVersion(androidGroups[1].Value);
                        confidence += 5;
                    }
                }
                confidence += 5;
            }
            else if (userAgent.completeUserAgent.Contains("Safari") && !userAgent.completeUserAgent.Contains("Mobile"))
            {
                identified.SetReferenceBrowser("Safari");
                Match safariMatcher = versionRegex.Match(userAgent.completeUserAgent);
                GroupCollection safariGroups = safariMatcher.Groups;

                if (safariMatcher.Success)
                {
                    if (safariGroups[1] != null && safariGroups[1].Value.Trim().Length > 0)
                    {
                        identified.SetReferenceBrowserVersion(safariGroups[1].Value);
                        confidence += 5;
                    }
                }
                confidence += 5;
            }


            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }
    }
}
