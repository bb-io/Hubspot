using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Hubspot.Base;

public class FileManager(string folderLocation) : IFileManagementClient
{
    public Task<Stream> DownloadAsync(FileReference reference)
    {
        var path = Path.Combine(folderLocation, @$"Input\{reference.Name}");
        var bytes = File.ReadAllBytes(path);

        var stream = new MemoryStream(bytes);
        return Task.FromResult((Stream)stream);
    }

    public Task<FileReference> UploadAsync(Stream stream, string contentType, string fileName)
    {
        var path = Path.Combine(folderLocation, @$"Output\{fileName}");
        new FileInfo(path).Directory.Create();
        using (var fileStream = File.Create(path))
        {
            stream.CopyTo(fileStream);
        }

        return Task.FromResult(new FileReference() { Name = fileName });
    }

    public string ReadOutputAsString(FileReference reference)
    {
        var path = Path.Combine(folderLocation, @$"Output\{reference.Name}");
        Assert.IsTrue(File.Exists(path), $"File not found at: {path}");
        return File.ReadAllText(path, Encoding.UTF8)!;
    }
}