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

            Type stype = Type.GetType("Oddr.ODDRService, OpenDddr.Oddr");

            IService openDDRService = ServiceFactory.newService(stype, props.GetProperty("oddr.vocabulary.device"), props);

            IPropertyRef vendorRef = openDDRService.NewPropertyRef("vendor");
            IPropertyRef modelRef = openDDRService.NewPropertyRef("model");
            IPropertyRef displayWidthRef = openDDRService.NewPropertyRef("displayWidth");
            IPropertyRef displayHeightRef = openDDRService.NewPropertyRef("displayHeight");

            IPropertyRef[] propertyRefs = new IPropertyRef[] { vendorRef, modelRef, displayWidthRef, displayHeightRef };

            IEvidence e = new ODDRHTTPEvidence();
            e.Put("User-Agent", userAgent);

            IPropertyValues propertyValues = openDDRService.GetPropertyValues(e, propertyRefs);
            if (propertyValues.GetValue(vendorRef).Exists())
            {
                Console.WriteLine(propertyValues.GetValue(vendorRef).GetString());
            }

            if (propertyValues.GetValue(modelRef).Exists())
            {
                Console.WriteLine(propertyValues.GetValue(modelRef).GetString());
            }

            if (propertyValues.GetValue(displayWidthRef).Exists())
            {
                Console.WriteLine(propertyValues.GetValue(displayWidthRef).GetString());
            }

            if (propertyValues.GetValue(displayHeightRef).Exists())
            {
                Console.WriteLine(propertyValues.GetValue(displayHeightRef).GetString());
            }

            Console.ReadKey();
        }
    }
}
