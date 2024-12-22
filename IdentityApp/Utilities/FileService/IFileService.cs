using System;

namespace StylishApp.Utilities.FileService;

public interface IFileService
{
    string Upload(IFormFile file, string folderName);
    void Delete(string fileName, string folderName);
    bool IsImage(string contentType);

    bool IsAvailableSize(long size, long max = 100);
}
