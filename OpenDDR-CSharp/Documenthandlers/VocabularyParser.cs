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
using System.Xml.Linq;
using Oddr.Models.Vocabularies;
using System.IO;
using System.Xml;

namespace Oddr.Documenthandlers
{
    public class VocabularyParser
    {
        private const String ELEMENT_VOCABULARY_DESCRIPTION = "VocabularyDescription";
        private const String ELEMENT_ASPECTS = "Aspects";
        private const String ELEMENT_ASPECT = "Aspect";
        private const String ELEMENT_VARIABLES = "Variables";
        private const String ELEMENT_VARIABLE = "Variable";
        private const String ELEMENT_PROPERTIES = "Properties";
        private const String ELEMENT_PROPERTY = "Property";
        private const String ATTRIBUTE_PROPERTY_TARGET = "target";
        private const String ATTRIBUTE_PROPERTY_ASPECT_NAME = "name";
        private const String ATTRIBUTE_PROPERTY_ASPECT = "aspect";
        private const String ATTRIBUTE_PROPERTY_NAME = "name";
        private const String ATTRIBUTE_PROPERTY_VOCABULARY = "vocabulary";
        private const String ATTRIBUTE_PROPERTY_ID = "id";
        private const String ATTRIBUTE_PROPERTY_DATA_TYPE = "datatype";
        private const String ATTRIBUTE_PROPERTY_DATA_TYPE_CAMEL = "datatype";
        private const String ATTRIBUTE_PROPERTY_EXPR = "expr";
        private const String ATTRIBUTE_PROPERTY_ASPECTS = "aspects";
        private const String ATTRIBUTE_PROPERTY_DEFAULT_ASPECT = "defaultAspect";
        public Vocabulary vocabulary
        {
            private set;
            get;
        }
        private XDocument doc;

        /// <exception cref="ArgumentNullException">Thrown when...</exception>
        public VocabularyParser(Stream stream)
        {
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

            vocabulary = (from v in doc.Descendants(ELEMENT_VOCABULARY_DESCRIPTION)
                                     select new Vocabulary
                                     {
                                         vocabularyIRI = (string)v.Attribute(ATTRIBUTE_PROPERTY_TARGET).Value,
                                         aspects = (from aspects in v.Descendants(ELEMENT_ASPECTS)
                                                    select (string)aspects.Attribute(ELEMENT_ASPECT)).ToArray<string>(),
                                         properties = new Dictionary<string,VocabularyProperty>(),
                                         vocabularyVariables = new Dictionary<string,VocabularyVariable>(),
                                     }).First<Vocabulary>();

            XElement vocDescrXElement = doc.Descendants(ELEMENT_VOCABULARY_DESCRIPTION).First<XElement>();
            XElement propertiesXElement = vocDescrXElement.Descendants(ELEMENT_PROPERTIES).First<XElement>();

            VocabularyProperty[] vocabularyProperties = (from prop in propertiesXElement.Descendants(ELEMENT_PROPERTY)
                                                         //where prop.Attribute(ATTRIBUTE_PROPERTY_DATA_TYPE) != null
                                                         select new VocabularyProperty
                                                         {
                                                             aspects = prop.Attribute(ATTRIBUTE_PROPERTY_ASPECTS).Value.Replace(" ", "").Split(','),
                                                             defaultAspect = prop.Attribute(ATTRIBUTE_PROPERTY_DEFAULT_ASPECT).Value,
                                                             //expr = prop.Attribute(ATTRIBUTE_PROPERTY_EXPR).Value,
                                                             name = prop.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                             type = prop.Attribute(ATTRIBUTE_PROPERTY_DATA_TYPE).Value,
                                                         }).ToArray<VocabularyProperty>();

            foreach (VocabularyProperty vp in vocabularyProperties)
            {
                vocabulary.properties.Add(vp.name, vp);
            }


            try
            {
                XElement variablesXElement = vocDescrXElement.Descendants(ELEMENT_VARIABLES).First<XElement>();

                VocabularyVariable[] vocabularyVariables = (from var in variablesXElement.Descendants(ELEMENT_VARIABLE)
                                                            select new VocabularyVariable
                                                            {
                                                                aspect = var.Attribute(ATTRIBUTE_PROPERTY_ASPECT).Value,
                                                                id = var.Attribute(ATTRIBUTE_PROPERTY_ID).Value,
                                                                name = var.Attribute(ATTRIBUTE_PROPERTY_NAME).Value,
                                                                vocabulary = var.Attribute(ATTRIBUTE_PROPERTY_VOCABULARY).Value,
                                                            }).ToArray<VocabularyVariable>();

                foreach (VocabularyVariable vv in vocabularyVariables)
                {
                    vocabulary.vocabularyVariables.Add(vv.id, vv);
                }
            }
            catch (Exception ex)
            {
                //TODO: Handle this Exception
            }
        }
    }
}
