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
using Oddr.Models.Browsers;
using Oddr.Models.Devices;
using System.Runtime.CompilerServices;
using OSModel = Oddr.Models.OS;

namespace Oddr.Models
{
    /// <summary>
    /// Extends ODDRHTTPEvidence. Contains the reference to identified Bowser, Device and OperatingSystem. 
    /// It can be used to retrieve back model object in order to get properties directly.
    /// </summary>
    public class BufferedODDRHTTPEvidence : ODDRHTTPEvidence
    {
        /// <summary>
        /// Identified Browser object. Null if not yet identified.
        /// </summary>
        public Browser browserFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        /// <summary>
        /// Identified Device object. Null if not yet identified.
        /// </summary>
        public Device deviceFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        /// <summary>
        /// Identified OperatingSystem object. Null if not yet identified.
        /// </summary>
        public OSModel.OperatingSystem osFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        /// <summary>
        /// When Evidence change, stored model object are removed in order to allow new identification. 
        /// </summary>
        /// <param name="key">Header name</param>
        /// <param name="value">Header value</param>
        public override void Put(String key, String value) {
            this.osFound = null;
            this.browserFound = null;
            base.Put(key, value);
        }
    }
}
