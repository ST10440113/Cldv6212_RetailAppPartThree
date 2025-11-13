using Cldv6212_RetailAppPartThree.Models;
using Cldv6212_RetailAppPartThree.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cldv6212_RetailAppPartThree.Controllers
{
    public class FilesController : Controller
    {
        private readonly FileService _fileService;

        public FilesController(FileService fileService)
        {
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            List<FileModel> files;
            try
            {
                files = await _fileService.ListFilesAsync("uploads");
            }
            catch (Exception ex)
            {

                ViewBag.Message = $"Failed to load files : {ex.Message}";
                files = new List<FileModel>();
            }
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file to upload.");
                return await Index();
            }
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    string directoryName = "uploads";
                    string fileName = file.FileName;
                    await _fileService.UploadFileAsync(directoryName, fileName, stream);
                }
                TempData["Message"] = "File uploaded successfully.";

            }
            catch (Exception ex)
            {
                TempData["Message"] = $"File upload failed: {ex.Message}";
            }
            return RedirectToAction("Index");
        }
    }
}
