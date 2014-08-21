using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using W3c.Ddr.Models;
using W3c.Ddr.Simple;
using Oddr.Models;

namespace OpenDDRWebTest
{
	public partial class _Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs eArgs)
		{
			IService openDDRService = null;
			IPropertyRef[] propertyRefs = null;

			IPropertyName vendorDevicePropertyName = null;
			IPropertyRef vendorDeviceRef = null;

			IPropertyName modelDevicePropertyName = null;
			IPropertyRef modelDeviceRef = null;

			IPropertyName vendorBrowserPropertyName = null;
			IPropertyRef vendorBrowserRef = null;

			IPropertyName modelBrowserPropertyName = null;
			IPropertyRef modelBrowserRef = null;

			IPropertyName modelBrowserPropertyVer = null;
			IPropertyRef modelBrowserVer = null;


			string userAgent = Request.UserAgent;

			// IE11 user agent that causes a null value for deviceFound
			// userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; MALC; rv:11.0) like Gecko";

			Output.InnerHtml += "<p>USER AGENT: " + userAgent + "</p>";

			if (Application["oddr"] == null)
			{
				try
				{
					Properties props = new Properties("oddr.properties", "/resources/");

					Type stype = Type.GetType("Oddr.ODDRService, OpenDdr");

					openDDRService = ServiceFactory.newService(stype, props.GetProperty("oddr.vocabulary.device"), props);
					Application["oddr"] = openDDRService;

					vendorDevicePropertyName = openDDRService.NewPropertyName("vendor", @"http://www.openddr.org/oddr-vocabulary");
					vendorDeviceRef = openDDRService.NewPropertyRef(vendorDevicePropertyName, "device");
					Application["vendorDevicePropertyName"] = vendorDevicePropertyName;
					Application["vendorDeviceRef"] = vendorDeviceRef;

					modelDevicePropertyName = openDDRService.NewPropertyName("model", @"http://www.openddr.org/oddr-vocabulary");
					modelDeviceRef = openDDRService.NewPropertyRef(modelDevicePropertyName, "device");
					Application["modelDevicePropertyName"] = modelDevicePropertyName;
					Application["modelDeviceRef"] = modelDeviceRef;

					vendorBrowserPropertyName = openDDRService.NewPropertyName("vendor", @"http://www.openddr.org/oddr-vocabulary");
					vendorBrowserRef = openDDRService.NewPropertyRef(vendorBrowserPropertyName, "webBrowser");
					Application["vendorBrowserPropertyName"] = vendorBrowserPropertyName;
					Application["vendorBrowserRef"] = vendorBrowserRef;

					modelBrowserPropertyName = openDDRService.NewPropertyName("model", @"http://www.openddr.org/oddr-vocabulary");
					modelBrowserRef = openDDRService.NewPropertyRef(modelBrowserPropertyName, "webBrowser");
					Application["modelBrowserPropertyName"] = modelBrowserPropertyName;
					Application["modelBrowserRef"] = modelBrowserRef;

					modelBrowserPropertyVer = openDDRService.NewPropertyName("version", @"http://www.openddr.org/oddr-vocabulary");
					modelBrowserVer = openDDRService.NewPropertyRef(modelBrowserPropertyVer, "webBrowser");
					Application["modelBrowserPropertyVer"] = modelBrowserPropertyVer;
					Application["modelBrowserVer"] = modelBrowserVer;

					propertyRefs = new IPropertyRef[] { vendorDeviceRef, modelDeviceRef, vendorBrowserRef, modelBrowserRef, modelBrowserVer };
					Application["propertyRefs"] = propertyRefs;
				}

				catch (Exception exc)
				{
					Output.InnerHtml += "<br />ERROR: " + exc.ToString() + "<br />";
				}
			}

			else
			{
				openDDRService = (IService)Application["oddr"];

				vendorDevicePropertyName = (IPropertyName)Application["vendorDevicePropertyName"];
				vendorDeviceRef = (IPropertyRef)Application["vendorDeviceRef"];

				modelDevicePropertyName = (IPropertyName)Application["modelDevicePropertyName"];
				modelDeviceRef = (IPropertyRef)Application["modelDeviceRef"];

				vendorBrowserPropertyName = (IPropertyName)Application["vendorBrowserPropertyName"];
				vendorBrowserRef = (IPropertyRef)Application["vendorBrowserRef"];

				modelBrowserPropertyName = (IPropertyName)Application["modelBrowserPropertyName"];
				modelBrowserRef = (IPropertyRef)Application["modelBrowserRef"];

				modelBrowserPropertyVer = (IPropertyName)Application["modelBrowserPropertyVer"];
				modelBrowserVer = (IPropertyRef)Application["modelBrowserVer"];

				propertyRefs = (IPropertyRef[])Application["propertyRefs"];
			}

			try
			{
				IEvidence e = new BufferedODDRHTTPEvidence();
				e.Put("User-Agent", userAgent);

				IPropertyValues propertyValues = openDDRService.GetPropertyValues(e, propertyRefs);

				if (propertyValues.GetValue(vendorDeviceRef).Exists())
				{
					Output.InnerHtml += "<p>Vendor Device Ref: " + propertyValues.GetValue(vendorDeviceRef).GetString() + "</p>";
				}

				if (propertyValues.GetValue(modelDeviceRef).Exists())
				{
					Output.InnerHtml += "<p>Mobile Device Ref: " + propertyValues.GetValue(modelDeviceRef).GetString() + "</p>";
				}

				if (propertyValues.GetValue(vendorBrowserRef).Exists())
				{
					Output.InnerHtml += "<p>Vendor: " + propertyValues.GetValue(vendorBrowserRef).GetString() + "</p>";
				}

				if (propertyValues.GetValue(modelBrowserRef).Exists())
				{
					Output.InnerHtml += "<p>Model: " + propertyValues.GetValue(modelBrowserRef).GetString() + "</p>";
				}

				if (propertyValues.GetValue(modelBrowserVer).Exists())
				{
					Output.InnerHtml += "<p>Version: " + propertyValues.GetValue(modelBrowserVer).GetString() + "</p>";
				}

				if (((BufferedODDRHTTPEvidence)e).deviceFound != null)
				{
					Output.InnerHtml += "<p>Dual orientation: " + ((BufferedODDRHTTPEvidence)e).deviceFound.Get("dual_orientation") + "</p>";

					Output.InnerHtml += "<p>Tablet: " + ((BufferedODDRHTTPEvidence)e).deviceFound.Get("is_tablet") + "</p>";

					Output.InnerHtml += "<p>Wireless device: " + ((BufferedODDRHTTPEvidence)e).deviceFound.Get("is_wireless_device") + "</p>";

					Output.InnerHtml += "<p>Mobile browser: " + ((BufferedODDRHTTPEvidence)e).deviceFound.Get("mobile_browser") + "</p>";
				}
			}

			catch (Exception exc)
			{
				Output.InnerHtml += "<br />ERROR2: " + exc.ToString() + "<br />";
			}
		}
	}
}