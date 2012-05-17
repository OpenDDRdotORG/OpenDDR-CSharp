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

namespace Oddr.Models.OS
{
    public class OperatingSystem : BuiltObject, IComparable, ICloneable
    {
        public String majorRevision
        {
            get;
            set;
        }
        public String minorRevision
        {
            get;
            set;
        }
        public String microRevision
        {
            get;
            set;
        }
        public String nanoRevision
        {
            get;
            set;
        }

        public OperatingSystem() : base()
        {
            this.majorRevision = "0";
            this.minorRevision = "0";
            this.microRevision = "0";
            this.nanoRevision = "0";
        }

        public OperatingSystem(Dictionary<String, String> properties) : base(properties)
        {
            this.majorRevision = "0";
            this.minorRevision = "0";
            this.microRevision = "0";
            this.nanoRevision = "0";
        }

        public String GetId()
        {
            if (GetModel() == null || GetVendor() == null) {
                return null;
            }
            String id = GetVendor() + "." + GetModel() + "." + this.majorRevision + "." + this.minorRevision + "." + this.microRevision + "." + this.nanoRevision;
            return id;
        }

        //GETTERS
        //utility getter for significant oddr OS properties
        public String GetModel()
        {
            return Get("model");
        }

        public String GetVendor()
        {
            return Get("vendor");
        }

        public String GetVersion()
        {
            return Get("version");
        }

        public String GetBuild()
        {
            return Get("build");
        }

        public String GetDescription()
        {
            return Get("description");
        }

        //SETTERS
        //utility setter for significant oddr OS properties
        public void SetModel(String model)
        {
            PutProperty("model", model);
        }

        public void SetVendor(String vendor)
        {
            PutProperty("vendor", vendor);
        }

        public void SetVersion(String version)
        {
            PutProperty("version", version);
        }

        public void SetBuild(String build)
        {
            PutProperty("build", build);
        }

        public void SetDescription(String description)
        {
            PutProperty("description", description);
        }

        //Comparable
        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is OperatingSystem)) {
                return int.MaxValue;
            }

            OperatingSystem bd = (OperatingSystem) obj;
            return this.confidence - bd.confidence;
        }

        // Cloneable
        public object Clone()
        {
            OperatingSystem os = new OperatingSystem();
            os.majorRevision = this.majorRevision;
            os.minorRevision = this.minorRevision;
            os.microRevision = this.microRevision;
            os.nanoRevision = this.nanoRevision;
            os.confidence = this.confidence;
            os.PutPropertiesMap(this.properties);
            return os;
        }

        //Utility
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetVendor());
            sb.Append(" ");
            sb.Append(GetModel());

            if (GetDescription() != null && GetDescription().Length > 0) {
                sb.Append("(");
                sb.Append(GetDescription());
                sb.Append(GetVersion()).Append(")");
            }

            sb.Append(" [").Append(this.majorRevision).Append(".").Append(this.minorRevision).Append(".").Append(this.microRevision).Append(".").Append(this.nanoRevision).Append("]");
            if (GetBuild() != null) {
                sb.Append(" - ").Append(GetBuild());
            }
            sb.Append(" ").Append(this.confidence).Append("%");
            String toRet = sb.ToString();
            return toRet;
        }
    }
}
