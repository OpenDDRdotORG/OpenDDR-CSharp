OpenDDR-ASP.NET
==============

This is the ASP.NET/C# fork of the great OpenDDR project.

OpenDDR is an API which allows developers to detect devices like mobile phones and tablets, as well as their capabilities, so that action can be taken according to the device. In my case, I needed to leverage the library to identify website visitors so that a custom experience could be presented, in cases where a truly responsive design was not possible. So I forked the codebase and added features and functionality to support the use of OpenDDR in an ASP.NET web application, as well as a C# web project example of its use.

To summarize, the primary goal of this fork is to create a version of the core OpenDDR project which adds ASP.NET-specific features and examples written in C#. It strives to be 100% compatible with the current project and data formats by making only the changes required to add the missing functionality.

Below a description of the directory tree:
* __DDR-Simple-API__: contains the porting of the W3C's Device Description Repository Simple API 
* __OpenDDR-CSharp__: contains the porting of the Java version of OpenDDR
* __OpenDDRTest__: contains a simple test project of the C# version of OpenDDR
* __OpenDDRWebTest__: contains a simple ASP.NET/C# web application test project

A basic explanation of the properties in oddr.properties follows. Note that one of the changes in this fork includes the removal of the requirement to edit this file. You can leave this file alone and specify a relative web path to the file when instatiating the Properties class. It will then replace the text placeholder "/FILESYSTEM_PATH_TO_RESOURCES/" with the relative web path for you.
* __oddr.ua.device.builder.path__: Path of the file that explains how to identify the devices. In this, for each builder, are specified the device IDs that the builder handles and the identification rules
* __oddr.ua.device.datasource.path__: Path of the device datasource
* __oddr.ua.device.builder.patch.paths__: Path of the patch file for the builder file
* __oddr.ua.device.datasource.patch.paths__: Path of the patch file for the device data source
* __oddr.ua.browser.datasource.path__: Path of the browser data source
* __oddr.ua.operatingSystem.datasource.path__: Path of the operating system data source
* __ddr.vocabulary.core.path__: Path of the W3C vocabulary file
* __oddr.vocabulary.path__: Path of the OpenDDR vocabulary
* __oddr.limited.vocabulary.path__: Path of the reduced vocabulary. This vocabulary is usefull to limitate the memory load. It can be safely left unspecified.
* __oddr.vocabulary.device__: IRI of the default vocabulary. It is the target namespace specified in a vocabulary
* __oddr.threshold__: Identification threshold. It is used to balance the request evaluation time and identification matching.

The sample ASP.NET C# class below shows how to use OpenDDR in an ASPX page, using an Application-level variable to cache the service and data for optimal speed and memory use: 

<pre><code>
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

			string userAgent = Request.UserAgent;

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

					propertyRefs = new IPropertyRef[] { vendorDeviceRef, modelDeviceRef, vendorBrowserRef, modelBrowserRef };
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

				Output.InnerHtml += "<p>Dual orientation: " + ((BufferedODDRHTTPEvidence)e).deviceFound.Get("dual_orientation") + "</p>";

				Output.InnerHtml += "<p>Tablet: " + ((BufferedODDRHTTPEvidence)e).deviceFound.Get("is_tablet") + "</p>";

				Output.InnerHtml += "<p>Wireless device: " + ((BufferedODDRHTTPEvidence)e).deviceFound.Get("is_wireless_device") + "</p>";

				Output.InnerHtml += "<p>Mobile browser: " + ((BufferedODDRHTTPEvidence)e).deviceFound.Get("mobile_browser") + "</p>";
			}
			catch (Exception exc)
			{
				Output.InnerHtml += "<br />ERROR2: " + exc.ToString() + "<br />";
			}
		}
	}
}
</code></pre>
