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
using System.Collections.Specialized;
using Oddr.Models.Devices;
using Oddr.Models;
using System.Text.RegularExpressions;

namespace Oddr.Builders.Devices
{
    public class SimpleDeviceBuilder : IDeviceBuilder
    {
        private OrderedDictionary simpleTokenMap;
        private Dictionary<String, Device> devices;

        public SimpleDeviceBuilder()
            : base()
        {
            simpleTokenMap = new OrderedDictionary();
        }

        public void PutDevice(string deviceID, List<string> initProperties)
        {
            foreach (String token in initProperties)
            {
                simpleTokenMap.Add(token, deviceID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        public void CompleteInit(Dictionary<string, Device> devices)
        {
            this.devices = devices;

            foreach (String deviceID in simpleTokenMap.Values)
            {
                if (!devices.ContainsKey(deviceID))
                {
                    throw new InvalidOperationException("unable to find device with id: " + deviceID + "in devices");
                }
            }
        }

        public bool CanBuild(UserAgent userAgent)
        {
            foreach (String token in simpleTokenMap.Keys)
            {
                Regex tokenRegex = new Regex(/*"(?i).*"*/".*" + Regex.Escape(token) + ".*", RegexOptions.IgnoreCase);
                if (tokenRegex.IsMatch(userAgent.completeUserAgent))
                {
                    return true;
                }
            }
            return false;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            foreach (string token in simpleTokenMap.Keys)
            {
                Regex tokenRegex = new Regex(/*"(?i).*"*/".*" + Regex.Escape(token) + ".*", RegexOptions.IgnoreCase);
                if (tokenRegex.IsMatch(userAgent.completeUserAgent) && simpleTokenMap.Contains(token))
                {
                    String desktopDeviceId = (string)simpleTokenMap[token];
                    Device device = null;
                    if (devices.TryGetValue(desktopDeviceId, out device))
                    {
                        return device;
                    }
                }
            }
            return null;
        }
    }
}
