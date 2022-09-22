using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTI_App.Api;
using XTI_App.Fakes;

namespace XTI_App.Tests;

internal sealed class FileUploadTemplateTest
{
    [Test]
    public void ShouldNotHaveFileUploads_WhenNoFileUploadsAreRequested()
    {
        var template = CreateFileUploadTemplate();
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
        var template = CreateFileUploadTemplate();
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
        var template = CreateFileUploadTemplate();
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
        var template = CreateFileUploadTemplate();
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
        var template = CreateFileUploadTemplate();
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
        var template = CreateFileUploadTemplate();
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
        var template = CreateFileUploadTemplate();
        var actionTemplate = template.GroupTemplates
            .First(gt => gt.Name == "Group1")
            .ActionTemplates
            .First(at => at.Name == "FileUploadArraySecondLevel");
        var hasFileUploads = actionTemplate.HasFileUploadTemplates();
        Assert.That(hasFileUploads, Is.True);
    }

    private static AppApiTemplate CreateFileUploadTemplate()
    {
        var api = new AppApi(FakeInfo.AppKey, new AppApiSuperUser(), ResourceAccess.AllowAuthenticated());
        var group1 = api.AddGroup("Group1");
        group1.AddAction
        (
            "NoFileUpload",
            () => new NoFileUploadAction()
        );
        group1.AddAction
        (
            "FileUpload",
            () => new FileUploadAction()
        );
        group1.AddAction
        (
            "FileUploadArray",
            () => new FileUploadArrayAction()
        );
        group1.AddAction
        (
            "FileUploadTopLevel",
            () => new FileUploadTopLevelAction()
        );
        group1.AddAction
        (
            "FileUploadArrayTopLevel",
            () => new FileUploadArrayTopLevelAction()
        );
        group1.AddAction
        (
            "FileUploadSecondLevel",
            () => new FileUploadSecondLevelAction()
        );
        group1.AddAction
        (
            "FileUploadArraySecondLevel",
            () => new FileUploadArraySecondLevelAction()
        );
        var template = new AppApiTemplate(api);
        return template;
    }

    private sealed class FileUploadTopLevelRequest
    {
        public IFormFile? File { get; set; }
    }

    private sealed class FileUploadArrayRequest
    {
        public IFormFile[] Files { get; set; } = new IFormFile[0];
    }

    private sealed class FileUploadSecondLevelRequest
    {
        public IFormFile? File { get; set; }
    }

    private sealed class FileUploadArraySecondLevelRequest
    {
        public IFormFile[] Files { get; set; } = new IFormFile[0];
    }

    private sealed class NoFileUploadRequest
    {
        public int Value1 { get; set; }
        public string Value2 { get; set; } = "";
    }

    private sealed class NoFileUploadAction : AppAction<NoFileUploadRequest, EmptyActionResult>
    {
        public Task<EmptyActionResult> Execute(NoFileUploadRequest model, CancellationToken stoppingToken)
        {
            return Task.FromResult(new EmptyActionResult());
        }
    }

    private sealed class FileUploadAction : AppAction<IFormFile, EmptyActionResult>
    {
        public Task<EmptyActionResult> Execute(IFormFile model, CancellationToken stoppingToken)
        {
            return Task.FromResult(new EmptyActionResult());
        }
    }

    private sealed class FileUploadArrayAction : AppAction<IFormFile[], EmptyActionResult>
    {
        public Task<EmptyActionResult> Execute(IFormFile[] model, CancellationToken stoppingToken)
        {
            return Task.FromResult(new EmptyActionResult());
        }
    }

    private sealed class FileUploadTopLevelAction : AppAction<FileUploadTopLevelRequest, EmptyActionResult>
    {
        public Task<EmptyActionResult> Execute(FileUploadTopLevelRequest model, CancellationToken stoppingToken)
        {
            return Task.FromResult(new EmptyActionResult());
        }
    }

    private sealed class FileUploadArrayTopLevelAction : AppAction<FileUploadArrayRequest, EmptyActionResult>
    {
        public Task<EmptyActionResult> Execute(FileUploadArrayRequest model, CancellationToken stoppingToken)
        {
            return Task.FromResult(new EmptyActionResult());
        }
    }

    private sealed class FileUploadSecondLevelAction : AppAction<FileUploadSecondLevelRequest, EmptyActionResult>
    {
        public Task<EmptyActionResult> Execute(FileUploadSecondLevelRequest model, CancellationToken stoppingToken)
        {
            return Task.FromResult(new EmptyActionResult());
        }
    }

    private sealed class FileUploadArraySecondLevelAction : AppAction<FileUploadArraySecondLevelRequest, EmptyActionResult>
    {
        public Task<EmptyActionResult> Execute(FileUploadArraySecondLevelRequest model, CancellationToken stoppingToken)
        {
            return Task.FromResult(new EmptyActionResult());
        }
    }
}
