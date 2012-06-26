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
    /// <summary>
    /// The Service interface is the core of the DDR Simple API
    /// </summary>
    /// <remarks>
    /// Using methods of Service the caller supplies Evidence representing the Delivery Context and an indication of the Properties of interest. These methods return PropertyValue objects which can then be queried to reveal the values of the Properties of interest.
    /// The Service may be instantiated by a supplied factory class.
    /// The class invokes the initialize method to establish a Default Vocabulary and to pass implementation specific settings.
    /// Whether or not the underlying implementation combines more than one source of data is opaque to the user of the API. The API makes no assumptions about the number of sources of data.
    /// All implementations are required to support Evidence consisting of name/value pairs of HTTP headers.
    /// The methods of the Service interface fall into the following categories: Factory Methods, Query Methods, Information Methods, Initialization.
    /// The "Factory" methods provide a means of instantiating objects that support the interfaces defined in the Recommendation that is consistent between implementations. Implementations may provide other means of instantiating the interfaces.
    /// Query methods return values for Properties of the Delivery Context represented by the supplied Evidence.
    /// Information methods return information about the implementation.
    /// Initialization method initialize the library.
    /// </remarks>
    public interface IService
    {   
        /// <summary>
        /// Initialize the Library. Called by the ServiceFactory Class to initialize the implementation. Implementation specific initialization parameters may be passed using the props parameter.
        /// </summary>
        /// <exception cref="NameException">Throws when...</exception>
        /// <exception cref="InitializationException">Throws when...</exception>
        void Initialize(String defaultVocabularyIRI, Properties prprts);

        /// <summary>
        /// Get Implementation Version.
        /// </summary>
        /// <returns>Returns information about the implementation of the API including the current version. This may be used for diagnostic purposes, particularly where the implementation language does not already provide a means for obtaining such information.</returns>
        String GetAPIVersion();

        /// <summary>
        /// Get Data Version
        /// </summary>
        /// <returns>Returns information about the underlying data (values for Properties) if the implementation has a versioning system for that information.</returns>
        String GetDataVersion();

        /// <summary>
        /// List Properties
        /// </summary>
        /// <returns>Lists the combination of all known Properties and Aspects in all Vocabularies that can be used without causing a NameException to be thrown. The order in which Properties are listed is not significant.</returns>
        IPropertyRef[] ListPropertyRefs();

        /// <summary>
        /// Return the value of a specific Property
        /// </summary>
        /// <param name="evdnc">The specified IEvidence</param>
        /// <param name="pr">The supplied Property</param>
        /// <returns>Return the value of a specific Property</returns>
        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValue GetPropertyValue(IEvidence evdnc, IPropertyRef pr);

        /// <summary>
        /// Return the value of a specific Property in its Default Aspect in the Vocabulary specified by propertyName
        /// </summary>
        /// <param name="evdnc">The specified IEvidence</param>
        /// <param name="pn">The IPropertyName for the property</param>
        /// <returns>Return the value of a specific Property in its Default Aspect in the Vocabulary specified by propertyName</returns>
        IPropertyValue GetPropertyValue(IEvidence evdnc, IPropertyName pn);

        /// <summary>
        /// Return the value of a specific Property in its Default Aspect in the Default Vocabulary
        /// </summary>
        /// <param name="evdnc">The specified IEvidence</param>
        /// <param name="localPropertyName">The name of the property</param>
        /// <returns>Return the value of a specific Property in its Default Aspect in the Default Vocabulary</returns>
        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValue GetPropertyValue(IEvidence evdnc, String localPropertyName);

        /// <summary>
        /// Return the value of a specific Property with a specific Aspect in a specific Vocabulary.
        /// </summary>
        /// <param name="evdnc">The specified IEvidence</param>
        /// <param name="localPropertyName">The name of the property</param>
        /// <param name="localAspectName">The aspect of the property</param>
        /// <param name="vocabularyIRI">The vocabulary of the property</param>
        /// <returns>Return the value of a specific Property with a specific Aspect in a specific Vocabulary.</returns>
        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValue GetPropertyValue(IEvidence evdnc, String localPropertyName, String localAspectName, String vocabularyIRI);

        /// <summary>
        /// Return all available Property values for all the Aspects and Vocabularies known by an implementation of the DDR Simple API
        /// </summary>
        /// <param name="evdnc">The specified IEvidence</param>
        /// <returns>Return all available Property values for all the Aspects and Vocabularies</returns>
        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValues GetPropertyValues(IEvidence evdnc);

        /// <summary>
        /// Return values for all the supplied Properties, returning empty values for those that are not known. An "unknown value" is distinguished by the PropertyValue exists() method returning false.
        /// </summary>
        /// <param name="evdnc">The specified IEvidence</param>
        /// <param name="prs">The supplied Properties</param>
        /// <returns>Return values for all the supplied Properties</returns>
        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValues GetPropertyValues(IEvidence evdnc, IPropertyRef[] prs);

        /// <summary>
        /// Return all known values for the given Aspect of the Default Vocabulary
        /// </summary>
        /// <param name="evdnc">The specified IEvidence</param>
        /// <param name="localAspectName">The specified Aspect</param>
        /// <returns>Return all known values for the given Aspect of the Default Vocabulary</returns>
        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValues GetPropertyValues(IEvidence evdnc, String localAspectName);

        /// <summary>
        /// Return all known values for an Aspect of a specified Vocabulary
        /// </summary>
        /// <param name="evdnc">The specified IEvidence</param>
        /// <param name="localAspectName">The specified Aspect</param>
        /// <param name="vocabularyIRI">The specified Namespace</param>
        /// <returns>Return all known values for an Aspect of a specified Vocabulary</returns>
        /// <exception cref="NameException">Throws when...</exception>
        IPropertyValues GetPropertyValues(IEvidence evdnc, String localAspectName, String vocabularyIRI);

        /// <summary>
        /// Create PropertyName using Default Vocabulary
        /// </summary>
        /// <param name="localPropertyName">The name of the property.</param>
        /// <returns>Return the IPropertyName</returns>
        /// <exception cref="NameException">Throws when the localPropertyName is not defined in the Default Vacabulary</exception>
        IPropertyName NewPropertyName(String localPropertyName);

        /// <summary>
        /// Create PropertyName with specified Vocabulary
        /// </summary>
        /// <param name="localPropertyName">The name of the property.</param>
        /// <param name="vocabularyIRI">The Vocabulary namespace.</param>
        /// <returns>Return the IPropertyName</returns>
        /// <exception cref="NameException">Throws when the localPropertyName is not defined in the Vacabulary identified by vocabularyIRI</exception>
        IPropertyName NewPropertyName(String localPropertyName, String vocabularyIRI);

        /// <summary>
        /// Create PropertyRef using Default Vocabulary and Aspect
        /// </summary>
        /// <param name="localPropertyName">The name of the property.</param>
        /// <returns>Return the IPropertyRef</returns>
        /// <exception cref="NameException">Throws when the localPropertyName is not defined in the Default Vacabulary</exception>
        IPropertyRef NewPropertyRef(String localPropertyName);

        /// <summary>
        /// Create PropertyRef from PropertyName using Default Aspect. The Aspect of the PropertyRef created is the default Aspect of the Property in the Vocabulary determined by the propertyName parameter.
        /// </summary>
        /// <param name="pn">The IPropertyName for the property</param>
        /// <returns>Return the IPropertyRef</returns>
        /// <exception cref="NameException">Throws when the pn is not defined</exception>
        IPropertyRef NewPropertyRef(IPropertyName pn);

        /// <summary>
        /// Create PropertyRef from PropertyName in Named Aspect. The namespace associated with the Aspect localAspectName is associated with the namespace of the propertyName parameter.
        /// </summary>
        /// <param name="pn">The IPropertyName for the property</param>
        /// <param name="localAspectName">The aspect of the property</param>
        /// <returns>Return the IPropertyRef</returns>
        /// <exception cref="NameException">Throws when the pn and the localAspectName are not defined</exception>
        IPropertyRef NewPropertyRef(IPropertyName pn, String localAspectName);

        /// <summary>
        /// Create Empty HTTP Evidence
        /// </summary>
        /// <returns>Return an empty Evidence</returns>
        IEvidence NewHTTPEvidence();

        /// <summary>
        /// Create HTTP Evidence from Map
        /// </summary>
        /// <param name="map">The Map parameter contains name/value pairs representing HTTP Header names and values.</param>
        /// <returns>Return an Evidence filled with map</returns>
        IEvidence NewHTTPEvidence(Dictionary<String, String> map);
    }
}
