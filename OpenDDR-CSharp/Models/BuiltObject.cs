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
    /// <summary>
    /// Superclass of identified model object.
    /// </summary>
    public class BuiltObject
    {
        /// <summary>
        /// Confidence of identified model object.
        /// </summary>
        public int confidence
        {
            set;
            get;
        }
        /// <summary>
        /// Dictionary of properties of identified model object.
        /// </summary>
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

        /// <summary>
        /// Retrieve a property from properties dictionary.
        /// </summary>
        /// <param name="property">The name of requested properties.</param>
        /// <returns>Return the value of requested property.</returns>
        public String Get(String property)
        {
            if (properties.ContainsKey(property))
            {
                return properties[property];
            }
            return null;
        }

        /// <summary>
        /// Add a property to properties dictionary.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void PutProperty(String name, String value) {
            this.properties[name] = value;
        }

        public void PutPropertiesMap(Dictionary<String, String> properties)
        {
            foreach (KeyValuePair<string, string> kvp in properties)
            {
                this.properties[kvp.Key] = kvp.Value;
            }
        }
    }
}
