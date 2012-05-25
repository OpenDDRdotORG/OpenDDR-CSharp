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
using Oddr.Models.Devices;
using Oddr.Models;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace Oddr.Builders.Devices
{
    public class IOSDeviceBuilder : IDeviceBuilder
    {
        private OrderedDictionary iOSDevices;
        private Dictionary<String, Device> devices;

        public IOSDeviceBuilder()
            : base()
        {
            iOSDevices = new OrderedDictionary();
        }

        public void PutDevice(string deviceID, List<string> initProperties)
        {
            try
            {
                iOSDevices.Add(initProperties[0], deviceID);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(this.GetType().FullName + " " + initProperties[0] + " " + deviceID + " " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        public void CompleteInit(Dictionary<string, Device> devices)
        {
            String global = "iPhone";
            if (iOSDevices.Contains(global)) {
                String iphone = (string)iOSDevices[global];
                iOSDevices.Remove(global);
                iOSDevices.Add(global, iphone);
            }

            this.devices = devices;

            foreach (String deviceID in iOSDevices.Values) {
                if (!devices.ContainsKey(deviceID)) {
                    throw new InvalidOperationException("unable to find device with id: " + deviceID + " in devices");
                }
            }
        }

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsIOSDevices && (!userAgent.containsAndroid) && (!userAgent.containsWindowsPhone);
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            foreach (string token in iOSDevices.Keys)
            {
                Regex tokenRegex = new Regex(".*" + token + ".*");
                if (tokenRegex.IsMatch(userAgent.completeUserAgent))
                {
                    if (iOSDevices.Contains(token))
                    {
                        String iosDeviceID = (string)iOSDevices[token];
                        Device retDevice = null;
                        if (devices.TryGetValue(iosDeviceID, out retDevice))
                        {
                            retDevice = (Device)retDevice.Clone();
                            retDevice.confidence = 90;
                            return retDevice;
                        }
                    }
                }
            }
            return null;
        }
    }
}
