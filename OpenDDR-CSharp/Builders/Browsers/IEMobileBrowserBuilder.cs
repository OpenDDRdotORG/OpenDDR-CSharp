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
using System.Text.RegularExpressions;
using Oddr.Models;
using Oddr.Models.Browsers;

namespace Oddr.Builders.Browsers
{
    class IEMobileBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const string VERSION_REGEXP = ".*[^MS]IEMobile.([0-9\\.]+).*?";
        private const string MSIE_VERSION_REGEXP = ".*MSIE.([0-9\\.]+).*";
        private const string MSIEMOBILE_VERSION_REGEXP = ".*MSIEMobile.([0-9\\.]+).*";
        private Regex versionRegex = new Regex(VERSION_REGEXP, RegexOptions.Compiled);
        private Regex msieVersionRegex = new Regex(MSIE_VERSION_REGEXP, RegexOptions.Compiled);
        private Regex msieMobileVersionRegex = new Regex(MSIEMOBILE_VERSION_REGEXP, RegexOptions.Compiled);

        private const string WINDOWS_CE_PHONE_REGEXP = ".*Windows.?(?:(?:CE)|(?:Phone)).*";
        private Regex windowsCePhoneRegex = new Regex(WINDOWS_CE_PHONE_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!userAgent.containsWindowsPhone || !(windowsCePhoneRegex.IsMatch(userAgent.completeUserAgent)))
            {
                return null;
            }

            int confidence = 40;
            Browser identified = new Browser();

            identified.SetVendor("Microsoft");
            identified.SetModel("IEMobile");

            if (userAgent.completeUserAgent.Contains("MSIEMobile"))
            {
                confidence += 10;
            }

            if (userAgent.mozillaPattern)
            {
                confidence += 10;
            }

            if (versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match versionMatcher = versionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = versionMatcher.Groups;

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
            else
            {
                //fallback version
                identified.SetVersion("1.0");
                identified.majorRevision = "1";
            }

            if (msieVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match msieMatcher = msieVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = msieMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetReferenceBrowser("MSIE");
                    identified.SetReferenceBrowserVersion(groups[1].Value);
                    confidence += 10;
                }
            }

            if (msieMobileVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match msieMobileMatcher = msieMobileVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = msieMobileMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetLayoutEngine("MSIEMobile");
                    identified.SetLayoutEngineVersion(groups[1].Value);
                    confidence += 10;
                }
            }

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.TRIDENT))
                {
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
            return userAgent.containsWindowsPhone;
        }
    }
}
