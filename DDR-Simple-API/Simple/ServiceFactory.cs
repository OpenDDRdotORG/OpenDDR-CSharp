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
using W3c.Ddr.Exceptions;
using System.Reflection;

namespace W3c.Ddr.Simple
{
    /// <summary>
    /// Defines a factory for instantiating Service with the supplied default namespace and configuration.
    /// </summary>
    public class ServiceFactory
    {
        
        //public static IService newService(String clazz, String defaultVocabulary, Properties configuration)
        /// <summary>
        /// Instantiates an instance of the Type serviceType establishing the Default Vocabulary to be the one specified and with implementation specific values passed as Properties.
        /// </summary>
        /// <param name="serviceType">The interface implementation</param>
        /// <param name="defaultVocabulary">The default vocabulary</param>
        /// <param name="configuration">The Property</param>
        /// <returns>Return the service instance</returns>
        /// <exception cref="InitializationException">Throws when...</exception>
        /// <exception cref="NameException">Throws when...</exception>
        public static IService newService(Type serviceType, String defaultVocabulary, Properties configuration)
        {

		    IService theService = null;

            if (serviceType == null)
            {
                throw new W3c.Ddr.Exceptions.SystemException(W3c.Ddr.Exceptions.SystemException.ILLEGAL_ARGUMENT, "Service class cannot be null");
		    }

		    if (defaultVocabulary == null)
            {
                throw new W3c.Ddr.Exceptions.SystemException(W3c.Ddr.Exceptions.SystemException.ILLEGAL_ARGUMENT, "Default vocabulary cannot be null");
		    }

            try
            {
                // Instantiation
                //Type serviceType = Type.GetType(clazz, true);
                theService = (IService)Activator.CreateInstance(serviceType);
            }
            catch (TargetInvocationException e)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, e);
            }
            catch (ArgumentException e)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, e);
            }
            catch (Exception thr)
            {
                throw new W3c.Ddr.Exceptions.SystemException(W3c.Ddr.Exceptions.SystemException.CANNOT_PROCEED, thr);
            }

		    // Initialization
		    theService.Initialize(defaultVocabulary, configuration);

		    return theService;
	    }
    }
}
