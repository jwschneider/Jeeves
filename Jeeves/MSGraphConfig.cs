using System;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Globalization;

namespace Jeeves
{
	// https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-desktop-app-configuration?tabs=dotnet
	public class MSGraphConfig
	{
		public PublicClientApplicationOptions ClientAppOptions { get; set; }

		public string MicrosoftGraphBaseEndpoint { get; set; }

		public string Authority { get; set; }

		public static MSGraphConfig ReadFileFromJson(string path)
        {
			IConfigurationRoot Configuration;
			var builder = new ConfigurationBuilder()
								.SetBasePath(Directory.GetCurrentDirectory())
								.AddJsonFile(path);
			Configuration = builder.Build();
			MSGraphConfig config = new MSGraphConfig()
			{
				ClientAppOptions = new PublicClientApplicationOptions()
			};
			Configuration.Bind("Authentication", config.ClientAppOptions);
			config.Authority = String.Format(CultureInfo.InvariantCulture, config.ClientAppOptions.Instance, config.ClientAppOptions.TenantId);
			config.MicrosoftGraphBaseEndpoint = Configuration.GetValue<string>("WebAPI:MicrosoftGraphBaseEndpoint");
			return config;
        }
	}
}

