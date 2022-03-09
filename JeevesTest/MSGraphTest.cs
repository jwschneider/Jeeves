using System;
using Jeeves;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Identity.Client;
using System.Linq;

namespace JeevesTest
{
	[TestClass]
	public class MSGraphTest
	{
		[TestMethod]
		public void MSGraphAuthenticationTest()
		{
			string[] scope = { "User.Read" };
			IPublicClientApplication app = MSGraph.app;
			AuthenticationResult result = null;
			try
            {
				result = app.AcquireTokenInteractive(scope).ExecuteAsync().Result;
            }
			catch (MsalException ex)
            {
				Assert.Fail($"AcquireTokenInteractive threw exception \"{ex.Message}\".");
            }
			var accounts = app.GetAccountsAsync().Result;
			try
            {
				result = app.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync().Result;
            }
			catch (MsalUiRequiredException ex)
            {
				Assert.Fail($"AcquireTokenSilent should have succeeded but threw exception \"{ex.Message}\".");
            }
			Assert.IsNotNull(result);
		}
	}
}

