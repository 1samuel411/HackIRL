using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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
            if (Request.Files.Count <= 0)
            {
                return Content("Invalid request");
            }

            string url = StorageManager.UploadToStorage(Request.Files[0]);
            return Content(url);
        }

        public ActionResult SubmitForm()
        {
            #region Get Data
            string name = Request.QueryString["name"];
            if (String.IsNullOrEmpty(name))
                name = Request.Form["name"];

            string imageUrl = Request.QueryString["imageUrl"];
            if (String.IsNullOrEmpty(imageUrl))
                imageUrl = Request.Form["imageUrl"];

            string type = Request.QueryString["type"];
            if (String.IsNullOrEmpty(type))
                type = Request.Form["type"];

            string severity = Request.QueryString["severity"];
            if (String.IsNullOrEmpty(severity))
                severity = Request.Form["severity"];

            string animalType = Request.QueryString["animalType"];
            if (String.IsNullOrEmpty(animalType))
                animalType = Request.Form["animalType"];

            string lat = Request.QueryString["lat"];
            if (String.IsNullOrEmpty(lat))
                lat = Request.Form["lat"];

            string lon = Request.QueryString["lon"];
            if (String.IsNullOrEmpty(lon))
                lon = Request.Form["lon"];
            #endregion

            DAL.CreateCommand("INSERT INTO AlertTable (Name, Picture, Type, Latitude, Longitude, Severity, AnimalType) VALUES ('" + name + "', '" + imageUrl + "', '" + type + "', '" + lat + "', '" + lon + "', '" + severity + "', '" + animalType + "')");
            return Content("Success");
        }

        public ActionResult Respond()
        {
            #region Get Data
            string id = Request.QueryString["id"];
            if (String.IsNullOrEmpty(id))
                id = Request.Form["id"];
            #endregion

            DAL.CreateCommand("DELETE FROM AlertTable WHERE Id='" + id + "'");
            return Content("Success");
        }

        public ActionResult GetAlerts()
        {
            #region Get Data
            string lat = Request.QueryString["lat"];
            if (String.IsNullOrEmpty(lat))
                lat = Request.Form["lat"];

            string lon = Request.QueryString["lon"];
            if (String.IsNullOrEmpty(lon))
                lon = Request.Form["lon"];
            #endregion

            string query = "SELECT TOP 50 *, SQRT(POWER(69.1 * (Latitude - '" + lat + "'), 2) + " +
                "POWER(69.1 * ('" + lon + "' - Longitude) * COS(Latitude / 57.3), 2)) AS distance " +
                "FROM AlertTable ORDER BY distance";

            List<AlertModel> alertModel = new List<AlertModel>();
            DataTable table = DAL.CreateQuery(query);
            for(int i = 0; i < table.Rows.Count; i++)
            {
                AlertModel model = new AlertModel();
                model.id = table.Rows[i].Field<int>("Id");
                model.name = table.Rows[i].Field<string>("Name");
                model.imageUrl = table.Rows[i].Field<string>("Picture");
                model.type = table.Rows[i].Field<int>("Type");
                model.severity = table.Rows[i].Field<int>("Severity");
                model.animalType = table.Rows[i].Field<int>("AnimalType");
                model.lat = (float)table.Rows[i].Field<double>("Latitude");
                model.lon = (float)table.Rows[i].Field<double>("Longitude");
                alertModel.Add(model);
            }

            string data = JsonConvert.SerializeObject(alertModel.ToArray());

            return Content(data);
        }
    }
}