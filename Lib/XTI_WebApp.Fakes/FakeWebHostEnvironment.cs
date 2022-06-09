using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace XTI_WebApp.Fakes;

public sealed class FakeWebHostEnvironment : IHostEnvironment, IWebHostEnvironment
{
    public string EnvironmentName { get; set; } = "";
    public string ApplicationName { get; set; } = "";
    public string ContentRootPath { get; set; } = "";
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = "";
}