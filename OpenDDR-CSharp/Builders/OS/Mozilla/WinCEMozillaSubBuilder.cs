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
using Oddr.Models;
using System.Text.RegularExpressions;
using OSModel = Oddr.Models.OS;

namespace Oddr.Builders.OS.Mozilla
{
    public class WinCEMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = ".*Windows.?CE.?((\\d+)?\\.?(\\d+)?\\.?(\\d+)?).*";
        private const String VERSION_MSIE_IEMOBILE = "(?:.*(?:MSIE).?(\\d+)\\.(\\d+).*)|(?:.*IEMobile.?(\\d+)\\.(\\d+).*)";
        private Regex versionRegex = new Regex(VERSION_REGEXP);
        private Regex versionMsieRegex = new Regex(VERSION_MSIE_IEMOBILE);

        public bool CanBuild(UserAgent userAgent)
        {
            if (userAgent.containsWindowsPhone)
            {
                Regex winCeRegex = new Regex(".*Windows.?CE.*");
                if (winCeRegex.IsMatch(userAgent.GetPatternElementsInside()))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();
            model.majorRevision = "1";
            model.SetVendor("Microsoft");
            model.SetModel("Windows Phone");
            model.confidence = 40;

            string patternElementsInside = userAgent.GetPatternElementsInside();
            String[] splittedTokens = patternElementsInside.Split(";".ToCharArray());
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    if (model.confidence > 40)
                    {
                        model.confidence = 95;

                    }
                    else
                    {
                        model.confidence = 85;
                    }

                    if (groups[1] != null)
                    {
                        model.SetDescription(groups[1].Value);
                    }
                    if (groups[2] != null)
                    {
                        model.majorRevision = groups[2].Value;
                    }
                    if (groups[3] != null)
                    {
                        model.minorRevision = groups[3].Value;
                    }
                    if (groups[4] != null)
                    {
                        model.microRevision = groups[4].Value;
                    }
                }

                if (versionMsieRegex.IsMatch(tokenElement))
                {
                    Match versionMsieMatcher = versionMsieRegex.Match(tokenElement);
                    String version = model.GetVersion();
                    if (version == null || version.Length < 7)
                    {
                        version = "0.0.0.0";
                    }
                    String[] subVersion = version.Split(".".ToCharArray());
                    int count = 0;
                    GroupCollection groups = versionMsieMatcher.Groups;
                    for (int idx = 1; idx <= groups.Count; idx++)
                    {
                        if ((idx >= 1) && (idx <= 4) && groups[idx] != null)
                        {
                            subVersion[idx - 1] = groups[idx].Value;
                            count++;
                        }
                    }
                    model.SetVersion(subVersion[0] + "." + subVersion[1] + "." + subVersion[2] + "." + subVersion[3]);

                    if (model.confidence > 40)
                    {
                        model.confidence = 95;

                    }
                    else
                    {
                        model.confidence = (count * 18);
                    }
                }
            }
            SetWinCeVersion(model);
            return model;
        }

        private void SetWinCeVersion(OSModel.OperatingSystem model)
        {
            //TODO: to be refined
            String version = model.GetVersion();
            if (version == null)
            {
                return;

            }
            else if (!model.majorRevision.Equals("1"))
            {
                return;
            }

            Regex winCEVersionRegex = new Regex(".*(\\d+).(\\d+).(\\d+).(\\d+).*");

            if (winCEVersionRegex.IsMatch(version))
            {
                Match result = winCEVersionRegex.Match(version);
                GroupCollection groups = result.Groups;

                if (groups[1].Value.Equals("4"))
                {
                    model.majorRevision = "5";

                }
                else if (groups[1].Value.Equals("6"))
                {
                    model.majorRevision = "6";

                    if (groups[3].Equals("7"))
                    {
                        model.minorRevision = "1";
                    }
                }
            }
        }
    }
}
