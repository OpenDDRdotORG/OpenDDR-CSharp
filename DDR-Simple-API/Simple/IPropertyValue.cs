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
    /// PropertyValue models a PropertyRef together with its value
    /// </summary>
    /// <remarks>
    /// Values may be empty, in which case the method exists returns false. An attempt to query an empty value causes a ValueException as does an attempt to query a value with an incompatible accessor method (string as float, for example). For the getString method implementations must return an implementation dependent String representation if the type of the value is not natively String. For other methods if the underlying type of the data does not match the method signature then a ValueException must be thrown.
    /// </remarks>
    public interface IPropertyValue
    {
        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        double GetDouble();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        long GetLong();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        bool GetBoolean();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        int GetInteger();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        String[] GetEnumeration();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        float GetFloat();

        /// <summary>
        /// Property Reference
        /// </summary>
        /// <returns>The PropertyRef that this PropertyValue refers to.</returns>
        IPropertyRef PropertyRef();

        /// <summary>Value Retrieval</summary>
        /// <exception cref="ValueException">Throws when query an empty value or attempt to query a value with an incompatible type</exception>
        String GetString();

        /// <summary>
        /// Existence
        /// </summary>
        /// <returns>True if a value is available, false otherwise.</returns>
        bool Exists();
    }
}
