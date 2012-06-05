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
    public class OperaBrowserBuilder : LayoutEngineBrowserBuilder
    {
        private const String OPERAMINI_VERSION_REGEXP = "Opera Mobi/(.*)";
        private Regex operaMiniVersionRegex = new Regex(OPERAMINI_VERSION_REGEXP, RegexOptions.Compiled);

        protected override Browser BuildBrowser(UserAgent userAgent, string layoutEngine, string layoutEngineVersion, int hintedWidth, int hintedHeight)
        {
            if (!userAgent.operaPattern || userAgent.operaVersion == null || userAgent.operaVersion.Length == 0)
            {
                return null;
            }

            int confidence = 60;
            Browser identified = new Browser();

            identified.SetVendor("Opera");
            if (userAgent.completeUserAgent.Contains("Mobi"))
            {
                identified.SetModel("Opera Mobile");
                confidence += 10;

            }
            else if (userAgent.completeUserAgent.Contains("Tablet"))
            {
                identified.SetModel("Opera Tablet");

            }
            else
            {
                identified.SetModel("Opera");
            }

            identified.SetVersion(userAgent.operaVersion);
            String[] version = userAgent.operaVersion.Split(".".ToCharArray());

            if (version.Length > 0)
            {
                identified.majorRevision = version[0];
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

            if (layoutEngine != null)
            {
                identified.SetLayoutEngine(layoutEngine);
                identified.SetLayoutEngineVersion(layoutEngineVersion);
                if (layoutEngine.Equals(LayoutEngineBrowserBuilder.PRESTO))
                {
                    confidence += 10;
                }
            }

            String[] inside = userAgent.GetPatternElementsInside().Split(";".ToCharArray());
            foreach (String token in inside)
            {
                String element = token.Trim();

                if (operaMiniVersionRegex.IsMatch(element))
                {
                    Match miniMatcher = operaMiniVersionRegex.Match(element);
                    GroupCollection groups = miniMatcher.Groups;

                    if (groups[1] != null)
                    {
                        identified.SetReferenceBrowser("Opera Mobi");
                        identified.SetReferenceBrowserVersion(groups[1].Value);
                        confidence += 10;
                        break;
                    }
                }
            }

            identified.SetDisplayWidth(hintedWidth);
            identified.SetDisplayHeight(hintedHeight);
            identified.confidence = confidence;

            return identified;
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return (userAgent.operaPattern && (!userAgent.completeUserAgent.Contains("Opera Mini")));
        }
    }
}
