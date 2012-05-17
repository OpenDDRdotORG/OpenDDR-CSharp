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
using OSModel = Oddr.Models.OS;

namespace Oddr.Builders.OS.Mozilla
{
    public class BrewMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = ".*?(?:(?:Brew|BREW).?(?:MP)?).((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?(.*))";
        private Regex versionRegex = new Regex(VERSION_REGEXP);

        public bool CanBuild(UserAgent userAgent)
        {
            Regex brewRegex = new Regex(/*"(?i).*brew.*"*/".*brew.*", RegexOptions.IgnoreCase);
            return (userAgent.GetPatternElementsInside() != null && brewRegex.IsMatch(userAgent.GetPatternElementsInside()));
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();
            model.majorRevision = "1";
            model.SetVendor("Qualcomm");
            model.SetModel("Brew");
            model.confidence = 40;

            string patternElementInside = userAgent.GetPatternElementsInside();
            String[] splittedTokens = patternElementInside.Split(';');
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    model.confidence = 90;
                    if (groups[1] != null)
                    {
                        model.SetVersion(groups[1].Value);
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
                    if (groups[5] != null)
                    {
                        model.nanoRevision = groups[5].Value;
                    }
                }
            }
            return model;
        }
    }
}
