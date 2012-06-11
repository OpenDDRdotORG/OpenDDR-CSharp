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
using Oddr.Builders.Devices;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace Oddr.Documenthandlers
{
    public class DeviceBuilderParser
    {
        private const string ELEMENT_BUILDERS = "Builders";
        private const string BUILDER_DEVICE = "builder"; //Java source: private Object BUILDER_DEVICE = "builder";
        private const String ELEMENT_DEVICE = "device";
        private const String ELEMENT_PROPERTY = "property";
        private const String ELEMENT_LIST = "list";
        private const String ELEMENT_VALUE = "value";
        private const String ATTRIBUTE_DEVICE_ID = "id";
        private const string ATTRIBUTE_CLASS = "class";
        private List<IDeviceBuilder> builders;
        private XDocument doc;
        private Dictionary<string, string> deviceBuilderClassMapper;


        public DeviceBuilderParser(Stream stream)
        {
            Init(stream);
            this.builders = new List<IDeviceBuilder>();
        }

        public DeviceBuilderParser(Stream stream, List<IDeviceBuilder> builders)
        {
            Init(stream);
            this.builders = builders;
        }

        /// <exception cref="System.ArgumentNullException">Thrown when stream is null</exception>
        private void Init(Stream stream)
        {
            try
            {
                SetStream(stream);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }

            deviceBuilderClassMapper = new Dictionary<string, string>();
            deviceBuilderClassMapper.Add("org.openddr.simpleapi.oddr.builder.device.AndroidDeviceBuilder", "Oddr.Builders.Devices.AndroidDeviceBuilder");
            deviceBuilderClassMapper.Add("org.openddr.simpleapi.oddr.builder.device.SymbianDeviceBuilder", "Oddr.Builders.Devices.SymbianDeviceBuilder");
            deviceBuilderClassMapper.Add("org.openddr.simpleapi.oddr.builder.device.WinPhoneDeviceBuilder", "Oddr.Builders.Devices.WinPhoneDeviceBuilder");
            deviceBuilderClassMapper.Add("org.openddr.simpleapi.oddr.builder.device.IOSDeviceBuilder", "Oddr.Builders.Devices.IOSDeviceBuilder");
            deviceBuilderClassMapper.Add("org.openddr.simpleapi.oddr.builder.device.SimpleDeviceBuilder", "Oddr.Builders.Devices.SimpleDeviceBuilder");
            deviceBuilderClassMapper.Add("org.openddr.simpleapi.oddr.builder.device.TwoStepDeviceBuilder", "Oddr.Builders.Devices.TwoStepDeviceBuilder");
        }

        /// <exception cref="System.ArgumentNullException">Thrown when stream is null</exception>
        public void SetStream(Stream stream)
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

            BuilderWrapper[] buildersWrapper = (from b in doc.Descendants(ELEMENT_BUILDERS).Descendants(BUILDER_DEVICE)
                                                select new BuilderWrapper
                                                {
                                                    attributeClass = b.Attribute(ATTRIBUTE_CLASS).Value,
                                                    devices = (from d in b.Descendants(ELEMENT_DEVICE)
                                                               select new DeviceWrapper
                                                               {
                                                                   id = d.Attribute(ATTRIBUTE_DEVICE_ID).Value,
                                                                   values = d.Element(ELEMENT_LIST).Descendants(ELEMENT_VALUE).Select(x => x.Value).ToList(),
                                                               }).ToList<DeviceWrapper>(),
                                                }).ToArray<BuilderWrapper>();

            foreach (BuilderWrapper bw in buildersWrapper)
            {
                IDeviceBuilder deviceBuilderInstance = null;

                try
                {
                    Type builderType = Type.GetType(deviceBuilderClassMapper[bw.attributeClass], true);
                    foreach (IDeviceBuilder deviceBuilder in builders)
                    {
                        if (deviceBuilder.GetType().Equals(builderType))
                        {
                            deviceBuilderInstance = deviceBuilder;
                        }
                    }

                    if (deviceBuilderInstance == null)
                    {
                        deviceBuilderInstance = (IDeviceBuilder)Activator.CreateInstance(builderType);
                        builders.Add(deviceBuilderInstance);
                    }

                    foreach (DeviceWrapper d in bw.devices)
                    {
                        deviceBuilderInstance.PutDevice(d.id, d.values);
                    }
                }
                catch (ArgumentNullException ane)
                {
                    throw new ArgumentNullException("Argument is null", ane);
                }
                catch (TargetInvocationException tie)
                {
                    throw new ArgumentException("Can not instantiate class: {0} described in device builder document due to constructor exception", deviceBuilderClassMapper[bw.attributeClass]);
                }
                catch (TypeLoadException tle)
                {
                    throw new ArgumentException("Can not find class: {0} described in device builder document", deviceBuilderClassMapper[bw.attributeClass]);
                }
                catch (IOException ioe)
                {
                    throw new ArgumentException("Can not find file: {0} described in device builder document", deviceBuilderClassMapper[bw.attributeClass]);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Can not instantiate class: {0} described in device builder document", deviceBuilderClassMapper[bw.attributeClass]);
                }
            }
        }

        private class BuilderWrapper
        {
            public string attributeClass;
            public List<DeviceWrapper> devices;
        }

        private class DeviceWrapper
        {
            public string id;
            public List<string> values;
        }

        public IDeviceBuilder[] DeviceBuilders()
        {
            return builders.ToArray<IDeviceBuilder>();
        }
    }
}
