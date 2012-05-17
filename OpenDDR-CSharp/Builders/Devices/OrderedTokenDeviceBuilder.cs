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
using System.Text.RegularExpressions;
using Oddr.Models;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Oddr.Builders.Devices
{
    public abstract class OrderedTokenDeviceBuilder : IDeviceBuilder
    {
        protected OrderedDictionary orderedRules;

        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        protected abstract void AfterOrderingCompleteInit(Dictionary<String, Device> devices);

        public OrderedTokenDeviceBuilder()
        {
            orderedRules = new OrderedDictionary();
        }

        public void CompleteInit(Dictionary<string, Device> devices)
        {
            
            Dictionary<String, Object> tmp = new Dictionary<String, Object>();
            List<String> keys = new List<String>();
            foreach (string key in orderedRules.Keys)
            {
                keys.Add(key);
            }

            keys.Sort(new OrderedTokenDeviceComparer());

            foreach (String str in keys)
            {
                tmp.Add(str, orderedRules[str]);
            }
            List<String> keysOrdered = new List<String>();

            orderedRules = new OrderedDictionary();

            while (keys.Count > 0)
            {
                bool found = false;

                foreach (string k1 in keys)
                {
                    Regex k1Regex = new Regex(".*" + k1 + ".*");

                    foreach (string k2 in keys)
                    {
                        if ((!k1.Equals(k2)) && k1Regex.IsMatch(k2))
                        //if ((!k1.Equals(k2)) && Regex.IsMatch(k2, ".*" + k1 + ".*"))
                        //if ((!k1.Equals(k2)) && k2.Contains(k1))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        keysOrdered.Add(k1);
                        keys.Remove(k1);
                        break;
                    }
                }

                if (!found)
                {
                    continue;
                }
                int max = 0;
                int idx = -1;
                for (int i = 0; i < keys.Count; i++)
                {
                    String str = keys[i];
                    if (str.Length > max)
                    {
                        max = str.Length;
                        idx = i;
                    }
                }

                if (idx >= 0)
                {
                    keysOrdered.Add(keys[idx]);
                    keys.RemoveAt(idx);
                }
            }

            foreach (String key in keysOrdered)
            {
                orderedRules.Add(key, tmp[key]);
                tmp.Remove(key);
            }

            AfterOrderingCompleteInit(devices);
        }

        public abstract void PutDevice(string deviceID, List<string> initProperties);

        public abstract bool CanBuild(UserAgent userAgent);

        public abstract BuiltObject Build(UserAgent userAgent, int confidenceTreshold);
    }

    public class OrderedTokenDeviceComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return y.Length - x.Length;
        }
    }
}
