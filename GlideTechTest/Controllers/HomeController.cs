using LumenWorks.Framework.IO.Csv;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlideTechTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            var client = new RestClient("https://mock.glidebusiness.co.uk/features");
            client.Timeout = -1;

            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {

                    if (upload.FileName.EndsWith(".csv"))
                    {
                        Stream stream = upload.InputStream;
                        DataTable csvTable = new DataTable();
                        using (CsvReader csvReader =
                            new CsvReader(new StreamReader(stream), true))
                        {
                            csvTable.Load(csvReader);
                        }

                        foreach (DataRow row in csvTable.Rows)
                        {
                            // insert row[1] (Name) to replace Service Location
                            //insert row[13] & [14] (Latitude & Longtitude)  to replace coordinates 
                            //string json = "{\r\n \"type\": \"Feature\",\r\n \"geometry\": {\r\n   \"type\": \"Point\",\r\n   \"coordinates\": [-86.258, 34.3925]\r\n },\r\n \"glide\": { \r\n     \"layer\": 26, \r\n     \"plan\": 292 \r\n},\r\n \"properties\": {\"Name\": \"Service Location\"}\r\n}\r\n";

                            string json = "{\r\n \"type\": \"Feature\",\r\n \"geometry\": {\r\n   \"type\": \"Point\",\r\n   \"coordinates\": [" + row[13] + "," + row[14] + "]\r\n },\r\n \"glide\": { \r\n     \"layer\": 26, \r\n     \"plan\": 292 \r\n},\r\n \"properties\": {\"Name\": \"" + row[1] + "\"}\r\n}\r\n";

                            var request = new RestRequest(Method.POST);
                            request.AddHeader("token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJXZWxsIERvbmUgb24gRGVjb2RpbmcgdGhpcy4uLiIsIm5hbWUiOiJHbGlkZSBNb2NrIEludGVydmlldyIsImlhdCI6MTY0MDk5NTIwMH0.TEOBb0Kyij-BP8c7wxeW8RncjZi8NTrNlXkqi9weOgA");
                            request.AddHeader("Content-Type", "application/json");
                            request.AddParameter("application/json", json, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);
                            //Console.WriteLine(response.Content);
                            //Debug.WriteLine(json);
                        }


                        return View(csvTable);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }
    }
}