using System;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace Jeeves
{
	public static class MSGraph
	{
		public static IPublicClientApplication application { get; set; } = null;

		public static IPublicClientApplication GetClientApp()
        {
			if (application == null)
			{
				MSGraphConfig config = MSGraphConfig.ReadFileFromJson("appsettings.json");
				application = PublicClientApplicationBuilder.CreateWithApplicationOptions(config.ClientAppOptions)
								.WithAuthority(new Uri(config.Authority))
								.WithDefaultRedirectUri()
								.Build();
			}
			return application;
        }
		public static async Task<AuthenticationResult> GetTokenAsync(IEnumerable<string> scope)
        {
			return await GetTokenInteractiveAsync(GetClientApp(), scope);
		}
		private static async Task<AuthenticationResult> GetTokenInteractiveAsync(this IPublicClientApplication app, IEnumerable<string> scope)
        {
			var accounts = await app.GetAccountsAsync();
			AuthenticationResult result;
			try
            {
				result = await app.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync();
            }
			catch(MsalUiRequiredException)
            {
				result = await app.AcquireTokenInteractive(scope).ExecuteAsync();
            }
			return result;
        }
	}
}

