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
using W3c.Ddr.Models;

namespace W3c.Ddr.Simple
{
    public interface IService
    {
        /// <exception cref="NameException">Throws when...</exception>
        /// <exception cref="InitializationException">Throws when...</exception>
        void Initialize(String defaultVocabularyIRI, Properties prprts);

        String GetAPIVersion();

        String GetDataVersion();

        IPropertyRef[] ListPropertyRefs();

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValue GetPropertyValue(IEvidence evdnc, IPropertyRef pr);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValue GetPropertyValue(IEvidence evdnc, IPropertyName pn);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValue GetPropertyValue(IEvidence evdnc, String localPropertyName);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValue GetPropertyValue(IEvidence evdnc, String localPropertyName, String localAspectName, String vocabularyIRI);

        IPropertyValues GetPropertyValues(IEvidence evdnc);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValues GetPropertyValues(IEvidence evdnc, IPropertyRef[] prs);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValues GetPropertyValues(IEvidence evdnc, String localAspectName);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValues GetPropertyValues(IEvidence evdnc, String localAspectName, String vocabularyIRI);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyName NewPropertyName(String localPropertyName);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyName NewPropertyName(String localPropertyName, String vocabularyIRI);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyRef NewPropertyRef(String localPropertyName);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyRef NewPropertyRef(IPropertyName pn);

        /// <exception cref="NameException">Throws when...</exception>
        IPropertyRef NewPropertyRef(IPropertyName pn, String localAspectName);

        IEvidence NewHTTPEvidence();

        IEvidence NewHTTPEvidence(Dictionary<String, String> map);
    }
}
