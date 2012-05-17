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
using W3c.Ddr.Simple;

namespace Oddr.Models
{
    public class ODDRPropertyRef : IPropertyRef
    {
        private readonly IPropertyName propertyName;
        private readonly String aspectName;

        public ODDRPropertyRef(IPropertyName propertyName, String aspectName)
        {
            this.propertyName = propertyName;
            this.aspectName = aspectName;
        }

        public string AspectName()
        {
            return aspectName;
        }

        public string LocalPropertyName()
        {
            return propertyName.LocalPropertyName();
        }

        public string Namespace()
        {
            return propertyName.Namespace();
        }

        public string NullAspect()
        {
            return "__NULL";
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            ODDRPropertyRef oddr = obj as ODDRPropertyRef;
            if ((System.Object)oddr == null)
            {
                return false;
            }

            return (oddr.AspectName().Equals(this.aspectName) && oddr.LocalPropertyName().Equals(this.LocalPropertyName()) && oddr.Namespace().Equals(this.Namespace()));
        }

        public override int GetHashCode()
        {
            int hash = 3;
            hash = 73 * hash + (this.propertyName != null ? this.propertyName.GetHashCode() : 0);
            hash = 73 * hash + (this.aspectName != null ? this.aspectName.GetHashCode() : 0);
            return hash;
        }
    }
}
