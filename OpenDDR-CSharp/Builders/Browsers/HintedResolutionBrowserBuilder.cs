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
using Oddr.Models.Browsers;

namespace Oddr.Builders.Browsers
{
    public abstract class HintedResolutionBrowserBuilder : IBuilder
    {
        private const String RESOLUTION_HINT_WXH_REGEXP = ".*([0-9][0-9][0-9]+)[*Xx]([0-9][0-9][0-9]+).*";
        private const String RESOLUTION_HINT_FWVGA_REGEXP = ".*FWVGA.*";
        private const String RESOLUTION_HINT_WVGA_REGEXP = ".*WVGA.*";
        private const String RESOLUTION_HINT_WXGA_REGEXP = ".*WXGA.*";
        private const String RESOLUTION_HINT_WQVGA_REGEXP = ".*WQVGA.*";
        private Regex resolutionHintWxHRegex = new Regex(RESOLUTION_HINT_WXH_REGEXP, RegexOptions.Compiled);
        private Regex resolutionHintFWVGARegex = new Regex(RESOLUTION_HINT_FWVGA_REGEXP, RegexOptions.Compiled);
        private Regex resolutionHintWVGARegex = new Regex(RESOLUTION_HINT_WVGA_REGEXP, RegexOptions.Compiled);
        private Regex resolutionHintWXGARegex = new Regex(RESOLUTION_HINT_WXGA_REGEXP, RegexOptions.Compiled);
        private Regex resolutionHintWQVGARegex = new Regex(RESOLUTION_HINT_WQVGA_REGEXP, RegexOptions.Compiled);


        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            int hintedWidth = -1;
            int hintedHeight = -1;

            if (resolutionHintWxHRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match match = resolutionHintWxHRegex.Match(userAgent.completeUserAgent);
                GroupCollection groups = match.Groups;
                int.TryParse(groups[0].Value, out hintedWidth);
                int.TryParse(groups[1].Value, out hintedHeight);
            }
            else if (userAgent.completeUserAgent.Contains("VGA") || userAgent.completeUserAgent.Contains("WXGA"))
            {
                if (resolutionHintFWVGARegex.IsMatch(userAgent.completeUserAgent))
                {
                    hintedWidth = 480;
                    hintedHeight = 854;
                }
                else if (resolutionHintWVGARegex.IsMatch(userAgent.completeUserAgent))
                {
                    hintedWidth = 480;
                    hintedHeight = 800;
                }
                else if (resolutionHintWXGARegex.IsMatch(userAgent.completeUserAgent))
                {
                    hintedWidth = 768;
                    hintedHeight = 1280;
                }
                else if (resolutionHintWQVGARegex.IsMatch(userAgent.completeUserAgent))
                {
                    hintedWidth = 240;
                    hintedHeight = 400;
                }
            }

            return BuildBrowser(userAgent, hintedWidth, hintedHeight);
        }

        protected abstract Browser BuildBrowser(UserAgent userAgent, int hintedWidth, int hintedHeight);

        public abstract bool CanBuild(UserAgent userAgent);
    }
}
