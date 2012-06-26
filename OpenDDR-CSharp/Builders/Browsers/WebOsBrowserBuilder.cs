using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Oddr.Models.Browsers;
using Oddr.Models;

namespace Oddr.Builders.Browsers
{
    class WebOsBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String VERSION_REGEXP = ".*webOSBrowser/([0-9\\.]+).*?";
        private static Regex versionRegex = new Regex(VERSION_REGEXP);


        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            String version = null;

            if (!versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                return null;

            }
            else
            {
                Match versionMatcher = versionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = versionMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    version = groups[1].Value;
                }
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("HP");
            identified.SetModel("Web OS Browser");
            identified.SetVersion(version);
            String[] versionEl = version.Split(".".ToCharArray());

            if (versionEl.Length > 0)
            {
                identified.majorRevision = versionEl[0];
            }

            if (versionEl.Length > 1)
            {
                identified.microRevision = versionEl[1];
                confidence += 10;
            }

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
            }

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return userAgent.completeUserAgent.Contains("webOSBrowser");
        }
    }
}
