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
    public class BlackBerryMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = "(?:.*?Version.?((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*)|(?:.*?[Bb]lack.?[Bb]erry(?:\\d+)/((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*)|(?:.*?RIM.?Tablet.?OS.?((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*)";
        private Regex versionRegex = new Regex(VERSION_REGEXP);

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsBlackBerryOrRim;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();

            String rebuilded = userAgent.GetPatternElementsInside() + ";" + userAgent.GetPatternElementsPost();

            String[] splittedTokens = rebuilded.Split(";".ToCharArray());
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    if (groups[11].Value != null && groups[11].Value.Trim().Length > 0)
                    {
                        model.SetVendor("Research In Motion");
                        model.SetModel("RIM Tablet OS");
                        model.majorRevision = "1";
                        model.confidence = 50;

                        if (groups[11].Value != null && groups[11].Value.Trim().Length > 0)
                        {
                            model.SetVersion(groups[11].Value);

                        }

                        if (groups[12].Value != null && groups[12].Value.Trim().Length > 0)
                        {
                            model.majorRevision = groups[12].Value;
                            model.confidence = 60;

                        }

                        if (groups[13].Value != null && groups[13].Value.Trim().Length > 0)
                        {
                            model.minorRevision = groups[13].Value;
                            model.confidence = 70;

                        }

                        if (groups[14].Value != null && groups[14].Value.Trim().Length > 0)
                        {
                            model.microRevision = groups[14].Value;
                            model.confidence = 80;

                        }

                        if (groups[15].Value != null && groups[15].Value.Trim().Length > 0)
                        {
                            model.nanoRevision = groups[15].Value;
                            model.confidence = 90;

                        }
                        return model;

                    }
                    else if ((groups[1] != null && groups[1].Value.Trim().Length > 0) || (groups[6] != null && groups[6].Value.Trim().Length > 0))
                    {
                        model.SetVendor("Research In Motion");
                        model.SetModel("Black Berry OS");
                        model.majorRevision = "1";
                        model.confidence = 40;

                        if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                        {
                            if (groups[6] != null && groups[6].Value.Trim().Length > 0)
                            {
                                model.confidence = 100;

                            }
                            else
                            {
                                model.confidence = 80;
                            }

                        }
                        else if (groups[6] != null && groups[6].Value.Trim().Length > 0)
                        {
                            model.confidence = 90;
                        }

                        if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                        {
                            model.SetVersion(groups[1].Value);

                        }
                        else if (groups[6] != null && groups[6].Value.Trim().Length > 0)
                        {
                            model.SetVersion(groups[6].Value);
                        }

                        if (groups[2] != null && groups[2].Value.Trim().Length > 0)
                        {
                            model.majorRevision = groups[2].Value;

                        }
                        else if (groups[7] != null && groups[7].Value.Trim().Length > 0)
                        {
                            model.majorRevision = groups[7].Value;
                        }

                        if (groups[3] != null && groups[3].Value.Trim().Length > 0)
                        {
                            model.minorRevision = groups[3].Value;

                        }
                        else if (groups[8] != null && groups[8].Value.Trim().Length > 0)
                        {
                            model.minorRevision = groups[8].Value;
                        }

                        if (groups[4] != null && groups[4].Value.Trim().Length > 0)
                        {
                            model.microRevision = groups[4].Value;

                        }
                        else if (groups[9] != null && groups[9].Value.Trim().Length > 0)
                        {
                            model.microRevision = groups[9].Value;
                        }

                        if (groups[5] != null && groups[5].Value.Trim().Length > 0)
                        {
                            model.nanoRevision = groups[5].Value;

                        }
                        else if (groups[10] != null && groups[10].Value.Trim().Length > 0)
                        {
                            model.nanoRevision = groups[10].Value;
                        }
                        return model;

                    }
                }
            }
            return model;
        }
    }
}
