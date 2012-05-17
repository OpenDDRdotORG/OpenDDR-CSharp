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
using OSModel = Oddr.Models.OS;
using Oddr.Models;

namespace Oddr.Builders.OS.Mozilla
{
    public class SymbianMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = ".*Series.?60/(\\d+)(?:[\\.\\- ](\\d+))?(?:[\\.\\- ](\\d+))?.*";
        private const String VERSION_EXTRA = ".*Symbian(?:OS)?/(.*)";
        private Regex versionRegex = new Regex(VERSION_REGEXP);
        private Regex versionExtraRegex = new Regex(VERSION_EXTRA);

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsSymbian;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();
            model.majorRevision = "1";
            model.SetVendor("Nokia");
            model.SetModel("Symbian OS");
            model.confidence = 40;

            string patternElementsInside = userAgent.GetPatternElementsInside();
            String[] splittedTokens = patternElementsInside.Split(';');
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    model.SetDescription("Series60");
                    if (model.confidence > 40)
                    {
                        model.confidence = 100;

                    }
                    else
                    {
                        model.confidence = 90;
                    }

                    if (groups[1] != null)
                    {
                        model.majorRevision = groups[1].Value;
                    }
                    if (groups[2] != null)
                    {
                        model.minorRevision = groups[2].Value;
                    }
                    if (groups[3] != null)
                    {
                        model.microRevision = groups[3].Value;
                    }
                }

                if (versionExtraRegex.IsMatch(tokenElement))
                {
                    Match versionExtraMatcher = versionExtraRegex.Match(tokenElement);
                    GroupCollection groups = versionExtraMatcher.Groups;

                    if (model.confidence > 40)
                    {
                        model.confidence = 100;

                    }
                    else
                    {
                        model.confidence = 85;
                    }

                    if (groups[1] != null)
                    {
                        string groupValueTrimmed = groups[1].Value.Trim();
                        model.SetVersion(groupValueTrimmed);
                    }
                }
                //TODO: inference VERSION_EXTRA/VERSION_REGEXP and vice-versa
            }
            return model;
        }
    }
}
