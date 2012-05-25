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
using Oddr.Models.Devices;
using System.Text.RegularExpressions;
using Oddr.Models;
using System.Collections;

namespace Oddr.Builders.Devices
{
    public class AndroidDeviceBuilder : OrderedTokenDeviceBuilder
    {
        private const String BUILD_HASH_REGEXP = ".*Build/([^ \\)\\(]*).*";
        private Regex buildHashRegex = new Regex(BUILD_HASH_REGEXP);
        private Dictionary<String, Device> devices;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        protected override void AfterOrderingCompleteInit(Dictionary<string, Device> devices)
        {
            this.devices = devices;
            foreach (Object devIdObj in orderedRules.Values)
            {
                String devId = (String)devIdObj;
                if (!devices.ContainsKey(devId))
                {
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
            return userAgent.containsAndroid;
        }

        public override BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<Device> foundDevices = new List<Device>();
            ICollection tokens = orderedRules.Keys;
            foreach (string token in tokens)
            {
                Device d = ElaborateAndroidDeviceWithToken(userAgent, token);
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

        private Device ElaborateAndroidDeviceWithToken(UserAgent userAgent, String token)
        {
            if (userAgent.mozillaPattern || userAgent.operaPattern)
            {
                int subtract = 0;
                String currentToken = token;

                String looseToken = Regex.Replace(token, "[ _/-]", ".?");

                Regex looseRegex = new Regex(/*"(?i).*"*/".*" + looseToken + ".*", RegexOptions.IgnoreCase);
                string userAgentWithoutAndroid = userAgent.completeUserAgent.Replace("Android", "");

                if (!looseRegex.IsMatch(userAgentWithoutAndroid))
                {
                    return null;
                }

                String patternElementInsideClean = CleanPatternElementInside(userAgent.GetPatternElementsInside());

                Regex currentRegex = null;

                for (int i = 0; i <= 1; i++)
                {
                    if (i == 1)
                    {
                        currentToken = looseToken;
                    }

                    currentRegex = new Regex(/*"(?i).*"*/".*" + currentToken + ".?Build/.*", RegexOptions.IgnoreCase);
                    if (patternElementInsideClean != null && currentRegex.IsMatch(patternElementInsideClean))
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

                    currentRegex = new Regex(/*"(?i).*"*/".*" + currentToken, RegexOptions.IgnoreCase);
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

                    if (patternElementInsideClean != null && currentRegex.IsMatch(patternElementInsideClean))
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

                    currentRegex = new Regex(/*"(?i).*"*/".*" + currentToken + ".?;.*", RegexOptions.IgnoreCase);
                    if (patternElementInsideClean != null && currentRegex.IsMatch(patternElementInsideClean))
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
                    if (patternElementInsideClean != null && currentRegex.IsMatch(patternElementInsideClean))
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
                    if (i == 1)
                    {
                        if (userAgent.GetPatternElementsInside() != null && currentRegex.IsMatch(userAgent.GetPatternElementsInside()))
                        {
                            String deviceId = (String)orderedRules[token];

                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = (40 - subtract);
                                return retDevice;
                            }
                        }
                    }
                    subtract += 20;
                }
            }

            return null;
        }

        private String CleanPatternElementInside(String patternElementsInside)
        {
            String patternElementInsideClean = patternElementsInside;

            if (buildHashRegex.IsMatch(patternElementInsideClean))
            {
                Match buildHashMatcher = buildHashRegex.Match(patternElementInsideClean);
                GroupCollection groups = buildHashMatcher.Groups;

                String build = groups[1].Value;
                patternElementInsideClean = patternElementInsideClean.Replace("Build/" + build, "Build/");
            }
            patternElementInsideClean = patternElementInsideClean.Replace("Android", "");

            return patternElementInsideClean;
        }
    }
}
