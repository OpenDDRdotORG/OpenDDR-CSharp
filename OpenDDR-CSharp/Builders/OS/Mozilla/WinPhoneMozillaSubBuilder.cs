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
    public class WinPhoneMozillaSubBuilder : IBuilder
    {
        private const String VERSION_REGEXP = ".*Windows.?Phone.?(?:OS)?.?((\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?(?:\\.(\\d+))?).*";
        private Regex versionRegex = new Regex(VERSION_REGEXP);

        public bool CanBuild(UserAgent userAgent)
        {
            if (userAgent.containsWindowsPhone)
            {
                Regex windowsPhoneRegex = new Regex(".*Windows.?Phone.*");
                if (windowsPhoneRegex.IsMatch(userAgent.GetPatternElementsInside() + userAgent.GetPatternElementsPost()))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();
            model.SetVendor("Microsoft");
            model.SetModel("Windows Phone");
            model.confidence = 40;
            bool isInPostMoz = false;

            String toSplit = userAgent.GetPatternElementsInside();

            if (!versionRegex.IsMatch(toSplit))
            {
                toSplit = userAgent.GetPatternElementsPost();
                isInPostMoz = true;
            }

            String[] splittedTokens = toSplit.Split(";".ToCharArray());
            foreach (String tokenElement in splittedTokens)
            {
                if (versionRegex.IsMatch(tokenElement))
                {
                    Match versionMatcher = versionRegex.Match(tokenElement);
                    GroupCollection groups = versionMatcher.Groups;

                    if (isInPostMoz)
                    {
                        model.confidence = 85;

                    }
                    else
                    {
                        model.confidence = 90;
                    }

                    if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                    {
                        model.SetVersion(groups[1].Value);
                    }

                    if (groups[2] != null && groups[2].Value.Trim().Length > 0)
                    {
                        model.majorRevision = groups[2].Value;
                    }

                    if (groups[3] != null && groups[3].Value.Trim().Length > 0)
                    {
                        model.minorRevision = groups[3].Value;
                    }

                    if (groups[4] != null && groups[4].Value.Trim().Length > 0)
                    {
                        model.microRevision = groups[4].Value;
                    }

                    if (groups[5] != null && groups[5].Value.Trim().Length > 0)
                    {
                        model.nanoRevision = groups[5].Value;
                    }
                }

                if (model.majorRevision.Equals("0") && model.minorRevision.Equals("0"))
                {
                    model.majorRevision = "6";
                    model.minorRevision = "5";
                }
                model.SetDescription("Windows Phone " + model.majorRevision + "." + model.minorRevision + "." + model.microRevision + "." + model.nanoRevision);
            }
            return model;
        }
    }
}
