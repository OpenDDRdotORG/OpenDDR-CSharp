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

namespace Oddr.Builders.OS
{
    public class BlackBerryOSBuilder : IBuilder
    {
        private const String VERSION_REGEXP = "(?:.*?[Bb]lack.?[Bb]erry(?:\\d+)/((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*)";
        private Regex versionRegex = new Regex(VERSION_REGEXP);

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsBlackBerryOrRim;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            OSModel.OperatingSystem model = new OSModel.OperatingSystem();

            model.SetVendor("Research In Motion");
            model.SetModel("Black Berry OS");
            model.majorRevision = "1";

            if (versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match versionMatcher = versionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = versionMatcher.Groups;

                if (groups[1] != null)
                {
                    model.confidence = 50;
                    model.SetVersion(groups[1].Value);

                    if (groups[2] != null)
                    {
                        model.majorRevision = groups[2].Value;
                        model.confidence = 60;
                    }

                    if (groups[3] != null)
                    {
                        model.minorRevision = groups[3].Value;
                        model.confidence = 70;
                    }

                    if (groups[4] != null)
                    {
                        model.microRevision = groups[4].Value;
                        model.confidence = 80;
                    }

                    if (groups[5] != null)
                    {
                        model.nanoRevision = groups[5].Value;
                        model.confidence = 90;
                    }
                }
            }
            return model;
        }
    }
}
