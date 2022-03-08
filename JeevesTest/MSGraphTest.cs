using System;
using Jeeves;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Identity.Client;

namespace JeevesTest
{
	[TestClass]
	public class MSGraphTest
	{
		[TestMethod]
		public void MSGraphAuthenticationTest()
		{
			string[] scope = { "User.Read" };
			AuthenticationResult result = MSGraph.GetTokenAsync(scope).Result;
			Assert.IsNotNull(result);
		}
	}
}

