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
using Oddr.Vocabularies;
using W3c.Ddr.Models;
using Oddr.Models.Vocabularies;
using System.IO;
using W3c.Ddr.Exceptions;
using Oddr.Documenthandlers;

namespace Oddr
{
    class ODDRVocabularyService
    {
        public const String DDR_CORE_VOCABULARY_PATH_PROP = "ddr.vocabulary.core.path";
        public const String ODDR_VOCABULARY_PATH_PROP = "oddr.vocabulary.path";
        public const String ODDR_LIMITED_VOCABULARY_PATH_PROP = "oddr.limited.vocabulary.path";
        public const String DDR_CORE_VOCABULARY_STREAM_PROP = "ddr.vocabulary.core.stream";
        public const String ODDR_VOCABULARY_STREAM_PROP = "oddr.vocabulary.stream";
        public const String ODDR_LIMITED_VOCABULARY_STREAM_PROP = "oddr.limited.vocabulary.stream";
        public const String ODDR_LIMITED_VOCABULARY_IRI = "limitedVocabulary";
        public VocabularyHolder vocabularyHolder
        {
            private set;
            get;
        }

        /// <exception cref="InitializationException">Throws when...</exception>
        public void Initialize(Properties props)
        {
            Dictionary<String, Vocabulary> vocabularies = new Dictionary<String, Vocabulary>();

            String ddrCoreVocabularyPath = props.GetProperty(DDR_CORE_VOCABULARY_PATH_PROP);
            String oddrVocabularyPath = props.GetProperty(ODDR_VOCABULARY_PATH_PROP);

            Stream ddrCoreVocabulayStream = null;
            Stream[] oddrVocabularyStream = null;
            try {
                ddrCoreVocabulayStream = props.Get(DDR_CORE_VOCABULARY_STREAM_PROP) as Stream;
            } catch (Exception ex) {
                ddrCoreVocabulayStream = null;
            }
            try {
                oddrVocabularyStream = props.Get(ODDR_VOCABULARY_STREAM_PROP) as Stream[];
            } catch (Exception ex) {
                oddrVocabularyStream = null;
            }

            if ((string.IsNullOrEmpty(ddrCoreVocabularyPath)) && ddrCoreVocabulayStream == null) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not find property " + DDR_CORE_VOCABULARY_PATH_PROP));
            }

            if ((string.IsNullOrEmpty(oddrVocabularyPath)) && oddrVocabularyStream == null) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not find property " + ODDR_VOCABULARY_PATH_PROP));
            }

            VocabularyParser vocabularyParser = null;
            Vocabulary vocabulary = null;

            if (ddrCoreVocabulayStream != null)
            {
                vocabularyParser = ParseVocabularyFromStream(DDR_CORE_VOCABULARY_STREAM_PROP, ddrCoreVocabulayStream);
            }
            else
            {
                vocabularyParser = ParseVocabularyFromPath(DDR_CORE_VOCABULARY_PATH_PROP, ddrCoreVocabularyPath);
            }
            vocabulary = vocabularyParser.vocabulary;
            vocabularies.Add(vocabulary.vocabularyIRI, vocabulary);

            if (oddrVocabularyStream != null)
            {
                foreach (Stream stream in oddrVocabularyStream)
                {
                    vocabularyParser = ParseVocabularyFromStream(ODDR_VOCABULARY_STREAM_PROP, stream);
                    vocabulary = vocabularyParser.vocabulary;
                    vocabularies.Add(vocabulary.vocabularyIRI, vocabulary);
                }
            }
            else
            {
                String[] oddrVocabularyPaths = oddrVocabularyPath.Split(",".ToCharArray());
                foreach (string p in oddrVocabularyPaths)
                {
                    p.Trim();
                }

                foreach (String oddVocabularyString in oddrVocabularyPaths)
                {
                    vocabularyParser = ParseVocabularyFromPath(ODDR_VOCABULARY_PATH_PROP, oddVocabularyString);
                    vocabulary = vocabularyParser.vocabulary;
                    vocabularies.Add(vocabulary.vocabularyIRI, vocabulary);
                }
            }

            String oddrLimitedVocabularyPath = props.GetProperty(ODDR_LIMITED_VOCABULARY_PATH_PROP);
            Stream oddrLimitedVocabularyStream = props.Get(ODDR_LIMITED_VOCABULARY_STREAM_PROP) as Stream;

            if (oddrLimitedVocabularyStream != null) {
                vocabularyParser = ParseVocabularyFromStream(ODDR_LIMITED_VOCABULARY_STREAM_PROP, oddrLimitedVocabularyStream);

            } else {
               if (!string.IsNullOrEmpty(oddrLimitedVocabularyPath)) {
                   vocabularyParser = ParseVocabularyFromPath(ODDR_LIMITED_VOCABULARY_PATH_PROP, oddrLimitedVocabularyPath);
                }
            }
            vocabulary = vocabularyParser.vocabulary;
            vocabularies.Add(ODDR_LIMITED_VOCABULARY_IRI, vocabulary);

            vocabularyHolder = new VocabularyHolder(vocabularies);

            vocabularyParser = null;
            vocabularies = null;

        }

        /// <exception cref="InitializationException">Throws when...</exception>
        private VocabularyParser ParseVocabularyFromPath(String prop, String path)
        {
            VocabularyParser vocabularyParser;
            FileStream stream = null;

            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            }
            catch (IOException ex) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not open " + prop + " : " + path));
            }

            try
            {
                vocabularyParser = new VocabularyParser(stream);

            }
            catch (ArgumentNullException ex) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new InvalidOperationException("Can not instantiate VocabularyParser(Stream stream)"));

            }

            try
            {
                vocabularyParser.Parse();

            }
            catch (Exception ex) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse document: " + path));
            }

            stream.Close();
            return vocabularyParser;
        }

        /// <exception cref="InitializationException">Throws when...</exception>
        private VocabularyParser ParseVocabularyFromStream(String prop, Stream inputStream)
        {
            VocabularyParser vocabularyParser;
            try
            {
                vocabularyParser = new VocabularyParser(inputStream);
            }
            catch (ArgumentNullException ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new InvalidOperationException("Can not instantiate VocabularyParser(Stream stream)"));

            }

            try
            {
                vocabularyParser.Parse();

            }
            catch (Exception ex) {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse document in property: " + prop));

            }

            inputStream.Close();
            return vocabularyParser;
        }
    }
}
