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

namespace Oddr.Models
{
    public class BuiltObject
    {
        public int confidence
        {
            set;
            get;
        }
        public Dictionary<String, String> properties
        {
            protected set;
            get;
        }

        public BuiltObject() {
            this.properties = new Dictionary<String, String>();
            this.confidence = 0;
        }

        public BuiltObject(int confidence, Dictionary<String, String> properties)
        {
            this.confidence = confidence;
            this.properties = properties;
        }

        public BuiltObject(Dictionary<String, String> properties)
        {
            this.confidence = 0;
            this.properties = properties;
        }

        public String Get(String property)
        {
            if (properties.ContainsKey(property))
            {
                return properties[property];
            }
            return null;
        }

        public void PutProperty(String name, String value) {
            this.properties.Add(name, value);
        }

        public void PutPropertiesMap(Dictionary<String, String> properties)
        {
            this.properties = this.properties.Concat(properties).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
