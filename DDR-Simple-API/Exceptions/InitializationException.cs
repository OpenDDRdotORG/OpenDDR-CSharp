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
    /// This is a subclass of DDRException and represents an error during the initialization phase of the Simple API. It is thrown only by the initialize method of the Service interface and the newService method of the ServiceFactory class.
    /// </summary>
    public class InitializationException : DDRException
    {
        /// <summary>
        /// There was a problem during initialization. Implementations may define specific codes for different kinds of failures during initialization.
        /// </summary>
        public static int INITIALIZATION_ERROR = 300;

        public InitializationException()
            : base()
        {
        }

        public InitializationException(int code, String message)
            : base(code, message)
        {
        }

        public InitializationException(int code, Exception ex)
            : base(code, ex)
        {
        }

    }
}
