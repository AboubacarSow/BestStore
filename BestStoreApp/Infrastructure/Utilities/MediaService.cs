using System.ComponentModel.DataAnnotations;

namespace BestStoreApp.Infrastructure.Utilities
{
    public static  class MediaService
    {
        public static string UploadImage(IFormFile file)
        {
            var extension=Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
                throw new ValidationException("The file mus be on an image format");
            var currentDirectory=Directory.GetCurrentDirectory();
            var folder = Path.Combine(currentDirectory, "wwwroot/products");
            if (File.Exists(folder)) 
                Directory.CreateDirectory(folder);
            var imageName=String.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"),extension);
            var fullPath=Path.Combine(folder, imageName);
            using var stream = new FileStream(fullPath,mode:FileMode.Create);
            file.CopyTo(stream);

            return $"{imageName}";
        }
    }
}
