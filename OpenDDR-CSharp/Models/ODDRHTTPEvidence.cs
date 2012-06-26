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
    /// <summary>
    /// Evidence consisting of HTTP Header name and value pairs.
    /// </summary>
    public class ODDRHTTPEvidence : IEvidence
    {
        /// <summary>
        /// Headers dictionary.
        /// </summary>
        Dictionary<String, String> headers;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public ODDRHTTPEvidence() {
            headers = new Dictionary<String, String>();
        }

        /// <summary>
        /// Headers Dictionary parameterizable costructor.
        /// </summary>
        /// <param name="map">Headers Dictionary.</param>
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

        public virtual void Put(String key, String value)
        {
            headers.Add(key.ToLower(), value);
        }
    }
}
