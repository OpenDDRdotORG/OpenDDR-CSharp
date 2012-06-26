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
using Oddr.Models.Browsers;
using System.Text.RegularExpressions;

namespace Oddr.Builders.Browsers
{
    public class OpenWaveBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String VERSION_REGEXP = /*"(?i).*openwave[/ ]?([0-9\\.]+).*?"*/".*openwave[/ ]?([0-9\\.]+).*?";
        private static Regex versionRegex = new Regex(VERSION_REGEXP, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const string OPENWAVE_REGEXP = /*"(?i).*openwave.*"*/".*openwave.*";
        private static Regex openwaveRegex = new Regex(OPENWAVE_REGEXP, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            String version = null;

            if (!versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                return null;

            }
            else
            {
                Match versionMatcher = versionRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = versionMatcher.Groups;

                if (groups[1] != null && groups[1].Value.Trim().Length > 0)
                {
                    version = groups[1].Value;
                }
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Openwave");
            identified.SetModel("OpenWave");
            identified.SetVersion(version);

            String[] versionEl = version.Split(".".ToCharArray());

            if (versionEl.Length > 0)
            {
                identified.majorRevision = versionEl[0];
            }

            if (versionEl.Length > 1)
            {
                identified.minorRevision = versionEl[1];
                confidence += 10;
            }

            if (versionEl.Length > 2)
            {
                identified.microRevision = versionEl[2];
            }

            if (versionEl.Length > 3)
            {
                identified.nanoRevision = versionEl[3];
            }

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
            }

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(Models.UserAgent userAgent)
        {
            if (openwaveRegex.IsMatch(userAgent.completeUserAgent))
            {
                return true;
            }
            return false;
        }
    }
}
