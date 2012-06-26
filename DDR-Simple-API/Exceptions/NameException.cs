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
    /// This is a subclass of DDRException and is thrown when it is detected that the name of a Property or Aspect or vocabulary IRI is in error. The exception code, when set, indicates the nature of the error.
    /// A name of a Property or Aspect or a vocabulary IRI are in error when they are not syntactically valid or are not supported by the implementation.
    /// </summary>
    public class NameException : DDRException
    {
        /// <summary>
        /// The name of a Property is in error
        /// </summary>
        public static int PROPERTY_NOT_RECOGNIZED = 100;

        /// <summary>
        /// The name of an Aspect is in error
        /// </summary>
        public static int VOCABULARY_NOT_RECOGNIZED = 200;

        /// <summary>
        /// A vocabulary IRI is in error
        /// </summary>
        public static int ASPECT_NOT_RECOGNIZED = 800;

        public NameException()
            : base()
        {
        }

        public NameException(int code, String message)
            : base(code, message)
        {
        }

        public NameException(int code, Exception ex)
            : base(code, ex)
        {
        }
    }
}
