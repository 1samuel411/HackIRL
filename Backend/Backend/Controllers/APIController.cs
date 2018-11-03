using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Backend.Controllers
{
    public class APIController : Controller
    {
        // GET: API
        public ActionResult Index()
        {
            return View();
        }

        // POST: Set Icon
        public ActionResult UploadImage()
        {
            #region Get Data
            string sessionToken = Request.QueryString["sessionToken"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            #endregion

            if (Request.Files.Count <= 0)
            {
                return Content("Invalid request");
            }

            string url = StorageManager.UploadToStorage(Request.Files[0]);
            return Content(url);
        }

        public ActionResult SubmitForm()
        {

        }
    }
}