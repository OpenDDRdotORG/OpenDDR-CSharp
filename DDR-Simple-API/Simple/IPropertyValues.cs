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
    /// A set of PropertyValues.
    /// </summary>
    public interface IPropertyValues
    {
        /// <summary>
        /// Get All Properties in the Set
        /// </summary>
        /// <returns>Return an array of IPropertyValue</returns>
        IPropertyValue[] GetAll();

        /// <summary>
        /// Get the Named Property
        /// </summary>
        /// <returns>Return the IPropertyValue associated to the requested IPropertyRef </returns>
        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValue GetValue(IPropertyRef pr);
    }
}
