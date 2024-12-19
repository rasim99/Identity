namespace Identity.Utilities.File
{
    public interface IFileService
    {
        string Upload(IFormFile file,string folder);
        void Delete(string folder, string fileName);
        bool IsImage(string contentType);

        bool IsAvailableSize(long length, long maxLength = 250);
    }
}
