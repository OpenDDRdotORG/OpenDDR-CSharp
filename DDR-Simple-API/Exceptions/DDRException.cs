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
    /// It is the superclass of all DDR Simple API exceptions other than SystemException.
    /// Implementations should raise subclasses of DDRException, they should not raise this exception directly.
    /// </summary>
    public class DDRException : Exception
    {
        public const long serialVersionUID = 2618094065573111548L;
        /// <summary>
        /// This code may be used by implementations to create custom error codes. All implementation specific codes must be greater than this value.
        /// Implementations may define specific codes for different kinds of failures during initialization.
        /// </summary>
        public static int IMPLEMENTATION_ERROR = 65536;

        public int code
        {
            get;
            protected set;
        }

        public DDRException()
            : base()
        {
        }

        public DDRException(int code, String message) 
            : base(message)
        {
            this.code = code;
        }

        public DDRException(int code, Exception ex)
            : base("", ex)
        {
            this.code = code;
        }

        public String GetMessage()
        {
            return base.Message;
        }
    }
}
