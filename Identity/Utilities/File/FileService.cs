
using Microsoft.AspNetCore.Hosting;

namespace Identity.Utilities.File
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public string Upload(IFormFile file,string folder)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                file.CopyTo(fileStream);
            }
            return fileName ;
        }

        public void Delete(string folder,string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath,folder, fileName);

            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);    
        }

        public bool IsImage(string contentType)
        {
            if (contentType.Contains("image/")) return true;
            
            return false;
        }
        public bool IsAvailableSize(long length,long maxLength=250)
        {
            if(length/1024 <= maxLength) return true;
            return false;
        }
    }
}
