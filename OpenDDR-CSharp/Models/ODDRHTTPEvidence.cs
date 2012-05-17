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
    public class ODDRHTTPEvidence : IEvidence
    {
        Dictionary<String, String> headers;

        public ODDRHTTPEvidence() {
            headers = new Dictionary<String, String>();
        }

        public ODDRHTTPEvidence(Dictionary<String, String> map) {
            headers = new Dictionary<String, String>(map);
        }

        public bool Exist(string property)
        {
            if (property == null)
            {
                return false;
            }
            return headers.ContainsKey(property.ToLower());
        }

        public String Get(String header)
        {
            string toRet = null;
            headers.TryGetValue(header.ToLower(), out toRet);
            return toRet;
        }

        /**
         *
         * @param key case insensitive
         * @param value case sensitive
         */
        public virtual void Put(String key, String value)
        {
            headers.Add(key.ToLower(), value);
        }
    }
}
