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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Oddr.Models.OS;
using Oddr.Vocabularies;
using W3c.Ddr.Exceptions;

namespace Oddr.Documenthandlers
{
    public class OperatingSystemDatasourceParser
    {
        private const string ELEMENT_OPERATING_SYSTEM = "OperatingSystem";
        private const String ELEMENT_OPERATING_SYSTEM_DESCRIPTION = "operatingSystem";
        private const String ELEMENT_PROPERTY = "property";
        private const String ATTRIBUTE_OS_ID = "id";
        private const String ATTRIBUTE_PROPERTY_NAME = "name";
        private const String ATTRIBUTE_PROPERTY_VALUE = "value";
        private XDocument doc;
        private VocabularyHolder vocabularyHolder;
        public SortedDictionary<String, Oddr.Models.OS.OperatingSystem> operatingSystems
        {
            private set;
            get;
        }

        public OperatingSystemDatasourceParser(Stream stream)
        {
            Init(stream);
        }

        public OperatingSystemDatasourceParser(Stream stream, VocabularyHolder vocabularyHolder)
        {
            Init(stream);
            try
            {
                vocabularyHolder.ExistVocabulary(ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI);
                this.vocabularyHolder = vocabularyHolder;
            }
            catch (Exception ex)
            {
                this.vocabularyHolder = null;
            }

        }

        private void Init(Stream stream)
        {
            this.operatingSystems = new SortedDictionary<string, Oddr.Models.OS.OperatingSystem>(StringComparer.Ordinal);
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(stream);
                doc = XDocument.Load(reader);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }
            finally
            {
                if (reader != null)
                {
                    ((IDisposable)reader).Dispose();
                }
            }
        }

        /// <exception cref="Exception">Thrown when...</exception>
        public void Parse()
        {
            if (doc == null)
            {
                throw new Exception("Input stream is not valid");
            }

            OperatingSystemWrapper[] osWrapperArray = (from os in doc.Descendants(ELEMENT_OPERATING_SYSTEM).Descendants(ELEMENT_OPERATING_SYSTEM_DESCRIPTION)
                                                       select new OperatingSystemWrapper
                                             {
                                                 id = os.Attribute(ATTRIBUTE_OS_ID).Value,
                                                 properties = (from prop in os.Descendants(ELEMENT_PROPERTY)
                                                               select new StringPair
                                                               {
                                                                   key = prop.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                                   value = prop.Attribute(ATTRIBUTE_PROPERTY_VALUE).Value,
                                                               }).ToArray<StringPair>(),
                                             }).ToArray<OperatingSystemWrapper>();

            foreach (OperatingSystemWrapper osw in osWrapperArray)
            {
                Oddr.Models.OS.OperatingSystem os = osw.GetOperatingSystem(vocabularyHolder);
                operatingSystems.Add(osw.id, os);
            }
        }

        private class OperatingSystemWrapper
        {
            public string id;
            public StringPair[] properties;

            public Oddr.Models.OS.OperatingSystem GetOperatingSystem(VocabularyHolder vocabularyHolder)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                if (vocabularyHolder != null)
                {
                    foreach (StringPair sp in properties)
                    {
                        try
                        {
                            //vocabularyHolder.ExistProperty(sp.key, ODDRService.ASPECT_OPERATIVE_SYSTEM, ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI);
                            if (vocabularyHolder.ExistProperty(sp.key, ODDRService.ASPECT_OPERATIVE_SYSTEM, ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI) != null)
                            {
                                dic.Add(sp.key, sp.value);
                            }
                        }
                        //catch (NameException ex)
                        //{
                        //    //property non loaded
                        //}
                        catch (ArgumentException ae)
                        {
                            //Console.WriteLine(this.GetType().FullName + " " + sp.key + " already present!!!");
                        }
                    }
                }
                else
                {
                    foreach (StringPair sp in properties)
                    {
                        dic.Add(sp.key, sp.value);
                    }
                }

                return new Oddr.Models.OS.OperatingSystem(dic);
            }
        }

        private class StringPair
        {
            public string key;
            public string value;
        }
    }
}
