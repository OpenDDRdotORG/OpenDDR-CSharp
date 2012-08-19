﻿/**
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
using Oddr.Models.Browsers;
using Oddr.Models;

namespace Oddr.Builders.Browsers
{
    public class FennecBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String FENNEC_VERSION_REGEXP = ".*Fennec/([0-9a-z\\.\\-]+)";
        private const String FIREFOX_VERSION_REGEXP = ".*Firefox.([0-9a-z\\.b]+).*";
        private static Regex fennecVersionRegex = new Regex(FENNEC_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex firefoxVersionRegex = new Regex(FIREFOX_VERSION_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!(userAgent.mozillaPattern))
            {
                return null;
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Mozilla");
            identified.SetModel("Firefox Mobile");
            identified.SetVersion("-");
            identified.majorRevision = "-";

            if (fennecVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match fennecMatcher = fennecVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = fennecMatcher.Groups;
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
            else if (firefoxVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match firefoxMatcher = firefoxVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = firefoxMatcher.Groups;
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

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.GECKO))
                {
                    confidence += 10;
                }
            }

            if (firefoxVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match firefoxMatcher = firefoxVersionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = firefoxMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    identified.SetReferenceBrowser("Firefox");
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
            return (userAgent.completeUserAgent.Contains("Fennec") || (userAgent.completeUserAgent.Contains("Firefox") && userAgent.completeUserAgent.Contains("Mobile")));
        }
    }
}
