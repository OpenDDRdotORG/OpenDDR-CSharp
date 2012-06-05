/**
 * Copyright 2011 OpenDDR LLC
 * This software is distributed under the terms of the GNU Lesser General Public License.
 *
 *
 * This file is part of OpenDDR Simple APIs.
 * OpenDDR Simple APIs is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 *
 * OpenDDR Simple APIs is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with Simple APIs.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oddr.Models.Browsers;
using Oddr.Models;
using System.Text.RegularExpressions;

namespace Oddr.Builders.Browsers
{
    public class BlackBerryBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String BLACKBERRY_VERSION_REGEXP = ".*(?:(?:Version)|(?:[Bb]lack.?[Bb]erry.?(?:[0-9a-z]+)))/([0-9\\.]+).*";//"(?:.*?Version.?([0-9\\.]+).*)|(?:.*?[Bb]lack.?[Bb]erry(?:\\d+)/([0-9\\.]+).*)";
        private const String SAFARI_VERSION_REGEXP = ".*Safari/([0-9\\.]+).*";
        private Regex blackberryVersionRegex = new Regex(BLACKBERRY_VERSION_REGEXP, RegexOptions.Compiled);
        private Regex safariVersionRegex = new Regex(SAFARI_VERSION_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            int confidence = 50;
            Browser identified = new Browser();

            identified.SetVendor("RIM");
            identified.SetModel("BlackBerry");
            identified.SetVersion("-");
            identified.majorRevision = "-";

            if (blackberryVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match blackberryBrowserMatcher = blackberryVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = blackberryBrowserMatcher.Groups;
                if (groups[1] != null)
                {
                    String totalVersion = groups[1].Value;

                    if (totalVersion.Length > 0)
                    {
                        identified.SetVersion(totalVersion);
                        String[] version = totalVersion.Split(".".ToCharArray());

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

                        if (version.Length > 2)
                        {
                            identified.microRevision = version[2];
                        }

                        if (version.Length > 3)
                        {
                            identified.nanoRevision = version[3];
                        }
                    }
                }

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

            if (safariVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match safariMatcher = safariVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = safariMatcher.Groups;

                if (groups[1] != null)
                {
                    identified.SetReferenceBrowser("Safari");
                    identified.SetReferenceBrowserVersion(groups[1].Value);
                    confidence += 10;
                }
            }

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return userAgent.completeUserAgent.Contains("BlackBerry");
        }
    }
}
