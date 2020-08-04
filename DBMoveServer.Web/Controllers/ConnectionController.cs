using DBMoveServer.Transfer;
using DBMoveServer.Transfer.APIModel;
using Microsoft.AspNetCore.Mvc;
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
    }
}
