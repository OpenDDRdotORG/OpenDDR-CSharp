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

namespace Oddr.Models
{
    class UserAgentFactory
    {
        public static UserAgent newBrowserUserAgent(IEvidence evidence)
        {
            return newUserAgent(evidence.Get("user-agent"));
        }

        public static UserAgent newBrowserUserAgent(Dictionary<String, String> headers)
        {
            return newBrowserUserAgent(new ODDRHTTPEvidence(headers));
        }

        public static UserAgent newDeviceUserAgent(IEvidence evidence)
        {
            String ua = evidence.Get("x-device-user-agent");
            if (ua == null || ua.Trim().Length < 2)
            {
                ua = evidence.Get("user-agent");
            }
            return newUserAgent(ua);
        }

        public static UserAgent newDeviceUserAgent(Dictionary<String, String> headers)
        {
            return newDeviceUserAgent(new ODDRHTTPEvidence(headers));
        }

        public static UserAgent newUserAgent(String realUserAgent)
        {
            return new UserAgent(realUserAgent);
        }
    }
}
