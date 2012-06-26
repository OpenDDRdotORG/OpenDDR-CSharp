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
using Oddr.Models.Browsers;
using Oddr.Models;

namespace Oddr.Builders.Browsers
{
    public abstract class LayoutEngineBrowserBuilder : HintedResolutionBrowserBuilder
    {
        protected const String APPLEWEBKIT = "AppleWebKit";
        protected const String PRESTO = "Presto";
        protected const String GECKO = "Gecko";
        protected const String TRIDENT = "Trident";
        protected const String KHML = "KHTML";
        private const String WEBKIT_VERSION_REGEXP = ".*AppleWebKit/([0-9\\.]+).*?";
        private const String PRESTO_VERSION_REGEXP = ".*Presto/([0-9\\.]+).*?";
        private const String GECKO_VERSION_REGEXP = ".*Gecko/([0-9\\.]+).*?";
        private const String TRIDENT_VERSION_REGEXP = ".*Trident/([0-9\\.]+).*?";
        private const String KHTML_VERSION_REGEXP = ".*KHTML/([0-9\\.]+).*?";
        private static Regex webkitVersionRegex = new Regex(WEBKIT_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex prestoVersionRegex = new Regex(PRESTO_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex geckoVersionRegex = new Regex(GECKO_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex tridentVersionRegex = new Regex(TRIDENT_VERSION_REGEXP, RegexOptions.Compiled);
        private static Regex khtmlVersionRegex = new Regex(KHTML_VERSION_REGEXP, RegexOptions.Compiled);


        protected override Browser BuildBrowser(UserAgent userAgent, int hintedWidth, int hintedHeight)
        {
            String layoutEngine = null;
            String layoutEngineVersion = null;

            Match match = null;

            if (webkitVersionRegex.IsMatch(userAgent.completeUserAgent))
            {
                match = webkitVersionRegex.Match(userAgent.completeUserAgent);
                layoutEngine = APPLEWEBKIT;
                GroupCollection groups = match.Groups;
                layoutEngineVersion = groups[1].Value;

            }
            else
            {

                if (prestoVersionRegex.IsMatch(userAgent.completeUserAgent))
                {
                    match = prestoVersionRegex.Match(userAgent.completeUserAgent);
                    layoutEngine = "Presto";
                    GroupCollection groups = match.Groups;
                    layoutEngineVersion = groups[1].Value;

                }
                else
                {
                    if (geckoVersionRegex.IsMatch(userAgent.completeUserAgent))
                    {
                        match = geckoVersionRegex.Match(userAgent.completeUserAgent);
                        layoutEngine = "Gecko";
                        GroupCollection groups = match.Groups;
                        layoutEngineVersion = groups[1].Value;

                    }
                    else
                    {
                        if (tridentVersionRegex.IsMatch(userAgent.completeUserAgent))
                        {
                            match = tridentVersionRegex.Match(userAgent.completeUserAgent);
                            layoutEngine = "Trident";
                            GroupCollection groups = match.Groups;
                            layoutEngineVersion = groups[1].Value;

                        }
                        else
                        {
                            if (khtmlVersionRegex.IsMatch(userAgent.completeUserAgent))
                            {
                                match = khtmlVersionRegex.Match(userAgent.completeUserAgent);
                                layoutEngine = "KHTML";
                                GroupCollection groups = match.Groups;
                                layoutEngineVersion = groups[1].Value;
                            }
                        }
                    }
                }
            }
            return BuildBrowser(userAgent, layoutEngine, layoutEngineVersion, hintedWidth, hintedHeight);
        }

        protected abstract Browser BuildBrowser(UserAgent userAgent, String layoutEngine, String layoutEngineVersion, int hintedWidth, int hintedHeight);
    }
}
