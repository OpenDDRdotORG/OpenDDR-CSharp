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
    public class SystemException : Exception
    {
        public static int ILLEGAL_ARGUMENT = 400;

        public static int CANNOT_PROCEED = 500;

        protected int code
        {
            get;
            private set;
        }

        public SystemException()
            : base()
        {
        }

        public SystemException(int code, String message)
            : base(message)
        {
            this.code = code;
        }

        public SystemException(int code, Exception ex)
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
