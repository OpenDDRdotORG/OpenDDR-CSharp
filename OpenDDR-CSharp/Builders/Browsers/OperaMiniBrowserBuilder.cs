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
    public class OperaMiniBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String VERSION_REGEXP = ".*?Opera Mini/(?:att/)?v?((\\d+)\\.(\\d+)(?:\\.(\\d+))?(?:\\.(\\d+))?).*?";
        private const String BUILD_REGEXP = ".*?Opera Mini/(?:att/)?v?.*?/(.*?);.*";
        private Regex versionRegex = new Regex(VERSION_REGEXP, RegexOptions.Compiled);
        private Regex buildRegex = new Regex(BUILD_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!versionRegex.IsMatch(userAgent.completeUserAgent))
            {
                return null;
            }

            Match versionMatcher = versionRegex.Match(userAgent.completeUserAgent);
            GroupCollection groups = versionMatcher.Groups;

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Opera");
            identified.SetModel("Opera Mini");

            if (groups[1] != null && groups[1].Value.Trim().Length > 0)
            {
                identified.SetVersion(groups[1].Value);
            }
            if (groups[2] != null && groups[2].Value.Trim().Length > 0)
            {
                identified.majorRevision = groups[2].Value;
            }
            if (groups[3] != null && groups[3].Value.Trim().Length > 0)
            {
                identified.minorRevision = groups[3].Value;
            }
            if (groups[4] != null && groups[4].Value.Trim().Length > 0)
            {
                identified.microRevision = groups[4].Value;
            }
            if (groups[5] != null && groups[5].Value.Trim().Length > 0)
            {
                identified.nanoRevision = groups[5].Value;
            }

            if (userAgent.operaPattern && userAgent.operaVersion != null)
            {
                identified.SetReferenceBrowser("Opera");
                identified.SetReferenceBrowserVersion(userAgent.operaVersion);
                confidence += 20;
            }

            if (buildRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match buildMatcher = buildRegex.Match(userAgent.completeUserAgent);
                GroupCollection buildGroups = buildMatcher.Groups;

                if (buildGroups[1] != null && buildGroups[1].Value.Trim().Length > 0)
                {
                    identified.SetBuild(buildGroups[1].Value);
                    confidence += 10;
                }
            }

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.PRESTO))
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
            return userAgent.completeUserAgent.Contains("Opera Mini");
        }
    }
}
