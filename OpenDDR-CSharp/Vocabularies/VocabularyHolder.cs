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
using Oddr.Caches;
using Oddr.Models.Vocabularies;
using W3c.Ddr.Exceptions;
using System.Diagnostics;

namespace Oddr.Vocabularies
{
    public class VocabularyHolder
    {
        private Dictionary<String, Vocabulary> vocabularies = null;
        private ICache vocabularyPropertyCache = new Cache();

        public VocabularyHolder(Dictionary<String, Vocabulary> vocabularies)
        {
            this.vocabularies = new Dictionary<string,Vocabulary>(vocabularies);
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public void ExistVocabulary(String vocabularyIRI)
        {
            Vocabulary value = null;
            if (!(vocabularies.TryGetValue(vocabularyIRI, out value)))
            {
                throw new NameException(NameException.VOCABULARY_NOT_RECOGNIZED, "unknow \"" + vocabularyIRI + "\" vocabulary");
            }
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when an aspect or property or vocabulary is not recognized</exception>
        public VocabularyProperty ExistProperty(String propertyName, String aspect, String vocabularyIRI, bool throwsException)
        {
            String realAspect = aspect;
            VocabularyProperty vocabularyProperty = (VocabularyProperty)vocabularyPropertyCache.GetCachedElement(propertyName + aspect + vocabularyIRI);

            if (vocabularyProperty == null)
            {
                Vocabulary vocabulary = new Vocabulary();
                if (vocabularies.TryGetValue(vocabularyIRI, out vocabulary))
                {
                    Dictionary<String, VocabularyProperty> propertyMap = vocabulary.properties;

                    if (propertyMap.TryGetValue(propertyName, out vocabularyProperty))
                    {
                        if (realAspect != null && realAspect.Trim().Length > 0)
                        {
                            if (vocabularyProperty.aspects.Contains(realAspect))
                            {
                                vocabularyPropertyCache.SetCachedElement(propertyName + aspect + vocabularyIRI, vocabularyProperty);
                                return vocabularyProperty;

                            }
                            else
                            {
                                if (throwsException)
                                {
                                    throw new NameException(NameException.ASPECT_NOT_RECOGNIZED, "unknow \"" + realAspect + "\" aspect");
                                }
                                return null;
                            }

                        }
                        else
                        {
                            return vocabularyProperty;
                        }

                    }
                    else
                    {
                        if (throwsException)
                        {
                            throw new NameException(NameException.PROPERTY_NOT_RECOGNIZED, "unknow \"" + propertyName + "\" property");
                        }
                        return null;
                    }

                }
                else
                {
                    if (throwsException)
                    {
                        throw new NameException(NameException.VOCABULARY_NOT_RECOGNIZED, "unknow \"" + vocabularyIRI + "\" vacabulary");
                    }
                    return null;
                }

            }
            else
            {
                return vocabularyProperty;
            }
        }

        public VocabularyProperty ExistProperty(String propertyName, String aspect, String vocabularyIRI)
        {
            return ExistProperty(propertyName, aspect, vocabularyIRI, false);
        }

        public Dictionary<String, Vocabulary> GetVocabularies() {
            return vocabularies;
        }
    }
}
