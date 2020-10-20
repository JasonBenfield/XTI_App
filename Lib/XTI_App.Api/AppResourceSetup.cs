using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XTI_App.Api
{
    public sealed class AppResourceSetup : IAppSetup
    {
        private readonly AppFactory appFactory;
        private readonly AppApiTemplate appTemplate;

        public AppResourceSetup(AppFactory appFactory, AppApiTemplate appTemplate)
        {
            this.appFactory = appFactory;
            this.appTemplate = appTemplate;
        }


        public async Task Run()
        {
        }
    }
}
