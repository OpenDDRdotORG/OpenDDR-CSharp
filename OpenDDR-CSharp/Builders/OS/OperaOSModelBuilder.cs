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
using OSModel = Oddr.Models.OS;
using Oddr.Builders.OS.Mozilla;
using Oddr.Builders.Devices;

namespace Oddr.Builders.OS
{
    public class OperaOSModelBuilder : IBuilder
    {
        private IBuilder[] builders;

        public OperaOSModelBuilder()
        {
            builders = new IBuilder[]
            {
                new AndroidMozillaSubBuilder(),
                new SymbianMozillaSubBuilder(),
                new WinCEMozillaSubBuilder(),
                new WinPhoneDeviceBuilder()
            };
        }

        public bool CanBuild(UserAgent userAgent)
        {
            return userAgent.operaPattern;
        }

        public BuiltObject Build(UserAgent userAgent, int confidenceTreshold)
        {
            List<OSModel.OperatingSystem> founds = new List<OSModel.OperatingSystem>();
            OSModel.OperatingSystem found = null;
            foreach (IBuilder builder in builders)
            {
                if (builder.CanBuild(userAgent))
                {
                    OSModel.OperatingSystem builded = (OSModel.OperatingSystem)builder.Build(userAgent, confidenceTreshold);
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
                if (founds.Count > 0)
                {
                    founds.Sort();
                    founds.Reverse();
                    return founds[0];
                }

                return null;
            }
        }
    }
}
