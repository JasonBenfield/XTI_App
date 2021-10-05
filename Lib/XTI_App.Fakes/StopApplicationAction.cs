using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class StopApplicationAction : AppAction<EmptyRequest, EmptyActionResult>
    {
        private readonly IHostApplicationLifetime lifetime;

        public StopApplicationAction(IHostApplicationLifetime lifetime)
        {
            this.lifetime = lifetime;
        }

        public Task<EmptyActionResult> Execute(EmptyRequest model)
        {
            Console.WriteLine("Ending");
            lifetime.StopApplication();
            return Task.FromResult(new EmptyActionResult());
        }
    }
}
