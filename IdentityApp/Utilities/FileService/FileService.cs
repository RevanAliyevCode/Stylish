using System;

namespace StylishApp.Utilities.FileService;

public class FileService : IFileService
{
    readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string Upload(IFormFile file, string folderName)
    {
        string fileName = $"{Guid.NewGuid()}_{file.FileName}";
        string filePath = Path.Combine(_env.WebRootPath, folderName, fileName);

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
        file.CopyTo(fileStream);

        return fileName;
    }

    public void Delete(string fileName, string folderName)
    {
        string filePath = Path.Combine(_env.WebRootPath, folderName, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
    public bool IsAvailableSize(long size, long max = 100) => (size / 1024) < max;

    public bool IsImage(string contentType) => contentType.Contains("image/");

}
