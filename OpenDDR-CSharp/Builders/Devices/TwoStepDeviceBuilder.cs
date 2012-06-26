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
using System.Globalization;

namespace Oddr.Builders.Devices
{
    public class TwoStepDeviceBuilder : OrderedTokenDeviceBuilder
    {
        private Dictionary<String, Device> devices;
        private Dictionary<String, Object> regexs = new Dictionary<String, Object>();
        private static readonly Regex litteralRegex = new Regex(".*[a-zA-Z].*", RegexOptions.Compiled);
        private static readonly Regex betweenTokensRegex = new Regex("[ _/-]?", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        /// <exception cref="System.InvalidOperationException">Thrown when unable to find device with id in devices</exception>
        protected override void AfterOrderingCompleteInit(Dictionary<string, Device> devices)
        {
            foreach (String step1Token in orderedRules.Keys)
            {

                SortElement(step1Token);
            }

            this.devices = devices;
            foreach (Object devIdsMapObj in orderedRules.Values)
            {
                OrderedDictionary devIdsMap = devIdsMapObj as OrderedDictionary;
                if (devIdsMap != null)
                {
                    foreach (Object devIdObj in devIdsMap.Values)
                    {
                        String devId = (String)devIdObj;
                        if (!devices.ContainsKey(devId))
                        {
                            throw new InvalidOperationException("unable to find device with id: " + devId + " in devices");
                        }
                    }
                }
                else
                {
                    //Console.WriteLine(this.GetType().FullName + " can't cast orderedRules' values in OrderedDictionary");
                }
            }
        }

        private void SortElement(String step1Token)
        {
            OrderedDictionary currentStep1Map = (OrderedDictionary)orderedRules[step1Token];

            OrderedDictionary tmp = new OrderedDictionary();
            List<String> keys = new List<string>();
            foreach (String key in currentStep1Map.Keys)
            {
                keys.Add(key);
            }

            keys.Sort(new OrderedStep1KeyComparer());

            foreach (String str in keys)
            {
                tmp.Add(str, currentStep1Map[str]);
            }

            currentStep1Map = new OrderedDictionary();

            List<String> keysOrdered = orderKeys(keys);

            foreach (String key in keysOrdered)
            {
                currentStep1Map.Add(key, tmp[key]);
                tmp.Remove(key);
            }

            orderedRules[step1Token] = currentStep1Map;
        }

        private List<String> orderKeys(List<String> keys)
        {
            List<String> keysOrdered = new List<String>();

            while (keys.Count > 0)
            {
                if (!findKey(keys, keysOrdered))
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
                    keys.Remove(keys[idx]);
                }
            }

            return keysOrdered;
        }

        private bool findKey(List<String> keys, List<String> keysOrdered)
        {
            bool found = false;
            //CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            //foreach (String k1 in keys)
            foreach (string k1 in keys)
            {
                Regex k1Regex = new Regex(".*" + k1 + ".*");
                //foreach (String k2 in keys)
                foreach (string k2 in keys)
                {
                    if ((!k1.Equals(k2)) && k1Regex.IsMatch(k2))
                    //if ((!k1.Equals(k2))/* && k2.Contains(k1)*/ && compareInfo.IndexOf(k2, k1) != -1)
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

            return found;
        }

        public override void PutDevice(string deviceID, List<string> initProperties)
        {
            String step1TokenString = initProperties[0];
            String step2TokenString = initProperties[1];

            Regex step1TokenStringRegex = new Regex(".*" + step1TokenString + ".*");
            if (step1TokenStringRegex.IsMatch(step2TokenString))
            //if (step2TokenString.Contains(step1TokenString))
            {
                step2TokenString = Regex.Replace(step2TokenString, step1TokenString + "[^a-zA-Z0-9]?", "");
            }

            OrderedDictionary step1Token = null;
            if (orderedRules.Contains(step1TokenString))
            {
                step1Token = (OrderedDictionary)orderedRules[step1TokenString];
            }

            if (step1Token == null)
            {
                step1Token = new OrderedDictionary();
                try
                {
                    regexs.Add(step1TokenString + "_icase", new Regex(/*"(?i).*"*/".?>*" + step1TokenString + ".*", RegexOptions.IgnoreCase));
                    orderedRules.Add(initProperties[0], step1Token);
                    //Console.WriteLine("initProperties[0] " + initProperties[0] + " step1Token " + step1Token);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(this.GetType().FullName + " " + "initProperties[0] " + initProperties[0] + " step1Token " + step1Token + " " + ex.Message);
                }
            }
            try
            {
                step1Token.Add(step2TokenString, deviceID);
                regexs.Add(step2TokenString + "_loose", Regex.Replace(step2TokenString, "[ _/-]", ".?"));
                regexs.Add(step2TokenString + "_loose_icase", new Regex(/*"(?i).*"*/".?>*" + Regex.Replace(step2TokenString, "[ _/-]", ".?") + ".*", RegexOptions.IgnoreCase));
                regexs.Add(step2TokenString + "_icase", new Regex(/*"(?i).*"*/".?>*" + step2TokenString + ".*", RegexOptions.IgnoreCase));
                //Console.WriteLine("step2token " + step2TokenString + " deviceID " + deviceID);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(this.GetType().FullName + " " + "step2token " + step2TokenString + " deviceID " + deviceID + " " + ex.Message);
            }
        }

        public override bool CanBuild(UserAgent userAgent)
        {
            foreach (String step1Token in orderedRules.Keys)
            {
                Regex step1TokenRegex = (Regex)(regexs[step1Token + "_icase"]);
                //Regex step1TokenRegex = new Regex(/*"(?i).*"*/".*" + step1Token + ".*", RegexOptions.IgnoreCase);
                if (step1TokenRegex.IsMatch(userAgent.completeUserAgent))
                {
                    return true;
                }
            }
            return false;
        }

        public override BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<Device> foundDevices = new List<Device>();
            foreach (String step1Token in orderedRules.Keys)
            {
                //Regex step1TokenRegex = new Regex("(?i).*" + step1Token + ".*");
                //if (step1TokenRegex.IsMatch(userAgent.completeUserAgent))
                if (Regex.IsMatch(userAgent.completeUserAgent, /*"(?i).*"*/".?>*" + step1Token + ".*", RegexOptions.IgnoreCase))
                {
                    OrderedDictionary step1Compliant = (OrderedDictionary)orderedRules[step1Token];
                    foreach (String step2token in step1Compliant.Keys)
                    {
                        Device d = ElaborateTwoStepDeviceWithToken(userAgent, step1Token, step2token);
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

        private Device ElaborateTwoStepDeviceWithToken(UserAgent userAgent, String step1Token, String step2Token)
        {
            int maxLittleTokensDistance = 4;
            int maxBigTokensDistance = 8;
            int confidence;

            String originalToken = step2Token;
            String looseToken = (String)(regexs[step2Token + "_loose"]); 
            //String looseToken = Regex.Replace(step2Token, "[ _/-]", ".?");
            Regex step2TokenRegex = (Regex)(regexs[step2Token + "_icase"]);  
            //Regex step2TokenRegex = new Regex(/*"(?i).*"*/".*" + step2Token + ".*", RegexOptions.IgnoreCase);
            if (step2TokenRegex.IsMatch(userAgent.completeUserAgent))
            {
                confidence = 100;

            }
            else
            {
                Regex looseTokenRegex = (Regex)(regexs[step2Token + "_loose_icase"]);
                //Regex looseTokenRegex = new Regex(/*"(?i).*"*/".*" + looseToken + ".*", RegexOptions.IgnoreCase);
                if (looseTokenRegex.IsMatch(userAgent.completeUserAgent))
                {
                    step2Token = looseToken;
                    confidence = 90;

                }
                else
                {
                    return null;
                }
            }

            Regex step1And2TokenRegex = new Regex(/*"(?i)(?:(?:.*"*/"(?:(?:.*" + step1Token + "(.*?)" + step2Token + ".*)|(?:.*" + step2Token + "(.*?)" + step1Token + ".*))", RegexOptions.IgnoreCase);
            if (step1And2TokenRegex.IsMatch(userAgent.completeUserAgent))
            {
                Match result = step1And2TokenRegex.Match(userAgent.completeUserAgent);
                //test for case sensitive match
                Regex step1And2TokenCaseSensitiveRegex = new Regex("(?:(?:.*" + step1Token + "(.*?)" + step2Token + ".*)|(?:.*" + step2Token + "(.*?)" + step1Token + ".*))");
                if (step1And2TokenCaseSensitiveRegex.IsMatch(userAgent.completeUserAgent))
                {
                    Match bestResult = step1And2TokenCaseSensitiveRegex.Match(userAgent.completeUserAgent);
                    result = bestResult;

                }
                else
                {
                    confidence -= 20;
                }

                GroupCollection groups = result.Groups;
                String betweenTokens = groups[1].Value;
                String s2 = groups[2].Value;
                if (s2 != null && s2.Length != 0 && (betweenTokens == null || betweenTokens.Length > s2.Length))
                {
                    betweenTokens = s2;
                }
                int betweenTokensLength = betweenTokens.Length;
                if (step2Token.Length > 3)
                {
                    if (betweenTokensLength > maxBigTokensDistance)
                    {
                        confidence -= 10;
                    }

                    if (orderedRules.Contains(step1Token))
                    {
                        OrderedDictionary d = (OrderedDictionary)orderedRules[step1Token];
                        String deviceId = null;
                        if (d.Contains(originalToken))
                        {
                            deviceId = (string)d[originalToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = confidence;
                                return retDevice;
                            }
                        }
                    }
                }

                if ((betweenTokensLength < maxLittleTokensDistance) || (betweenTokensLength < maxBigTokensDistance && (step2Token.Length < 6 || !litteralRegex.IsMatch(step2Token))))
                {
                    if (betweenTokensLength <= 1)
                    {
                        
                        if (!betweenTokensRegex.IsMatch(betweenTokens))
                        {
                            confidence -= 20;
                        }

                    }
                    else
                    {
                        confidence -= 40;
                    }

                    confidence -= (betweenTokensLength * 4);

                    if (orderedRules.Contains(step1Token))
                    {
                        OrderedDictionary d = (OrderedDictionary)orderedRules[step1Token];
                        String deviceId = null;

                        if (d.Contains(originalToken))
                        {
                            deviceId = (string)d[originalToken];
                            Device retDevice = null;
                            if (devices.TryGetValue(deviceId, out retDevice))
                            {
                                retDevice = (Device)retDevice.Clone();
                                retDevice.confidence = confidence;
                                return retDevice;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }

    public class OrderedStep1KeyComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (y.Length == x.Length)
            {
                return y.CompareTo(x);
            }
            else
            {
                return y.Length - x.Length;
            }
        }
    }
}
