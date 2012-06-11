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
using W3c.Ddr.Simple;
using Oddr.Identificators;
using Oddr.Vocabularies;
using System.Text.RegularExpressions;
using log4net;
using W3c.Ddr.Models;
using W3c.Ddr.Exceptions;
using System.IO;
using Oddr.Models.Devices;
using Oddr.Documenthandlers;
using Oddr.Builders.Devices;
using Oddr.Builders;
using Oddr.Builders.Browsers;
using Oddr.Builders.OS;
using Oddr.Models.Browsers;
using OSModel = Oddr.Models.OS;
using Oddr.Models;
using Oddr.Models.Vocabularies;

namespace Oddr
{
    public class ODDRService : IService
    {
        public const String ASPECT_DEVICE = "device";
        public const String ASPECT_WEB_BROWSER = "webBrowser";
        public const String ASPECT_OPERATIVE_SYSTEM = "operativeSystem";
        public const String ASPECT_GROUP = "group";
        public const String ODDR_UA_DEVICE_BUILDER_PATH_PROP = "oddr.ua.device.builder.path";
        public const String ODDR_UA_DEVICE_DATASOURCE_PATH_PROP = "oddr.ua.device.datasource.path";
        public const String ODDR_UA_DEVICE_BUILDER_PATCH_PATHS_PROP = "oddr.ua.device.builder.patch.paths";
        public const String ODDR_UA_DEVICE_DATASOURCE_PATCH_PATHS_PROP = "oddr.ua.device.datasource.patch.paths";
        public const String ODDR_UA_BROWSER_DATASOURCE_PATH_PROP = "oddr.ua.browser.datasource.path";
        public const String ODDR_UA_OPERATINGSYSTEM_DATASOURCE_PATH_PROP = "oddr.ua.operatingSystem.datasource.path";
        public const String ODDR_UA_DEVICE_BUILDER_STREAM_PROP = "oddr.ua.device.builder.stream";
        public const String ODDR_UA_DEVICE_DATASOURCE_STREAM_PROP = "oddr.ua.device.datasource.stream";
        public const String ODDR_UA_DEVICE_BUILDER_PATCH_STREAMS_PROP = "oddr.ua.device.builder.patch.streams";
        public const String ODDR_UA_DEVICE_DATASOURCE_PATCH_STREAMS_PROP = "oddr.ua.device.datasource.patch.streams";
        public const String ODDR_UA_BROWSER_DATASOURCE_STREAM_PROP = "oddr.ua.browser.datasource.stream";
        public const String ODDR_UA_OPERATINGSYSTEM_DATASOURCE_STREAM_PROP = "oddr.ua.operatingSystem.datasource.stream";
        public const String ODDR_THRESHOLD_PROP = "oddr.threshold";
        public const String ODDR_VOCABULARY_IRI = "oddr.vocabulary.device";
        private const String ODDR_API_VERSION = "1.0.0";
        private const String ODDR_DATA_VERSION = "2012";
        private const int ODDR_DEFAULT_THRESHOLD = 70;
        private String defaultVocabularyIRI = null;
        private DeviceIdentificator deviceIdentificator = null;
        private BrowserIdentificator browserIdentificator = null;
        private OSIdentificator osIdentificator = null;
        private VocabularyHolder vocabularyHolder = null;
        private int threshold = ODDR_DEFAULT_THRESHOLD;
        private const String GROUP_REGEXPR = "\\$([^ ]+)";
        private Regex groupRegexprRegex = new Regex(GROUP_REGEXPR, RegexOptions.Compiled);
        protected static readonly ILog logger = LogManager.GetLogger(typeof(ODDRService));


        public string GetAPIVersion()
        {
            return ODDR_API_VERSION;
        }

        public string GetDataVersion()
        {
            return ODDR_DATA_VERSION;
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyValue GetPropertyValue(IEvidence evdnc, string localPropertyName, string localAspectName, string vocabularyIRI)
        {
            return GetPropertyValue(evdnc, NewPropertyRef(NewPropertyName(localPropertyName, vocabularyIRI), localAspectName));
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyValue GetPropertyValue(IEvidence evdnc, string localPropertyName)
        {
            return GetPropertyValue(evdnc, NewPropertyName(localPropertyName));
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyValue GetPropertyValue(IEvidence evdnc, IPropertyName pn)
        {
            return GetPropertyValue(evdnc, NewPropertyRef(pn));
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyValue GetPropertyValue(IEvidence evdnc, IPropertyRef pr)
        {
            return GetPropertyValues(evdnc, new IPropertyRef[] { pr }).GetValue(pr);
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyValues GetPropertyValues(IEvidence evdnc, string localAspectName, string vocabularyIRI)
        {
            VocabularyProperty vocabularyProperty = vocabularyHolder.ExistProperty(localAspectName, null, vocabularyIRI, true);            

            IPropertyName propertyName = new ODDRPropertyName(localAspectName, vocabularyIRI);
            IPropertyRef propertyRef = new ODDRPropertyRef(propertyName, vocabularyProperty.defaultAspect);

            return GetPropertyValues(evdnc, new IPropertyRef[] { propertyRef });
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyValues GetPropertyValues(IEvidence evdnc, string localAspectName)
        {
            return GetPropertyValues(evdnc, localAspectName, defaultVocabularyIRI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evdnc"></param>
        /// <param name="prs"></param>
        /// <returns></returns>
        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyValues GetPropertyValues(IEvidence evdnc, IPropertyRef[] prs)
        {
            Device deviceFound = null;
            Browser browserFound = null;
            OSModel.OperatingSystem osFound = null;
            bool deviceIdentified = false;
            bool browserIdentified = false;
            bool osIdentified = false;
            UserAgent deviceUA = null;
            UserAgent browserUA = null;

            ODDRPropertyValues ret = new ODDRPropertyValues();
            Dictionary<String, Vocabulary> vocabularies = vocabularyHolder.GetVocabularies();

            foreach (IPropertyRef propertyRef in prs)
            {
                VocabularyProperty vocabularyProperty = vocabularyHolder.ExistProperty(propertyRef.LocalPropertyName(), propertyRef.AspectName(), propertyRef.Namespace(), true);

                Vocabulary vocabulary = null;
                string nameSpace = propertyRef.Namespace();
                if (vocabularies.TryGetValue(nameSpace, out vocabulary))
                {
                    if (ASPECT_DEVICE.Equals(propertyRef.AspectName()))
                    {
                        if (!deviceIdentified)
                        {
                            if (deviceUA == null)
                            {
                                deviceUA = UserAgentFactory.newDeviceUserAgent(evdnc);
                            }
                            if (evdnc is BufferedODDRHTTPEvidence)
                            {
                                deviceFound = ((BufferedODDRHTTPEvidence)evdnc).deviceFound;
                            }
                            if (deviceFound == null)
                            {
                                deviceFound = deviceIdentificator.Get(deviceUA, this.threshold) as Device;
                            }
                            if (evdnc is BufferedODDRHTTPEvidence)
                            {
                                ((BufferedODDRHTTPEvidence)evdnc).deviceFound = deviceFound;
                            }

                            deviceIdentified = true;
                        }
                        String property = null;

                        if (deviceFound != null)
                        {
                            property = deviceFound.Get(propertyRef.LocalPropertyName());
                            ret.addProperty(new ODDRPropertyValue(property, vocabularyProperty.type, propertyRef));

                        }
                        else
                        {
                            ret.addProperty(new ODDRPropertyValue(null, vocabularyProperty.type, propertyRef));
                        }
                        continue;

                    }
                    else if (ASPECT_WEB_BROWSER.Equals(propertyRef.AspectName()))
                    {
                        //TODO: evaluate ua-pixels header in evidence
                        if (!browserIdentified)
                        {
                            if (browserUA == null)
                            {
                                browserUA = UserAgentFactory.newBrowserUserAgent(evdnc);
                            }
                            if (evdnc is BufferedODDRHTTPEvidence)
                            {
                                browserFound = ((BufferedODDRHTTPEvidence)evdnc).browserFound;
                            }
                            if (browserFound == null)
                            {
                                browserFound = browserIdentificator.Get(browserUA, this.threshold) as Browser;
                            }
                            if (evdnc is BufferedODDRHTTPEvidence)
                            {
                                ((BufferedODDRHTTPEvidence)evdnc).browserFound = browserFound;
                            }
                            browserIdentified = true;
                        }
                        String property = null;
                        if (browserFound != null)
                        {
                            property = browserFound.Get(propertyRef.LocalPropertyName());
                            ret.addProperty(new ODDRPropertyValue(property, vocabularyProperty.type, propertyRef));

                        }
                        else
                        {
                            ret.addProperty(new ODDRPropertyValue(null, vocabularyProperty.type, propertyRef));
                        }
                        continue;

                    }
                    else if (ASPECT_OPERATIVE_SYSTEM.Equals(propertyRef.AspectName()))
                    {
                        //TODO: evaluate ua-os header in evidence
                        if (!osIdentified)
                        {
                            if (deviceUA == null)
                            {
                                deviceUA = UserAgentFactory.newDeviceUserAgent(evdnc);
                            }
                            if (evdnc is BufferedODDRHTTPEvidence)
                            {
                                osFound = ((BufferedODDRHTTPEvidence)evdnc).osFound;
                            }
                            if (osFound == null)
                            {
                                osFound = osIdentificator.Get(deviceUA, this.threshold) as OSModel.OperatingSystem;
                            }
                            if (evdnc is BufferedODDRHTTPEvidence)
                            {
                                ((BufferedODDRHTTPEvidence)evdnc).osFound = osFound;
                            }
                            osIdentified = true;
                        }
                        String property = null;
                        if (osFound != null)
                        {
                            property = osFound.Get(propertyRef.LocalPropertyName());
                            ret.addProperty(new ODDRPropertyValue(property, vocabularyProperty.type, propertyRef));

                        }
                        else
                        {
                            ret.addProperty(new ODDRPropertyValue(null, vocabularyProperty.type, propertyRef));
                        }
                        continue;
                    }
                }

            }

            return ret;
        }

        public IPropertyValues GetPropertyValues(IEvidence evdnc)
        {
            Device deviceFound = null;
            Browser browserFound = null;
            OSModel.OperatingSystem osFound = null;
            bool deviceIdentified = false;
            bool browserIdentified = false;
            bool osIdentified = false;
            UserAgent deviceUA = null;
            UserAgent browserUA = null;

            ODDRPropertyValues ret = new ODDRPropertyValues();
            Dictionary<String, Vocabulary> vocabularies = vocabularyHolder.GetVocabularies();

            foreach (String vocabularyKey in vocabularies.Keys)
            {
                Vocabulary vocabulary = vocabularies[vocabularyKey];
                Dictionary<String, VocabularyProperty> properties = vocabulary.properties;

                foreach (String propertyKey in properties.Keys)
                {
                    IPropertyName propertyName = new ODDRPropertyName(propertyKey, vocabularyKey);

                    VocabularyProperty vocabularyProperty = properties[propertyKey];
                    string[] aspects = vocabularyProperty.aspects;

                    for (int i = 0; i < aspects.Length; i++)
                    {
                        IPropertyRef propertyRef = new ODDRPropertyRef(propertyName, aspects[i]);
                        if (ASPECT_DEVICE.Equals(propertyRef.AspectName()))
                        {
                            if (!deviceIdentified)
                            {
                                if (deviceUA == null)
                                {
                                    deviceUA = UserAgentFactory.newDeviceUserAgent(evdnc);
                                }
                                if (evdnc is BufferedODDRHTTPEvidence)
                                {
                                    deviceFound = ((BufferedODDRHTTPEvidence)evdnc).deviceFound;
                                }
                                if (deviceFound == null)
                                {
                                    deviceFound = deviceIdentificator.Get(deviceUA, this.threshold) as Device;
                                }
                                if (evdnc is BufferedODDRHTTPEvidence)
                                {
                                    ((BufferedODDRHTTPEvidence)evdnc).deviceFound = deviceFound;
                                }
                                deviceIdentified = true;
                            }
                            String property = null;
                            if (deviceFound != null)
                            {
                                property = deviceFound.Get(propertyRef.LocalPropertyName());
                                ret.addProperty(new ODDRPropertyValue(property, vocabularyProperty.type, propertyRef));

                            }
                            else
                            {
                                ret.addProperty(new ODDRPropertyValue(null, vocabularyProperty.type, propertyRef));
                            }
                            continue;

                        }
                        else if (ASPECT_WEB_BROWSER.Equals(propertyRef.AspectName()))
                        {
                            if (!browserIdentified)
                            {
                                if (browserUA == null)
                                {
                                    browserUA = UserAgentFactory.newBrowserUserAgent(evdnc);
                                }
                                if (evdnc is BufferedODDRHTTPEvidence)
                                {
                                    browserFound = ((BufferedODDRHTTPEvidence)evdnc).browserFound;
                                }
                                if (browserFound == null)
                                {
                                    browserFound = browserIdentificator.Get(browserUA, this.threshold) as Browser;
                                }
                                if (evdnc is BufferedODDRHTTPEvidence)
                                {
                                    ((BufferedODDRHTTPEvidence)evdnc).browserFound = browserFound;
                                }

                                browserIdentified = true;
                            }
                            String property = null;
                            if (browserFound != null)
                            {
                                property = browserFound.Get(propertyRef.LocalPropertyName());
                                ret.addProperty(new ODDRPropertyValue(property, vocabularyProperty.type, propertyRef));

                            }
                            else
                            {
                                ret.addProperty(new ODDRPropertyValue(null, vocabularyProperty.type, propertyRef));
                            }
                            continue;

                        }
                        else if (ASPECT_OPERATIVE_SYSTEM.Equals(propertyRef.AspectName()))
                        {
                            if (!osIdentified)
                            {
                                if (deviceUA == null)
                                {
                                    deviceUA = UserAgentFactory.newDeviceUserAgent(evdnc);
                                }
                                if (evdnc is BufferedODDRHTTPEvidence)
                                {
                                    osFound = ((BufferedODDRHTTPEvidence)evdnc).osFound;
                                }
                                if (osFound == null)
                                {
                                    osFound = osIdentificator.Get(deviceUA, this.threshold) as OSModel.OperatingSystem;
                                }
                                if (evdnc is BufferedODDRHTTPEvidence)
                                {
                                    ((BufferedODDRHTTPEvidence)evdnc).osFound = osFound;
                                }

                                osIdentified = true;
                            }
                            String property = null;
                            if (osFound != null)
                            {
                                property = osFound.Get(propertyRef.LocalPropertyName());
                                ret.addProperty(new ODDRPropertyValue(property, vocabularyProperty.type, propertyRef));

                            }
                            else
                            {
                                ret.addProperty(new ODDRPropertyValue(null, vocabularyProperty.type, propertyRef));
                            }
                            continue;

                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultVocabularyIRI"></param>
        /// <param name="prprts"></param>
        public void Initialize(string defaultVocabularyIRI, Properties prprts)
        {
            if (defaultVocabularyIRI == null || defaultVocabularyIRI.Trim().Length == 0)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new NullReferenceException("defaultVocabularyIRI can not be null"));
            }

            /*Initializing VocabularyHolder*/
            ODDRVocabularyService oddrVocabularyService = new ODDRVocabularyService();
            oddrVocabularyService.Initialize(prprts);

            vocabularyHolder = oddrVocabularyService.vocabularyHolder;
            vocabularyHolder.ExistVocabulary(defaultVocabularyIRI);

            String oddrUaDeviceBuilderPath = prprts.GetProperty(ODDR_UA_DEVICE_BUILDER_PATH_PROP);
            String oddrUaDeviceDatasourcePath = prprts.GetProperty(ODDR_UA_DEVICE_DATASOURCE_PATH_PROP);
            String oddrUaDeviceBuilderPatchPaths = prprts.GetProperty(ODDR_UA_DEVICE_BUILDER_PATCH_PATHS_PROP);
            String oddrUaDeviceDatasourcePatchPaths = prprts.GetProperty(ODDR_UA_DEVICE_DATASOURCE_PATCH_PATHS_PROP);
            String oddrUaBrowserDatasourcePaths = prprts.GetProperty(ODDR_UA_BROWSER_DATASOURCE_PATH_PROP);
            String oddrUaOperatingSystemDatasourcePaths = prprts.GetProperty(ODDR_UA_OPERATINGSYSTEM_DATASOURCE_PATH_PROP);

            Stream oddrUaDeviceBuilderStream = null;
            Stream oddrUaDeviceDatasourceStream = null;
            Stream[] oddrUaDeviceBuilderPatchStreams = null;
            Stream[] oddrUaDeviceDatasourcePatchStreams = null;
            Stream oddrUaBrowserDatasourceStream = null;
            Stream oddrUaOperatingSystemDatasourceStream = null;

            try
            {
                oddrUaDeviceBuilderStream = (Stream)prprts.Get(ODDR_UA_DEVICE_BUILDER_STREAM_PROP);
            }
            catch (Exception ex)
            {
                oddrUaDeviceBuilderStream = null;
            }
            try
            {
                oddrUaDeviceDatasourceStream = (Stream)prprts.Get(ODDR_UA_DEVICE_DATASOURCE_STREAM_PROP);
            }
            catch (Exception ex)
            {
                oddrUaDeviceDatasourceStream = null;
            }
            try
            {
                oddrUaDeviceBuilderPatchStreams = (Stream[])prprts.Get(ODDR_UA_DEVICE_BUILDER_PATCH_STREAMS_PROP);
            }
            catch (Exception ex)
            {
                oddrUaDeviceBuilderPatchStreams = null;
            }
            try
            {
                oddrUaDeviceDatasourcePatchStreams = (Stream[])prprts.Get(ODDR_UA_DEVICE_DATASOURCE_PATCH_STREAMS_PROP);
            }
            catch (Exception ex)
            {
                oddrUaDeviceDatasourcePatchStreams = null;
            }
            try
            {
                oddrUaBrowserDatasourceStream = (Stream)prprts.Get(ODDR_UA_BROWSER_DATASOURCE_STREAM_PROP);
            }
            catch (Exception ex)
            {
                oddrUaBrowserDatasourceStream = null;
            }
            try
            {
                oddrUaOperatingSystemDatasourceStream = (Stream)prprts.Get(ODDR_UA_OPERATINGSYSTEM_DATASOURCE_STREAM_PROP);
            }
            catch (Exception ex)
            {
                oddrUaOperatingSystemDatasourceStream = null;
            }

            String oddrThreshold = prprts.GetProperty(ODDR_THRESHOLD_PROP);

            if ((oddrUaDeviceBuilderPath == null || oddrUaDeviceBuilderPath.Trim().Length == 0) && oddrUaDeviceBuilderStream == null)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not find property " + ODDR_UA_DEVICE_BUILDER_PATH_PROP));
            }

            if ((oddrUaDeviceDatasourcePath == null || oddrUaDeviceDatasourcePath.Trim().Length == 0) && oddrUaDeviceDatasourceStream == null)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not find property " + ODDR_UA_DEVICE_DATASOURCE_PATH_PROP));
            }

            String[] oddrUaDeviceBuilderPatchPathArray = null;

            if (oddrUaDeviceBuilderPatchPaths != null && oddrUaDeviceBuilderPatchPaths.Trim().Length != 0)
            {
                oddrUaDeviceBuilderPatchPathArray = oddrUaDeviceBuilderPatchPaths.Split(",".ToCharArray());

            }
            else
            {
                oddrUaDeviceBuilderPatchPathArray = new String[0];
            }

            String[] ooddrUaDeviceDatasourcePatchPathArray = null;

            if (oddrUaDeviceDatasourcePatchPaths != null && oddrUaDeviceDatasourcePatchPaths.Trim().Length != 0)
            {
                ooddrUaDeviceDatasourcePatchPathArray = oddrUaDeviceDatasourcePatchPaths.Split(",".ToCharArray());

            }
            else
            {
                ooddrUaDeviceDatasourcePatchPathArray = new String[0];
            }

            if (oddrUaDeviceBuilderPatchStreams == null)
            {
                oddrUaDeviceBuilderPatchStreams = new Stream[0];
            }

            if (oddrUaDeviceDatasourcePatchStreams == null)
            {
                oddrUaDeviceDatasourcePatchStreams = new Stream[0];
            }

            if ((oddrUaBrowserDatasourcePaths == null || oddrUaBrowserDatasourcePaths.Trim().Length == 0) && oddrUaBrowserDatasourceStream == null)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not find property " + ODDR_UA_BROWSER_DATASOURCE_PATH_PROP));
            }

            if ((oddrUaOperatingSystemDatasourcePaths == null || oddrUaOperatingSystemDatasourcePaths.Trim().Length == 0) && oddrUaOperatingSystemDatasourceStream == null)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not find property " + ODDR_UA_OPERATINGSYSTEM_DATASOURCE_PATH_PROP));
            }

            if (oddrThreshold == null || oddrThreshold.Trim().Length == 0)
            {
                this.threshold = ODDR_DEFAULT_THRESHOLD;

            }
            else
            {
                try
                {
                    this.threshold = int.Parse(oddrThreshold);
                    if (this.threshold <= 0)
                    {
                        this.threshold = ODDR_DEFAULT_THRESHOLD;
                    }

                }
                catch (FormatException x)
                {
                    this.threshold = ODDR_DEFAULT_THRESHOLD;
                }
            }

            Dictionary<String, Device> devices = new Dictionary<String, Device>();


            Stream stream = null;

            try
            {
                if (oddrUaDeviceDatasourceStream != null)
                {
                    stream = oddrUaDeviceDatasourceStream;
                }
                else
                {
                    stream = new FileStream(oddrUaDeviceDatasourcePath, FileMode.Open);
                }

            }
            catch (IOException ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not open " + ODDR_UA_DEVICE_DATASOURCE_PATH_PROP + " " + oddrUaDeviceDatasourcePath));
            }

            /// TODO: Check stream. If stream is null DeviceDataSourceParser throws ArgumentNullException
            DeviceDatasourceParser deviceDatasourceParser = new DeviceDatasourceParser(stream, devices, vocabularyHolder);

            try
            {
                deviceDatasourceParser.Parse();
            }
            catch (Exception ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse " + ODDR_UA_DEVICE_DATASOURCE_PATH_PROP + " :" + oddrUaDeviceDatasourcePath));
            }

            try
            {
                stream.Close();
            }
            catch (IOException ex)
            {
                logger.Warn("", ex);
            }


            deviceDatasourceParser.patching = true;

            if (oddrUaDeviceDatasourcePatchStreams != null && oddrUaDeviceDatasourcePatchStreams.Length != 0)
            {
                for (int i = 0; i < oddrUaDeviceDatasourcePatchStreams.Length; i++)
                {
                    stream = oddrUaDeviceDatasourcePatchStreams[i];

                    try
                    {
                        deviceDatasourceParser.SetStream(stream);
                        deviceDatasourceParser.Parse();

                    }
                    catch (ArgumentNullException ane)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not open DeviceDatasource input stream " + i));
                    }
                    catch (Exception ex)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse DeviceDatasource input stream " + i));
                    }

                    try
                    {
                        stream.Close();

                    }
                    catch (IOException ex)
                    {
                        logger.Warn("", ex);
                    }
                }
            }
            else
            {
                for (int i = 0; i < ooddrUaDeviceDatasourcePatchPathArray.Length; i++)
                {
                    try
                    {
                        stream = new FileStream(ooddrUaDeviceDatasourcePatchPathArray[i], FileMode.Open);
                    }
                    catch (IOException ex)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not open " + ODDR_UA_DEVICE_DATASOURCE_PATH_PROP + " " + ooddrUaDeviceDatasourcePatchPathArray[i]));
                    }

                    try
                    {
                        deviceDatasourceParser.SetStream(stream);
                        deviceDatasourceParser.Parse();
                    }
                    catch (ArgumentNullException ane)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not open " + ODDR_UA_DEVICE_DATASOURCE_PATH_PROP + " :" + ooddrUaDeviceDatasourcePatchPathArray[i]));
                    }
                    catch (Exception ex)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse document: " + ooddrUaDeviceDatasourcePatchPathArray[i]));
                    }

                    try
                    {
                        stream.Close();
                    }
                    catch (IOException ex)
                    {
                        logger.Warn("", ex);
                    }
                }

            }

            List<IDeviceBuilder> builders = new List<IDeviceBuilder>();

            try
            {
                if (oddrUaDeviceBuilderStream != null)
                {
                    stream = oddrUaDeviceBuilderStream;
                }
                else
                {
                    stream = new FileStream(oddrUaDeviceBuilderPath, FileMode.Open);
                }

            }
            catch (IOException ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not open " + ODDR_UA_DEVICE_BUILDER_PATH_PROP + " " + oddrUaDeviceBuilderPath));
            }

            /// TODO: Check stream. If stream is null DeviceBuilderParser throws ArgumentNullException
            DeviceBuilderParser deviceBuilderParser = new DeviceBuilderParser(stream, builders);

            try
            {
                deviceBuilderParser.Parse();
            }
            catch (Exception ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse " + ODDR_UA_DEVICE_DATASOURCE_PATH_PROP + " :" + oddrUaDeviceBuilderPath));
            }

            try
            {
                stream.Close();

            }
            catch (IOException ex)
            {
                logger.Warn("", ex);
            }

            if (oddrUaDeviceBuilderPatchStreams != null && oddrUaDeviceBuilderPatchStreams.Length != 0)
            {
                for (int i = 0; i < oddrUaDeviceBuilderPatchStreams.Length; i++)
                {
                    stream = oddrUaDeviceBuilderPatchStreams[i];

                    try
                    {
                        deviceBuilderParser.SetStream(stream);
                        deviceBuilderParser.Parse();

                    }
                    catch (ArgumentNullException ane)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not open DeviceBuilder input stream " + i));
                    }
                    catch (Exception ex)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse DeviceBuilder input stream " + i));
                    }

                    try
                    {
                        stream.Close();
                    }
                    catch (IOException ex)
                    {
                        logger.Warn("", ex);
                    }
                }

            }
            else
            {
                for (int i = 0; i < oddrUaDeviceBuilderPatchPathArray.Length; i++)
                {
                    try
                    {
                        stream = new FileStream(oddrUaDeviceBuilderPatchPathArray[i], FileMode.Open);

                    }
                    catch (IOException ex)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not open " + ODDR_UA_DEVICE_BUILDER_PATCH_PATHS_PROP + " " + oddrUaDeviceBuilderPatchPathArray[i]));
                    }

                    try
                    {
                        deviceBuilderParser.SetStream(stream);
                        deviceBuilderParser.Parse();
                    }
                    catch (ArgumentNullException ane)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not open " + ODDR_UA_DEVICE_DATASOURCE_PATH_PROP + " :" + oddrUaDeviceBuilderPatchPathArray[i]));
                    }
                    catch (Exception ex)
                    {
                        throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse document: " + oddrUaDeviceBuilderPatchPathArray[i]));
                    }


                    try
                    {
                        stream.Close();

                    }
                    catch (IOException ex)
                    {
                        logger.Warn("", ex);
                    }
                }
            }

            try
            {
                if (oddrUaBrowserDatasourceStream != null)
                {
                    stream = oddrUaBrowserDatasourceStream;
                }
                else
                {
                    stream = new FileStream(oddrUaBrowserDatasourcePaths, FileMode.Open);
                }

            }
            catch (IOException ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not open " + ODDR_UA_BROWSER_DATASOURCE_PATH_PROP + " " + oddrUaBrowserDatasourcePaths));
            }

            /// TODO: Check stream. If stream is null BrowserDatasourceParser throws ArgumentNullException
            BrowserDatasourceParser browserDatasourceParser = new BrowserDatasourceParser(stream, vocabularyHolder);

            try
            {
                browserDatasourceParser.Parse();
            }
            catch (Exception ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse document: " + oddrUaBrowserDatasourcePaths));
            }

            try
            {
                stream.Close();
            }
            catch (IOException ex)
            {
                logger.Warn("", ex);
            }

            try
            {
                if (oddrUaOperatingSystemDatasourceStream != null)
                {
                    stream = oddrUaOperatingSystemDatasourceStream;
                }
                else
                {
                    stream = new FileStream(oddrUaOperatingSystemDatasourcePaths, FileMode.Open);
                }

            }
            catch (IOException ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new ArgumentException("Can not open " + ODDR_UA_OPERATINGSYSTEM_DATASOURCE_PATH_PROP + " " + oddrUaOperatingSystemDatasourcePaths));
            }

            /// TODO: Check stream. If stream is null OperatingSystemDatasourceParser throws ArgumentNullException
            OperatingSystemDatasourceParser operatingSystemDatasourceParser = new OperatingSystemDatasourceParser(stream, vocabularyHolder);

            try
            {
                operatingSystemDatasourceParser.Parse();

            }
            catch (Exception ex)
            {
                throw new InitializationException(InitializationException.INITIALIZATION_ERROR, new Exception("Can not parse document: " + oddrUaOperatingSystemDatasourcePaths));

            }

            try
            {
                stream.Close();

            }
            catch (IOException ex)
            {
                logger.Warn("", ex);
            }

            deviceIdentificator = new DeviceIdentificator(deviceBuilderParser.DeviceBuilders(), deviceDatasourceParser.devices);
            deviceIdentificator.CompleteInit();

            Dictionary<string, Browser> browsers = new Dictionary<string, Browser>(browserDatasourceParser.browsers);
            browserIdentificator = new BrowserIdentificator(new IBuilder[] { DefaultBrowserBuilder.Instance }, browsers);
            browserIdentificator.CompleteInit();

            Dictionary<string, OSModel.OperatingSystem> operatingSystems = new Dictionary<string, OSModel.OperatingSystem>(operatingSystemDatasourceParser.operatingSystems);
            osIdentificator = new OSIdentificator(new IBuilder[] { DefaultOSBuilder.Instance }, operatingSystems);
            osIdentificator.CompleteInit();

            deviceDatasourceParser = null;
            deviceBuilderParser = null;
            browserDatasourceParser = null;
            operatingSystemDatasourceParser = null;

            this.defaultVocabularyIRI = defaultVocabularyIRI;

            oddrVocabularyService = null;

            return;
        }

        public IPropertyRef[] ListPropertyRefs()
        {
            List<IPropertyRef> propertyRefsList = new List<IPropertyRef>();
            Dictionary<String, Vocabulary> vocabularies = vocabularyHolder.GetVocabularies();

            foreach (String vocabularyKey in vocabularies.Keys)
            {
                Vocabulary vocabulary = vocabularies[vocabularyKey];
                Dictionary<String, VocabularyProperty> properties = vocabulary.properties;

                foreach (String propertyKey in properties.Keys)
                {
                    VocabularyProperty vocabularyProperty = properties[propertyKey];
                    string[] aspects = vocabularyProperty.aspects;
                    IPropertyName propertyName = new ODDRPropertyName(propertyKey, vocabularyKey);
                    for (int i = 0; i < aspects.Length; i++)
                    {
                        IPropertyRef propertyRef = new ODDRPropertyRef(propertyName, aspects[i]);
                        propertyRefsList.Add(propertyRef);
                    }
                }
            }

            IPropertyRef[] propertyRefs = new IPropertyRef[propertyRefsList.Count];
            propertyRefs = propertyRefsList.ToArray();

            return propertyRefs;
        }

        public IEvidence NewHTTPEvidence(Dictionary<string, string> map)
        {
            return new ODDRHTTPEvidence(map);
        }

        public IEvidence NewHTTPEvidence()
        {
            return new ODDRHTTPEvidence();
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyName NewPropertyName(string localPropertyName, string vocabularyIRI)
        {
            vocabularyHolder.ExistProperty(localPropertyName, null, vocabularyIRI, true);
            return new ODDRPropertyName(localPropertyName, vocabularyIRI);
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyName NewPropertyName(string localPropertyName)
        {
            return NewPropertyName(localPropertyName, defaultVocabularyIRI);
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyRef NewPropertyRef(IPropertyName pn, string localAspectName)
        {
            vocabularyHolder.ExistProperty(pn.LocalPropertyName(), localAspectName, pn.Namespace(), true);
            return new ODDRPropertyRef(pn, localAspectName);
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyRef NewPropertyRef(IPropertyName pn)
        {
            VocabularyProperty vocabularyProperty = vocabularyHolder.ExistProperty(pn.LocalPropertyName(), null, pn.Namespace(), true);
            return NewPropertyRef(pn, vocabularyProperty.defaultAspect);
        }

        /// <exception cref="W3c.Ddr.Exceptions.NameException">Thrown when...</exception>
        public IPropertyRef NewPropertyRef(string localPropertyName)
        {
            return NewPropertyRef(NewPropertyName(localPropertyName));
        }
    }
}
