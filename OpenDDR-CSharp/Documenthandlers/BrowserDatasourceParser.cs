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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Oddr.Models;
using Oddr.Models.Browsers;
using Oddr.Vocabularies;

namespace Oddr.Documenthandlers
{
    public class BrowserDatasourceParser
    {
        private const String ELEMENT_BROWSERS = "Browsers";
        private const String ELEMENT_BROWSER_DESCRIPTION = "browser";
        private const String ELEMENT_PROPERTY = "property";
        private const String ATTRIBUTE_BROWSER_ID = "id";
        private const String ATTRIBUTE_PROPERTY_NAME = "name";
        private const String ATTRIBUTE_PROPERTY_VALUE = "value";
        private VocabularyHolder vocabularyHolder;
        private XDocument doc;
        public SortedDictionary<String, Browser> browsers
        {
            private set;
            get;
        }

        public BrowserDatasourceParser(Stream stream)
        {
            Init(stream);
        }

        public BrowserDatasourceParser(Stream stream, VocabularyHolder vocabularyHolder)
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
            this.browsers = new SortedDictionary<string, Browser>(StringComparer.Ordinal);
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

            BrowserWrapper[] browserArray = (from b in doc.Descendants(ELEMENT_BROWSERS).Descendants(ELEMENT_BROWSER_DESCRIPTION)
                                             select new BrowserWrapper
                                      {
                                          id = b.Attribute(ATTRIBUTE_BROWSER_ID).Value,
                                          properties = (from prop in b.Descendants(ELEMENT_PROPERTY)
                                                        select new StringPair
                                                        {
                                                            key = prop.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                            value = prop.Attribute(ATTRIBUTE_PROPERTY_VALUE).Value,
                                                        }).ToArray<StringPair>(),
                                      }).ToArray<BrowserWrapper>();

            foreach (BrowserWrapper b in browserArray)
            {
                Browser browser = b.GetBrowser(vocabularyHolder);
                browsers.Add(b.id, browser);
            }
        }


        private class BrowserWrapper
        {
            public string id;
            public StringPair[] properties;

            public Browser GetBrowser(VocabularyHolder vocabularyHolder)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                
                if (vocabularyHolder != null)
                {
                    foreach (StringPair sp in properties)
                    {
                        try
                        {
                            //vocabularyHolder.ExistProperty(sp.key, ODDRService.ASPECT_WEB_BROWSER, ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI);
                            if (vocabularyHolder.ExistProperty(sp.key, ODDRService.ASPECT_WEB_BROWSER, ODDRVocabularyService.ODDR_LIMITED_VOCABULARY_IRI) != null)
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
                            //Console.WriteLine(this.GetType().FullName + " " + sp.key + " already present in device " + id);
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

                Browser b;
                b = new Browser(dic);

                return b;
            }
        }

        private class StringPair
        {
            public string key;
            public string value;
        }
    }
}
