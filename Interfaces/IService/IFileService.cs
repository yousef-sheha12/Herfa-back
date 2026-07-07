namespace Herfa_back.Interfaces.IService
{
    public interface IFileService
    {
        // فانكشن لرفع الصورة وترجع المسار اللي هيتخزن في الداتابيز
        Task<string?> UploadImageAsync(IFormFile? file, string folderName);

        // فانكشن اختيارية لو حبيت تمسح الصورة القديمة عند التعديل
        void DeleteImage(string? imagePath);
    }
}
