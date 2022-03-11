using System;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Jeeves
{
	// ref https://github.com/microsoftgraph/consoleApp-deltaQuery-dotNet-sample/blob/master/DeltaQuery/Authentication/DeviceCodeAuthProvider.cs
	public class MSGraphAuth : IAuthenticationProvider
	{
		private IPublicClientApplication _app;
		private string[] _scopes;

        public MSGraphAuth(string[] scopes)
        {
            _scopes = scopes;
            MSGraphConfig config = MSGraphConfig.ReadFileFromJson("appsettings.json");
            _app = PublicClientApplicationBuilder.CreateWithApplicationOptions(config.ClientAppOptions)
                    .WithDefaultRedirectUri()
                    .Build();
        }

        public async Task<string> GetTokenAsync(IEnumerable<string> scope)
        {
            // todo interactive for debug and device flow for production
            return await GetTokenInteractiveAsync(_app, scope);
        }

        private async Task<string> GetTokenInteractiveAsync(IPublicClientApplication app, IEnumerable<string> scope)
        {
            var accounts = await app.GetAccountsAsync();
            AuthenticationResult result;
            try
            {
                result = await app.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                result = await app.AcquireTokenInteractive(scope).ExecuteAsync();
            }
            return result.AccessToken;
        }

        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", await GetTokenAsync(_scopes));
        }
    }
}

