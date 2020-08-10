using DBMoveServer.LogUtility;
using DBMoveServer.Transfer;
using DBMoveServer.Transfer.APIModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace DBMoveServer.Web.Controllers
{
    public class ConnectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Transfer([FromForm]TransferRequest request)
        {
            try
            {
                DBHelper helper = new DBHelper();

                byte[] bt = null;
                string path = helper.CreateTables(request);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (var stream = System.IO.File.OpenRead(path))
                    {
                        stream.CopyTo(memoryStream);
                    }
                    memoryStream.Position = 0;
                    bt = memoryStream.ToArray();
                }
                return File(bt, "application/octet-stream", Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLogger("ConnectionController").Error("转换失败", ex);
                return Json(new { Code = -1, Msg = "转换失败"});
            }
        }
    }
}
