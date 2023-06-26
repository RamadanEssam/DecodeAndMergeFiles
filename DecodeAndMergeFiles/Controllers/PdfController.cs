using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DecodeAndMergeFiles.Controllers
{


    [Route("api/pdf")]
    [ApiController]

    public class PdfController : ControllerBase
    {

        [HttpPost("merge")]
        public IActionResult MergePdfFiles(List<string> base64Files)
        {
            var pdfFiles = new List<byte[]>();

            foreach (var base64File in base64Files)
            {
                var decodedFile = DecodeBase64(base64File);
                pdfFiles.Add(decodedFile);
            }

            var mergedPdf = MergePDFs(pdfFiles);

            return File(mergedPdf, "application/pdf", "merged.pdf");
        }



        private byte[] DecodeBase64(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }
        private byte[] MergePDFs(List<byte[]> pdfFiles)
        {
            using (var memoryStream = new MemoryStream())
            {
                var document = new iTextSharp.text.Document();
                var writer = new iTextSharp.text.pdf.PdfCopy(document, memoryStream);
                document.Open();

                foreach (var pdfFile in pdfFiles)
                {
                    var reader = new iTextSharp.text.pdf.PdfReader(pdfFile);
                    for (var page = 1; page <= reader.NumberOfPages; page++)
                    {
                        var importedPage = writer.GetImportedPage(reader, page);
                        writer.AddPage(importedPage);
                    }
                    reader.Close();
                }

                writer.Close();
                document.Close();

                return memoryStream.ToArray();
            }
        }
    }


   



}
        
    
