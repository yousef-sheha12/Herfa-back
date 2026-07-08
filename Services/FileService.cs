using Herfa_back.Interfaces.IService;

namespace Herfa_back.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            // حذف العلامة / من البداية لو موجودة عشان المسار يقرأ صح بالسيرفر
            var relativePath = imagePath.TrimStart('/');
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<string?> UploadImageAsync(IFormFile? file, string folderName)
        {
            // لو مفيش ملف ارفع null
            if (file == null || file.Length == 0)
                return null;

            // 1- تحديد مسار المجلد الأساسي في wwwroot
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

            // التأكد من أن المجلد موجود
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 2- توليد اسم فريد للصورة
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            // 3- دمج المسار بالكامل لحفظ الملف
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4- حفظ الصورة في المجلد على السيرفر
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 5- إرجاع المسار النسبي اللي هيتخزن في الداتابيز
            // استخدمنا "/" بدل Path.Combine هنا عشان الـ URLs في الـ Browser تفهمها صح في الـ Front-end
            return $"/{folderName}/{uniqueFileName}";
        }
    }
}
