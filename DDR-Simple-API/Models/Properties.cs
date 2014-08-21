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
using System.Web;

namespace W3c.Ddr.Models
{
    /// <summary>
    /// This Class models a set of properties. 
    /// </summary>
    public class Properties
    {
        private Dictionary<String, object> list;
        private String filename;

        public Properties()
        {
            list = new Dictionary<String, object>();
        }

        public Properties(String file)
        {
            Reload(file, "");
        }

		public Properties(String oddrPropertiesFilename, String relativeWebPath)
		{
			ReloadWeb(oddrPropertiesFilename, relativeWebPath);
		}

        public String GetProperty(String field, String defValue)
        {
            return (GetProperty(field) == null) ? (defValue) : (GetProperty(field));
        }

        public String GetProperty(String field)
        {
            object oval = null;

            if (list.TryGetValue(field, out oval))
            {
                return oval as string;
            }
            return null;
        }

        public object Get(string field)
        {
            object oval = null;
            if (list.TryGetValue(field, out oval))
            {
                return oval;
            }
            return null;
        }

        public void Set(String field, Object value)
        {
            if (!list.ContainsKey(field))
                list.Add(field, value.ToString());
            else
                list[field] = value.ToString();
        }

        public void Save()
        {
            Save(this.filename);
        }

        public void Save(String filename)
        {
            this.filename = filename;

            if (!System.IO.File.Exists(filename))
                System.IO.File.Create(filename);

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);

            foreach (String prop in list.Keys.ToArray())
                if (!(String.IsNullOrEmpty(list[prop] as string) || (list[prop] as string).Trim().Length == 0))
                    file.WriteLine(prop + "=" + list[prop]);

            file.Close();
        }

        public void Reload()
        {
            Reload(this.filename, "");
        }

		public void ReloadWeb(String oddrPropertiesFilename, String relativeWebPath)
		{
			if (!relativeWebPath.EndsWith("/"))
			{
				relativeWebPath += "/";
			}

			if (oddrPropertiesFilename.StartsWith("/"))
			{
				oddrPropertiesFilename = oddrPropertiesFilename.TrimStart('/');
			}

			oddrPropertiesFilename = HttpContext.Current.Server.MapPath(relativeWebPath + oddrPropertiesFilename);

			Reload(oddrPropertiesFilename, relativeWebPath);
		}

		public void Reload(String filename, String relativeWebPath)
        {
            this.filename = filename;
            list = new Dictionary<String, object>();

            if (System.IO.File.Exists(filename))
				LoadFromFile(filename, relativeWebPath);
            else
                System.IO.File.Create(filename);
        }

        private void LoadFromFile(String file, String relativeWebPath)
        {
            foreach (String line in System.IO.File.ReadAllLines(file))
            {
                if ((!String.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    (line.Contains('=')))
                {
                    int index = line.IndexOf('=');
                    String key = line.Substring(0, index).Trim();
                    String value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    try
                    {
						if (!String.IsNullOrEmpty(relativeWebPath))
						{
							if (!relativeWebPath.EndsWith("/"))
							{
								relativeWebPath += "/";
							}

							if (value.Contains("/FILESYSTEM_PATH_TO_RESOURCES/"))
							{
								value = value.Replace("/FILESYSTEM_PATH_TO_RESOURCES/", relativeWebPath);
								value = HttpContext.Current.Server.MapPath(value);
							}
						}

                        //ignore dublicates
                        list.Add(key, value);
                    }
                    catch { }
                }
            }
        }
    }
}
