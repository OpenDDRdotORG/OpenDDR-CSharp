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

namespace W3c.Ddr.Simple
{
    /// <summary>
    /// Evidence is the general term applied to providing information to the DDR to allow it to determine the Delivery Context.
    /// </summary>
    /// <remarks>
    /// In the DDR Simple API implementations must support Evidence consisting of HTTP Header name and value pairs. Implementations must treat HTTP Header names in a case insensitive manner. HTTP Header values may be case sensitive, depending on the header concerned. Other types of Evidence may be supported by implementations. They are not defined in http://www.w3.org/TR/DDR-Simple-API/ Recommendation.
    /// </remarks>
    public interface IEvidence
    {
        /// <summary>
        /// Add Evidence
        /// </summary>
        /// <param name="key">Header name</param>
        /// <param name="value">Header value</param>
        void Put(String key, String value);

        /// <summary>
        /// Query Evidence
        /// </summary>
        /// <param name="key">Header name</param>
        /// <returns>Return true if requested evidence exist</returns>
        bool Exist(String key);

        /// <summary>
        /// Retrieve Evidence
        /// </summary>
        /// <param name="key">Header name</param>
        /// <returns>Return the value of the Evidence identified by key</returns>
        String Get(String key);
    }
}
