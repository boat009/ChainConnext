using ChainConnext.Shared;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ChainConnext.Server.Controllers
{
    [DisableRequestSizeLimit]
    public partial class UploadController : Controller
    {
        private readonly IWebHostEnvironment environment;

        public UploadController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        [HttpPost("upload/single")]
        public IActionResult Single(IFormFile file)
        {
            try
            {
                ExecResult Rs = new ExecResult();
                Rs.Data = file;
                if (file.FileName.ToLower().EndsWith(".xlsx"))
                {
                    DataSet ds = new DataSet();
                    using (var stream = file.OpenReadStream())
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(
                                new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true
                                    }
                                }
                                );
                            ds = result;
                        }
                    }
                    //using (TextWriter sw = new StringWriter())
                    //{
                    //    ds.WriteXml(sw);
                    //    Rs.JsonData = sw.ToString();
                    //}
                    Rs.JsonData = BaseShared.DataTableToJson(ds.Tables[0]);
                    //Rs.Data = ds;
                }
                else
                {

                }
                //Put your code here
                return Ok(Rs);
                //return Ok(file.OpenReadStream());
                //return Ok(new { Completed = true });
                //var readStream = file.OpenReadStream();
                //var buf = new byte[readStream.Length];
                //var ms = new MemoryStream(buf);
                //await readStream.CopyToAsync(ms);
                //var buffer = ms.ToArray();
                //return Ok(Convert.ToBase64String(buffer));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("upload/image")]
        public IActionResult Image(IFormFile file)
        {
            try
            {
                // Used for demo purposes only
                DeleteOldFiles();

                var fileName = $"upload-{DateTime.Today.ToString("yyyy-MM-dd")}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(environment.WebRootPath, fileName), FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);

                    // Return the URL of the file
                    var url = Url.Content($"~/{fileName}");

                    return Ok(new { Url = url });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        void DeleteOldFiles()
        {
            foreach (var file in Directory.GetFiles(environment.WebRootPath))
            {
                var fileName = Path.GetFileName(file);

                if (fileName.StartsWith("upload-") && !fileName.StartsWith($"upload-{DateTime.Today.ToString("yyyy-MM-dd")}"))
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch
                    {

                    }
                }
            }
        }

        [HttpPost("upload/multiple")]
        public IActionResult Multiple(IFormFile[] files)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("upload/{id}")]
        public IActionResult Post(IFormFile[] files, int id)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("upload/specific")]
        public IActionResult Specific(IFormFile myName)
        {
            try
            {
                // Put your code here
                return Ok(new { Completed = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
