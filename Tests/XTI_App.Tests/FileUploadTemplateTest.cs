using Microsoft.AspNetCore.Http;
using XTI_App.Api;
using XTI_App.Fakes;
using XTI_Core.Extensions;

namespace XTI_App.Tests;

internal sealed class FileUploadTemplateTest
{
    [Test]
    public void ShouldNotHaveFileUploads_WhenNoFileUploadsAreRequested()
    {
        var sp = new XtiHostBuilder().Build().Scope();
        var template = CreateFileUploadTemplate(sp);
        var actionTemplate = template.GroupTemplates
            .First(gt => gt.Name == "Group1")
            .ActionTemplates
            .First(at => at.Name == "NoFileUpload");
        var hasFileUploads = actionTemplate.HasFileUploadTemplates();
        Assert.That(hasFileUploads, Is.False);
    }

    [Test]
    public void ShouldHaveFileUploads_WhenFileUploadIsTheRequest()
    {
        var sp = new XtiHostBuilder().Build().Scope();
        var template = CreateFileUploadTemplate(sp);
        var actionTemplate = template.GroupTemplates
            .First(gt => gt.Name == "Group1")
            .ActionTemplates
            .First(at => at.Name == "FileUpload");
        var hasFileUploads = actionTemplate.HasFileUploadTemplates();
        Assert.That(hasFileUploads, Is.True);
    }

    [Test]
    public void ShouldHaveFileUploads_WhenFileUploadArrayIsTheRequest()
    {
        var sp = new XtiHostBuilder().Build().Scope();
        var template = CreateFileUploadTemplate(sp);
        var actionTemplate = template.GroupTemplates
            .First(gt => gt.Name == "Group1")
            .ActionTemplates
            .First(at => at.Name == "FileUploadArray");
        var hasFileUploads = actionTemplate.HasFileUploadTemplates();
        Assert.That(hasFileUploads, Is.True);
    }

    [Test]
    public void ShouldHaveFileUploads_WhenFileUploadIsThePropertyOfAnObject()
    {
        var sp = new XtiHostBuilder().Build().Scope();
        var template = CreateFileUploadTemplate(sp);
        var actionTemplate = template.GroupTemplates
            .First(gt => gt.Name == "Group1")
            .ActionTemplates
            .First(at => at.Name == "FileUploadTopLevel");
        var hasFileUploads = actionTemplate.HasFileUploadTemplates();
        Assert.That(hasFileUploads, Is.True);
    }

    [Test]
    public void ShouldHaveFileUpload_WhenFileUploadArrayIsThePropertyOfAnObject()
    {
        var sp = new XtiHostBuilder().Build().Scope();
        var template = CreateFileUploadTemplate(sp);
        var actionTemplate = template.GroupTemplates
            .First(gt => gt.Name == "Group1")
            .ActionTemplates
            .First(at => at.Name == "FileUploadArrayTopLevel");
        var hasFileUploads = actionTemplate.HasFileUploadTemplates();
        Assert.That(hasFileUploads, Is.True);
    }

    [Test]
    public void ShouldHaveFileUploads_WhenFileUploadIsThePropertyOfANestedObject()
    {
        var sp = new XtiHostBuilder().Build().Scope();
        var template = CreateFileUploadTemplate(sp);
        var actionTemplate = template.GroupTemplates
            .First(gt => gt.Name == "Group1")
            .ActionTemplates
            .First(at => at.Name == "FileUploadSecondLevel");
        var hasFileUploads = actionTemplate.HasFileUploadTemplates();
        Assert.That(hasFileUploads, Is.True);
    }

    [Test]
    public void ShouldHaveFileUploads_WhenFileUploadArrayIsThePropertyOfANestedObject()
    {
        var sp = new XtiHostBuilder().Build().Scope();
        var template = CreateFileUploadTemplate(sp);
        var actionTemplate = template.GroupTemplates
            .First(gt => gt.Name == "Group1")
            .ActionTemplates
            .First(at => at.Name == "FileUploadArraySecondLevel");
        var hasFileUploads = actionTemplate.HasFileUploadTemplates();
        Assert.That(hasFileUploads, Is.True);
    }

    private static AppApiTemplate CreateFileUploadTemplate(IServiceProvider sp)
    {
        var api = new AppApi(sp, FakeInfo.AppKey, new AppApiSuperUser());
        var group1 = api.AddGroup("Group1");
        group1.AddAction<NoFileUploadRequest, EmptyActionResult>()
            .Named("NoFileUpload")
            .WithExecution(() => EmptyAppAction.Create<NoFileUploadRequest>())
            .Build();
        group1.AddAction<IFormFile, EmptyActionResult>()
            .Named("FileUpload")
            .WithExecution(() => EmptyAppAction.Create<IFormFile>())
            .Build();
        group1.AddAction<IFormFile[], EmptyActionResult>()
            .Named("FileUploadArray")
            .WithExecution(() => EmptyAppAction.Create<IFormFile[]>())
            .Build();
        group1.AddAction<FileUploadTopLevelRequest, EmptyActionResult>()
            .Named("FileUploadTopLevel")
            .WithExecution(() => EmptyAppAction.Create<FileUploadTopLevelRequest>())
            .Build();
        group1.AddAction<FileUploadArrayRequest, EmptyActionResult>()
            .Named("FileUploadArrayTopLevel")
            .WithExecution(() => EmptyAppAction.Create<FileUploadArrayRequest>())
            .Build();
        group1.AddAction<FileUploadSecondLevelRequest, EmptyActionResult>()
            .Named("FileUploadSecondLevel")
            .WithExecution(() => EmptyAppAction.Create<FileUploadSecondLevelRequest>())
            .Build();;
        group1.AddAction<FileUploadArraySecondLevelRequest, EmptyActionResult>()
            .Named("FileUploadArraySecondLevel")
            .WithExecution(() => EmptyAppAction.Create<FileUploadArraySecondLevelRequest>())
            .Build();
        var template = new AppApiTemplate(api, "");
        return template;
    }

    private sealed class FileUploadTopLevelRequest
    {
        public IFormFile? File { get; set; }
    }

    private sealed class FileUploadArrayRequest
    {
        public IFormFile[] Files { get; set; } = [];
    }

    private sealed class FileUploadSecondLevelRequest
    {
        public IFormFile? File { get; set; }
    }

    private sealed class FileUploadArraySecondLevelRequest
    {
        public IFormFile[] Files { get; set; } = [];
    }

    private sealed class NoFileUploadRequest
    {
        public int Value1 { get; set; }
        public string Value2 { get; set; } = "";
    }

}
