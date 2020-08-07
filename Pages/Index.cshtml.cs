using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptographerFLS.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CryptographerFLS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IWebHostEnvironment _hostingEnvironment;

        public static string original = "";
        public static EncryptModel model;
        [BindProperty]
        public bool ToEncrypt { get; set; }

        [BindProperty]
        public IFormFile FormFile { get; set; }
        [BindProperty]
        public string Key { get; set; }
        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _hostingEnvironment = environment;
        }
        public ActionResult OnGetDownloadTxt(bool crypted)
        {
            if (!crypted)
            {
                var path = Path.Combine(_hostingEnvironment.ContentRootPath + "/wwwroot", "result.txt");
                string contentType = "text/plain";

                using(StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(model.Decrypted);
                }

                return PhysicalFile(path, contentType, "result.txt");
            }
            else
            {
                var path = Path.Combine(_hostingEnvironment.ContentRootPath + "/wwwroot", "resultCrypted.txt");
                string contentType = "text/plain";

                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(model.Encrypted);
                }

                return PhysicalFile(path, contentType, "resultCrypted.txt");
            }
        }

        public ActionResult OnGetDownloadDocx(bool crypted)
        {
            if (!crypted)
            {
                var path = Path.Combine(_hostingEnvironment.ContentRootPath + "/wwwroot", "result.docx");
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                using(WordprocessingDocument doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();

                    Body body = mainPart.Document.AppendChild(new Body());

                    // Add new text.
                    Paragraph para = body.AppendChild(new Paragraph());
                    Run run = para.AppendChild(new Run());
                    run.AppendChild(new Text(model.Decrypted));
                }
                return PhysicalFile(path, contentType, "result.docx");
            }
            else
            {
                var path = Path.Combine(_hostingEnvironment.ContentRootPath + "/wwwroot", "resultCrypted.docx");
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                using (WordprocessingDocument doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();

                    Body body = mainPart.Document.AppendChild(new Body());

                    // Add new text.
                    Paragraph para = body.AppendChild(new Paragraph());
                    Run run = para.AppendChild(new Run());
                    run.AppendChild(new Text(model.Encrypted));
                }
                return PhysicalFile(path, contentType, "resultCrypted.docx");
            }
        }
        //Запись информации с помощью файла
        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (FormFile != null && FormFile?.Length > 0 && (FormFile.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || FormFile.ContentType == "text/plain"))
            {
                try
                {
                    //.docx
                    if (FormFile.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await FormFile.CopyToAsync(memoryStream);

                            using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                            {
                                var body = doc.MainDocumentPart.Document.Body;
                                original = body.InnerText;
                            }
                        }
                        model = new EncryptModel(original, ToEncrypt, Key);
                        Key = "";
                    }
                    else // .txt
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await FormFile.CopyToAsync(memoryStream);
                            if (memoryStream.Length < 2097152)
                            {
                                original = Encoding.UTF8.GetString(memoryStream.ToArray());
                            }
                            else
                            {
                                ModelState.AddModelError("File", "The file is too large.");
                            }
                        }
                        model = new EncryptModel(original, ToEncrypt, Key);
                        Key = "";
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("File", "ИСКЛЮЧЕНИЕ:" + e.Message.ToString());
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError("File", "Choose file bliiin!!");
            }
            return Page();
        }
        //Запись информации через интерфейс пользователя
        public PageResult OnPostUploadTextAsync(string text)
        {
            model = new EncryptModel(text, ToEncrypt, Key);
            Key = "";

            return Page();
        }
    }
}
