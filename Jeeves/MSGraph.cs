using System;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace Jeeves
{
	public static class MSGraph
	{
		public static MSGraphConfig config
		{
			get {
				if (_config == null)
                {
					_config = MSGraphConfig.ReadFileFromJson("appsettings.json");
                }
				return _config;
			}
		}
		public static IPublicClientApplication app
        {
			get
            {
				if (_app == null)
                {
					_app = PublicClientApplicationBuilder.CreateWithApplicationOptions(config.ClientAppOptions)
							.WithDefaultRedirectUri()
							.Build();
                }
				return _app;
            }
        }
		private static IPublicClientApplication _app = null;
		private static MSGraphConfig _config = null;

		public static async Task<AuthenticationResult> GetTokenAsync(IEnumerable<string> scope)
        {
			return await GetTokenInteractiveAsync(app, scope);
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

