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

namespace W3c.Ddr.Exceptions
{
    /// <summary>
    /// This is a subclass of DDRException and is thrown when an error is detected during an attempt to retrieve the value of a Property using one of the value accessor methods of the PropertyValue class. The exception code indicates the nature of the error.
    /// </summary>
    public class ValueException : DDRException
    {
        /// <summary>
        /// The value represented by the PropertyValue is incompatible with the return type of the method used to retrieve it.
        /// </summary>
        public static int INCOMPATIBLE_TYPES = 600;

        /// <summary>
        /// The property value is unknown.
        /// </summary>
        public static int NOT_KNOWN = 900;

        /// <summary>
        /// The implementation is aware of multiple values for this Property.
        /// </summary>
        public static int MULTIPLE_VALUES = 10000;

        public ValueException()
            : base()
        {
        }

        public ValueException(int code, String message)
            : base(code, message)
        {
        }

        public ValueException(int code, Exception ex)
            : base(code, ex)
        {
        }
    }
}
