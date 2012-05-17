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

namespace Oddr.Models.Browsers
{
    public class Browser : BuiltObject, IComparable, ICloneable
    {
        public String majorRevision
        {
            get; set;
        }
        public String minorRevision
        {
            get; set;
        }
        public String microRevision
        {
            get; set;
        }
        public String nanoRevision
        {
            get;
            set;
        }

        public Browser()
            : base()
        {
            Init();
        }

        public Browser(Dictionary<String, String> properties)
            : base(properties)
        {
            Init();
        }

        private void Init()
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
        //utility getter for core ddr properties
        public String GetCookieSupport()
        {
            return Get("cookieSupport");
        }

        public int GetDisplayHeight()
        {
            try {
                return int.Parse(Get("displayHeight"));

            } catch (Exception ex) {
                return -1;
            }
        }

        public int GetDisplayWidth()
        {
            try {
                return int.Parse(Get("displayWidth"));

            } catch (Exception ex) {
                return -1;
            }
        }

        public String GetImageFormatSupport()
        {
            return Get("imageFormatSupport");
        }

        public String GetInputModeSupport()
        {
            return Get("inputModeSupport");
        }

        public String GetMarkupSupport()
        {
            return Get("markupSupport");
        }

        public String GetModel()
        {
            return Get("model");
        }

        public String GetScriptSupport()
        {
            return Get("scriptSupport");
        }

        public String GetStylesheetSupport()
        {
            return Get("stylesheetSupport");
        }

        public String GetVendor()
        {
            return Get("vendor");
        }

        public String GetVersion()
        {
            return Get("version");
        }

        //utility getter for significant oddr browser properties
        public String GetRenderer()
        {
            return Get("layoutEngine");
        }

        public String GetRendererVersion()
        {
            return Get("layoutEngineVersion");
        }

        public String GetClaimedReference()
        {
            return Get("referencedBrowser");
        }

        public String GetClaimedReferenceVersion()
        {
            return Get("referencedBrowserVersion");
        }

        public String GetBuild()
        {
            return Get("build");
        }

        //SETTERS
        //utility setter for core ddr properties
        public void SetCookieSupport(String cookieSupport)
        {
            PutProperty("cookieSupport", cookieSupport);
        }

        public void SetDisplayHeight(int displayHeight)
        {            
            PutProperty("displayHeight", displayHeight.ToString());
        }

        public void SetDisplayWidth(int displayWidth)
        {
            PutProperty("displayWidth", displayWidth.ToString());
        }

        public void SetImageFormatSupport(String imageFormatSupport)
        {
            PutProperty("imageFormatSupport", imageFormatSupport);
        }

        public void SetInputModeSupport(String inputModeSupport)
        {
            PutProperty("inputModeSupport", inputModeSupport);
        }

        public void SetMarkupSupport(String markupSupport)
        {
            PutProperty("markupSupport", markupSupport);
        }

        public void SetModel(String model)
        {
            PutProperty("model", model);
        }

        public void SetScriptSupport(String scriptSupport)
        {
            PutProperty("scriptSupport", scriptSupport);
        }

        public void SetStylesheetSupport(String stylesheetSupport)
        {
            PutProperty("stylesheetSupport", stylesheetSupport);
        }

        public void SetVendor(String vendor)
        {
            PutProperty("vendor", vendor);
        }

        public void SetVersion(String version)
        {
            PutProperty("version", version);
        }

        //utility setter for significant oddr browser properties
        public void SetLayoutEngine(String layoutEngine)
        {
            PutProperty("layoutEngine", layoutEngine);
        }

        public void SetLayoutEngineVersion(String layoutEngineVersion)
        {
            PutProperty("layoutEngineVersion", layoutEngineVersion);
        }

        public void SetReferenceBrowser(String referenceBrowser)
        {
            PutProperty("referenceBrowser", referenceBrowser);
        }

        public void SetReferenceBrowserVersion(String referenceBrowserVersion)
        {
            PutProperty("referenceBrowserVersion", referenceBrowserVersion);
        }

        public void SetBuild(String build)
        {
            PutProperty("build", build);
        }
        
        //Comparable
        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is Browser))
            {
                return int.MaxValue;
            }

            Browser bd = (Browser) obj;
            return this.confidence - bd.confidence;
        }


        // Cloneable
        public object Clone()
        {
            Browser b = new Browser();
            b.majorRevision = this.majorRevision;
            b.minorRevision = this.minorRevision;
            b.microRevision = this.microRevision;
            b.nanoRevision = this.nanoRevision;
            b.confidence = this.confidence;
            b.PutPropertiesMap(this.properties);
 	        return b;
        }
        
        //Utility
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetVendor());
            sb.Append(" ");
            sb.Append(GetModel());

            sb.Append("(");
            if (GetRenderer() != null && GetRenderer().Length > 0) {
                sb.Append(" ");
                sb.Append(GetRenderer());
                sb.Append(" ");
                sb.Append(GetRendererVersion());
                sb.Append(" ");
            }
            if (GetClaimedReference() != null && GetClaimedReference().Length > 0) {
                sb.Append(" ");
                sb.Append(GetClaimedReference());
                sb.Append(" ");
                sb.Append(GetClaimedReferenceVersion());
                sb.Append(" ");
            }
            sb.Append(GetVersion()).Append(")");

            sb.Append(" [").Append(this.majorRevision).Append(".").Append(this.minorRevision).Append(".").Append(this.microRevision).Append(".").Append(this.nanoRevision).Append("]");
            if (GetBuild() != null) {
                sb.Append(" - ").Append(GetBuild());
            }
            if (GetDisplayWidth() > 0 && GetDisplayHeight() > 0) {
                sb.Append("<");
                sb.Append(GetDisplayWidth());
                sb.Append("x");
                sb.Append(GetDisplayHeight());
                sb.Append(">");
            }
            sb.Append(" ").Append(this.confidence).Append("%");
            String toRet = sb.ToString();
            return toRet;
        }
    }
}
