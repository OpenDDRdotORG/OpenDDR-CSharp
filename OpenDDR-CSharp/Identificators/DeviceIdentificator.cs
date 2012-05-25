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
using W3c.Ddr.Simple;
using Oddr.Models;
using Oddr.Models.Devices;
using Oddr.Builders.Devices;

namespace Oddr.Identificators
{
    public class DeviceIdentificator : IIdentificator
    {
        private IDeviceBuilder[] builders;
        private Dictionary<String, Device> devices;

        public DeviceIdentificator(IDeviceBuilder[] builders, Dictionary<String, Device> devices)
        {
            this.builders = builders;
            this.devices = devices;
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
            List<Device> foundDevices = new List<Device>();
            Device foundDevice = null;
            foreach (IDeviceBuilder deviceBuilder in builders)
            {
                if (deviceBuilder.CanBuild(userAgent))
                {
                    Device device = (Device)deviceBuilder.Build(userAgent, confidenceTreshold);
                    if (device != null)
                    {
                        String parentId = device.parentId;
                        Device parentDevice = null;
                        while (!"root".Equals(parentId))
                        {
                            if (devices.TryGetValue(parentId, out parentDevice))
                            {
                                foreach (KeyValuePair<string, string> entry in parentDevice.properties)
                                {
                                    if (!device.ContainsProperty(entry.Key))
                                    {
                                        device.PutProperty(entry.Key, entry.Value);
                                    }
                                }
                                parentId = parentDevice.parentId;
                            }
                        }

                        foundDevices.Add(device);
                        if (device.confidence >= confidenceTreshold)
                        {
                            foundDevice = device;
                            break;
                        }
                    }
                }
            }

            if (foundDevice != null)
            {
                return foundDevice;

            }
            else
            {
                if (foundDevices.Count > 0)
                {
                    foundDevices.Sort();
                    foundDevices.Reverse();
                    return foundDevices[0];
                }

                return null;
            }
        }

        public void CompleteInit()
        {
            foreach (IDeviceBuilder deviceBuilder in builders)
            {
                try
                {
                    deviceBuilder.CompleteInit(devices);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(this.GetType().FullName + " " + ex.Message);
                }
            }
        }
    }
}
