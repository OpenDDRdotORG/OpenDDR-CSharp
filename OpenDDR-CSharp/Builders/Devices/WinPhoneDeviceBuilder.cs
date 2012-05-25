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

namespace Oddr.Builders.Devices
{
    public class WinPhoneDeviceBuilder : OrderedTokenDeviceBuilder
    {
        private Dictionary<String, Device> devices;

        public WinPhoneDeviceBuilder()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        protected override void AfterOrderingCompleteInit(Dictionary<string, Device> devices)
        {
            this.devices = devices;
            foreach (Object devIdObj in orderedRules.Values) {
                String devId = (String) devIdObj;
                if (!devices.ContainsKey(devId)) {
                    throw new InvalidOperationException("unable to find device with id: " + devId + " in devices");
                }
            }
        }

        public override void PutDevice(string deviceID, List<string> initProperties)
        {
            try
            {
                orderedRules.Add(initProperties[0], deviceID);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(this.GetType().FullName + " " + initProperties[0] + " " + deviceID + " " + ex.Message);
            }
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            return userAgent.containsWindowsPhone;
        }

        public override BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<Device> foundDevices = new List<Device>();
            foreach (string token in orderedRules.Keys)
            {
                Device d = ElaborateWinPhoneDeviceWithToken(userAgent, token);
                if (d != null)
                {
                    if (d.confidence > confidenceTreshold)
                    {
                        return d;

                    }
                    else
                    {
                        if (d.confidence > 0)
                        {
                            foundDevices.Add(d);
                        }
                    }
                }
            }
            if (foundDevices.Count > 0)
            {
                foundDevices.Sort();
                foundDevices.Reverse();
                return foundDevices[0];
            }
            return null;
        }

        private Device ElaborateWinPhoneDeviceWithToken(UserAgent userAgent, String token)
        {
            if (userAgent.mozillaPattern || userAgent.operaPattern)
            {
                int subtract = 0;
                String currentToken = token;

                String looseToken = Regex.Replace(token, "[ _/-]", ".?");

                Regex looseRegex = new Regex(/*"(?i).*"*/".*" + looseToken + ".*", RegexOptions.IgnoreCase);

                if (!looseRegex.IsMatch(userAgent.completeUserAgent))
                {
                    return null;
                }

                Regex currentRegex = null;

                if (userAgent.operaPattern)
                {
                    subtract += 10;
                }

                for (int i = 0; i <= 1; i++)
                {
                    if (i == 1)
                    {
                        currentToken = looseToken;
                    }

                    currentRegex = new Regex(/*"(?i).*"*/".*" + currentToken, RegexOptions.IgnoreCase);
                    if (userAgent.GetPatternElementsInside() != null && currentRegex.IsMatch(userAgent.GetPatternElementsInside()))
                    {
                        String deviceId = (String)orderedRules[token];

                        Device retDevice = null;
                        if (devices.TryGetValue(deviceId, out retDevice))
                        {
                            retDevice = (Device)retDevice.Clone();
                            retDevice.confidence = (100 - subtract);
                            return retDevice;
                        }
                    }
                    if (userAgent.GetPatternElementsPre() != null && currentRegex.IsMatch(userAgent.GetPatternElementsPre()))
                    {
                        String deviceId = (String)orderedRules[token];

                        Device retDevice = null;
                        if (devices.TryGetValue(deviceId, out retDevice))
                        {
                            retDevice = (Device)retDevice.Clone();
                            retDevice.confidence = (95 - subtract);
                            return retDevice;
                        }
                    }

                    currentRegex = new Regex(/*"(?i).*"*/".*" + currentToken + ".?;.*", RegexOptions.IgnoreCase);
                    if (userAgent.GetPatternElementsInside() != null && currentRegex.IsMatch(userAgent.GetPatternElementsInside()))
                    {
                        String deviceId = (String)orderedRules[token];

                        Device retDevice = null;
                        if (devices.TryGetValue(deviceId, out retDevice))
                        {
                            retDevice = (Device)retDevice.Clone();
                            retDevice.confidence = (90 - subtract);
                            return retDevice;
                        }
                    }

                    if (i == 1)
                    {
                        currentRegex = looseRegex;

                    }
                    else
                    {
                        currentRegex = new Regex(/*"(?i).*"*/".*" + currentToken + ".*", RegexOptions.IgnoreCase);
                    }

                    if (userAgent.GetPatternElementsInside() != null && currentRegex.IsMatch(userAgent.GetPatternElementsInside()))
                    {
                        String deviceId = (String)orderedRules[token];

                        Device retDevice = null;
                        if (devices.TryGetValue(deviceId, out retDevice))
                        {
                            retDevice = (Device)retDevice.Clone();
                            retDevice.confidence = (80 - subtract);
                            return retDevice;
                        }
                    }
                    if (userAgent.GetPatternElementsPre() != null && currentRegex.IsMatch(userAgent.GetPatternElementsPre()))
                    {
                        String deviceId = (String)orderedRules[token];

                        Device retDevice = null;
                        if (devices.TryGetValue(deviceId, out retDevice))
                        {
                            retDevice = (Device)retDevice.Clone();
                            retDevice.confidence = (80 - subtract);
                            return retDevice;
                        }
                    }
                    if (userAgent.GetPatternElementsPost() != null && currentRegex.IsMatch(userAgent.GetPatternElementsPost()))
                    {
                        String deviceId = (String)orderedRules[token];

                        Device retDevice = null;
                        if (devices.TryGetValue(deviceId, out retDevice))
                        {
                            retDevice = (Device)retDevice.Clone();
                            retDevice.confidence = (60 - subtract);
                            return retDevice;
                        }
                    }
                    subtract += 20;
                }
            }

            return null;
        }
    }
}
