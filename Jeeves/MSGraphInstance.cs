using System;
using Microsoft.Identity.Client;
using Azure.Identity;
using Microsoft.Graph;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace Jeeves
{
	public class MSGraphInstance
	{
		//public static MSGraphConfig config
		//{
		//	get {
		//		if (_config == null)
		//		{
		//			_config = MSGraphConfig.ReadFileFromJson("appsettings.json");
		//		}
		//		return _config;
		//	}
		//}
		//public static IPublicClientApplication app
		//{
		//	get
		//	{
		//		if (_app == null)
		//		{
		//			_app = PublicClientApplicationBuilder.CreateWithApplicationOptions(config.ClientAppOptions)
		//					.WithDefaultRedirectUri()
		//					.Build();
		//		}
		//		return _app;
		//	}
		//}
		//// todo rather than instantiating a generic public client application, i should instatiate a graph service client with the interactive authenticaiton flow
		//public static GraphServiceClient client
		//{
		//	get
  //          {
		//		if (_client == null)
  //              {
		//			_client = new GraphServiceClient()
  //              }
  //          }
		//}
		//public static IPublicClientApplication _app = null;
		public MSGraphConfig config { get; set; }
		public GraphServiceClient client { get; set; }

		public MSGraphInstance(string[] scope)
        {
			IAuthenticationProvider provider = new MSGraphAuth(scope);
			client = new GraphServiceClient(provider);
        }

        public async Task<bool> DetectChangesAsync()
        {
			return false;
        }
		public async Task<IEnumerable<Job>> GetIncompleteJobsAsync()
        {
			return null;
        }
		private async Task<TodoTaskList> GetTaskList(string listName)
        {
			return null;
        }
	}
}

