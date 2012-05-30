using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using W3c.Ddr.Models;
using W3c.Ddr.Simple;
using Oddr.Models;

namespace OpenDDRTest
{
    public class SimpleTest
    {
        public static void Main(string[] args)
        {
            string oddrPropertiesPath = args[0];
            string userAgent = args[1];

            Properties props = new Properties(oddrPropertiesPath);

            Type stype = Type.GetType("Oddr.ODDRService, OpenDdr");

            IService openDDRService = ServiceFactory.newService(stype, props.GetProperty("oddr.vocabulary.device"), props);

            IPropertyName vendorDevicePropertyName = openDDRService.NewPropertyName("vendor", @"http://www.openddr.org/oddr-vocabulary");
            IPropertyRef vendorDeviceRef = openDDRService.NewPropertyRef(vendorDevicePropertyName, "device");

            IPropertyName modelDevicePropertyName = openDDRService.NewPropertyName("model", @"http://www.openddr.org/oddr-vocabulary");
            IPropertyRef modelDeviceRef = openDDRService.NewPropertyRef(modelDevicePropertyName, "device");

            IPropertyName vendorBrowserPropertyName = openDDRService.NewPropertyName("vendor", @"http://www.openddr.org/oddr-vocabulary");
            IPropertyRef vendorBrowserRef = openDDRService.NewPropertyRef(vendorBrowserPropertyName, "webBrowser");

            IPropertyName modelBrowserPropertyName = openDDRService.NewPropertyName("model", @"http://www.openddr.org/oddr-vocabulary");
            IPropertyRef modelBrowserRef = openDDRService.NewPropertyRef(modelBrowserPropertyName, "webBrowser");

            IPropertyRef[] propertyRefs = new IPropertyRef[] { vendorDeviceRef, modelDeviceRef, vendorBrowserRef, modelBrowserRef };

            IEvidence e = new BufferedODDRHTTPEvidence();
            e.Put("User-Agent", userAgent);

            IPropertyValues propertyValues = openDDRService.GetPropertyValues(e, propertyRefs);
            if (propertyValues.GetValue(vendorDeviceRef).Exists())
            {
                Console.WriteLine(propertyValues.GetValue(vendorDeviceRef).GetString());
            }

            if (propertyValues.GetValue(modelDeviceRef).Exists())
            {
                Console.WriteLine(propertyValues.GetValue(modelDeviceRef).GetString());
            }

            if (propertyValues.GetValue(vendorBrowserRef).Exists())
            {
                Console.WriteLine(propertyValues.GetValue(vendorBrowserRef).GetString());
            }

            if (propertyValues.GetValue(modelBrowserRef).Exists())
            {
                Console.WriteLine(propertyValues.GetValue(modelBrowserRef).GetString());
            }

            Console.WriteLine(((BufferedODDRHTTPEvidence) e).deviceFound.Get("is_wireless_device"));

            Console.ReadKey();
        }
    }
}
