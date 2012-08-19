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
using System.Runtime.CompilerServices;
using Oddr.Models.Browsers;

namespace Oddr.Builders.Browsers
{
    public class DefaultBrowserBuilder : IBuilder
    {
        private IBuilder[] builders;
        private static volatile DefaultBrowserBuilder instance;
        private static object syncRoot = new Object();

        private DefaultBrowserBuilder()
        {
            builders = new IBuilder[]
            {
                new OperaMiniBrowserBuilder(),
                new ChromeMobileBrowserBuilder(),
                new FennecBrowserBuilder(),
                new SafariMobileBrowserBuilder(),
                new SilkBrowserBuilder(),
                new AndroidMobileBrowserBuilder(),
                new NetFrontBrowserBuilder(),
                new UPBrowserBuilder(),
                new OpenWaveBrowserBuilder(),
                new SEMCBrowserBuilder(),
                new DolfinBrowserBuilder(),
                new JasmineBrowserBuilder(),
                new PolarisBrowserBuilder(),
                new ObigoBrowserBuilder(),
                new OperaBrowserBuilder(),
                new IEMobileBrowserBuilder(),
                new NokiaBrowserBuilder(),
                new BlackBerryBrowserBuilder(),
                new WebOsBrowserBuilder(),
                new InternetExplorerBrowserBuilder(),
                new ChromeBrowserBuilder(),
                new FirefoxBrowserBuilder(),
                new SafariBrowserBuilder(),
                new KonquerorBrowserBuilder(),
            };
        }

        public static DefaultBrowserBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new DefaultBrowserBuilder();
                        }
                    }
                }

                return instance;
            }
        }

        public bool CanBuild(UserAgent userAgent)
        {
            foreach (IBuilder browserBuilder in builders)
            {
                if (browserBuilder.CanBuild(userAgent))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<Browser> founds = new List<Browser>();
            Browser found = null;
            foreach (IBuilder builder in builders)
            {
                if (builder.CanBuild(userAgent))
                {
                    Browser builded = (Browser)builder.Build(userAgent, confidenceTreshold);
                    if (builded != null)
                    {
                        founds.Add(builded);
                        if (builded.confidence >= confidenceTreshold)
                        {
                            found = builded;
                            break;
                        }
                    }
                }
            }

            if (found != null)
            {
                return found;

            }
            else
            {
                if (founds.Count == 0)
                {
                    return null;
                }

                founds.Sort();
                founds.Reverse();

                return founds[0];
            }
        }
    }
}
