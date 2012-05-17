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
using W3c.Ddr.Simple;
using Oddr.Builders;
using log4net;
using Oddr.Models.Browsers;
using OSModel = Oddr.Models.OS;
using log4net.Config;

namespace Oddr.Identificators
{
    public class OSIdentificator : IIdentificator
    {
        protected static readonly ILog logger = LogManager.GetLogger(typeof(OSIdentificator));
        private IBuilder[] builders;
        private Dictionary<String, OSModel.OperatingSystem> operatingSystemCapabilities;

        public OSIdentificator(IBuilder[] builders, Dictionary<String, OSModel.OperatingSystem> operatingSystemCapabilities)
        {
            this.builders = builders;
            this.operatingSystemCapabilities = operatingSystemCapabilities;
        }

        public BuiltObject Get(string userAgent, int confidenceTreshold)
        {
            return Get(UserAgentFactory.newUserAgent(userAgent), confidenceTreshold);
        }

        //XXX: to be refined, this should NOT be the main entry point, we should use a set of evidence derivation
        public BuiltObject Get(IEvidence evdnc, int threshold)
        {
            UserAgent ua = UserAgentFactory.newDeviceUserAgent(evdnc);
            if (ua != null)
            {
                return Get(ua, threshold);
            }
            return null;
        }

        public BuiltObject Get(UserAgent userAgent, int confidenceTreshold)
        {
            foreach (IBuilder builder in builders)
            {
                if (builder.CanBuild(userAgent))
                {
                    OSModel.OperatingSystem os = (OSModel.OperatingSystem)builder.Build(userAgent, confidenceTreshold);
                    if (os != null)
                    {
                        if (operatingSystemCapabilities != null)
                        {
                            String bestID = GetClosestKnownBrowserID(os.GetId());
                            if (bestID != null)
                            {
                                OSModel.OperatingSystem operatingSystem = null;
                                if (operatingSystemCapabilities.TryGetValue(bestID, out operatingSystem))
                                {
                                    os.PutPropertiesMap(operatingSystem.properties);

                                    if (!bestID.Equals(os.GetId()))
                                    {
                                        os.confidence = (os.confidence - 15);
                                    }
                                }
                            }
                        }
                        return os;
                    }
                }
            }
            return null;
        }

        private String GetClosestKnownBrowserID(String actualOperatingSystemID) {
            XmlConfigurator.Configure();

            if (actualOperatingSystemID == null) {
                return null;
            }

            int idx = actualOperatingSystemID.IndexOf(".");

            if (idx < 0) {
                logger.Error("SHOULD NOT BE HERE, PLEASE CHECK BROWSER DOCUMENT(1)");
                logger.Debug(actualOperatingSystemID);
                return null;

            } else {
                idx++;
            }
            idx = actualOperatingSystemID.IndexOf(".", idx);

            if (idx < 0) {
                logger.Error("SHOULD NOT BE HERE, PLEASE CHECK BROWSER DOCUMENT(2)" + idx);
                logger.Debug(actualOperatingSystemID);
                return null;

            } else {
                idx++;
            }

            String bestID = null;
            foreach (String listOperatingSystemID in operatingSystemCapabilities.Keys) {
                if (listOperatingSystemID.Equals(actualOperatingSystemID)) {
                    return actualOperatingSystemID;
                }

                if (listOperatingSystemID.Length > idx && listOperatingSystemID.Substring(0, idx).Equals(actualOperatingSystemID.Substring(0, idx))) {
                    if (listOperatingSystemID.CompareTo(actualOperatingSystemID) <= 0) {
                        bestID = listOperatingSystemID;
                    }
                }
            }

            return bestID;
        }

        public void CompleteInit()
        {
            //does nothing
        }
    }
}
