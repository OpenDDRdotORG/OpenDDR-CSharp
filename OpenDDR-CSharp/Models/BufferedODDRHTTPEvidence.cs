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
    public class BufferedODDRHTTPEvidence : ODDRHTTPEvidence
    {
        public Browser browserFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }
        public Device deviceFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }
        public OSModel.OperatingSystem osFound
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public override void Put(String key, String value) {
            this.osFound = null;
            this.browserFound = null;
            base.Put(key, value);
        }
    }
}
