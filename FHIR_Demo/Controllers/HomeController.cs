using FHIR_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;//new0609
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FHIR_Demo.Controllers
{
    public class HomeController : Controller
    {
        private ConnectHelper connecthelper = new ConnectHelper();
        public static string select_url;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            var selectList = new List<SelectListItem>()
            {
                new SelectListItem {Text="Patient", Value="value-1" },
                new SelectListItem {Text="Encounter", Value="value-2" },
                new SelectListItem {Text="Observation", Value="value-3" },
                new SelectListItem {Text="Procedure", Value="value-3" },
                new SelectListItem {Text="Condition", Value="value-3" },
                new SelectListItem {Text="MedicationRequest", Value="value-3" },
                new SelectListItem {Text="DiagnosticReport", Value="value-3" },                
                new SelectListItem {Text="ServiceRequest", Value="value-3" },
            };

            //預設選擇哪一筆
            selectList.Where(q => q.Value == "value-2").First().Selected = true;

            ViewBag.SelectList = selectList;

            var selectList222 = new List<SelectListItem>()
            {
                new SelectListItem {Text="Patient", Value="value-1" },
                new SelectListItem {Text="Encounter", Value="value-2" },
                new SelectListItem {Text="Observation", Value="value-3" },
                new SelectListItem {Text="Procedure", Value="value-3" },
                new SelectListItem {Text="Condition", Value="value-3" },
                new SelectListItem {Text="MedicationRequest", Value="value-3" },
                new SelectListItem {Text="DiagnosticReport", Value="value-3" },
                new SelectListItem {Text="ServiceRequest", Value="value-3" },
            };

            //預設選擇哪一筆
            selectList222.Where(q => q.Value == "value-2").First().Selected = true;

            ViewBag.SelectList222 = selectList222;


            return View();

        }

        [HttpGet]
        public async Task<string> Get_MultipleSearch(string sendalltext)
        {
            //a = Request.Form["sendAlltext"];
            var url = ConfigurationManager.AppSettings.Get("FHIRMULSEARCHAPI") + sendalltext;
            //var Authorization = ConfigurationManager.AppSettings.Get("Authorization");

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;//憑證一定要通過
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;//版本
            HttpClient client = new HttpClient(); //請求
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Authorization);

            //var response = await client.GetAsync(url);

            var response = await client.GetAsync(url);
            var result = response.Content.ReadAsStringAsync().Result;
            return result;
          
        }


        [HttpPost]
        public async Task<ActionResult> Index2(string sendalltext)
        {

            var Getomi_json = await Get_MultipleSearch(sendalltext);
            return Json(Getomi_json);
        }


        public ActionResult test01()
        {
            return View();
        }

        private string FHIR_Resource;
        private string SearchParameter_query;
        private int filteredResultsCount;
        private int totalResultsCount;
        private string page2;
        public async Task<dynamic> GetResource(DataTableAjaxPostModel model, string resource, string search = null, string div_card2 = null)
        {
            //int pagesize = 10;
            //int pagecurrent = page < 1 ? 1 : page;
            FHIR_Resource = resource;
            SearchParameter_query = search;//搜尋條件

            if (div_card2 == null)
            {
                div_card2 = "1";
                page2 = div_card2;
            }
            else
            {
                page2 = div_card2;
            }


            var res = await YourCustomSearchFunc(model, page2);

            var result = new List<dynamic>(res.Count);
            //var result = new List<dynamic>();

            foreach (var s in res)
            {
                result.Add(s);
            };
            var output = JsonConvert.SerializeObject(Json(new
            {
                //draw = model.draw,
                recordsTotal = totalResultsCount,//總筆數
                recordsFiltered = filteredResultsCount,//總過濾筆數
                data = result
                //data = filteredResultsCount

            }).Data);

            return JsonConvert.SerializeObject(Json(new
            {
                //draw = model.draw,
                recordsTotal = totalResultsCount,//總筆數
                recordsFiltered = filteredResultsCount,//總過濾筆數
                data = result,
                //data = filteredResultsCount
            }).Data);

        }



        public async Task<IList<dynamic>> YourCustomSearchFunc(DataTableAjaxPostModel model, string page2)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            //var take = 332;
            var skip = model.start;
            var page = model.start / 10 + 1;
            string sortBy = "";
            bool sortDir = true;
            string numberpageee = page2;
            if (model.order != null)
            {
                sortBy = model.columns[model.order[0].column].name;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }

            var result = await GetDataFromDbase(searchBy, take, skip, page, sortBy, sortDir, numberpageee);
            //var result = await GetDataFromDbase(searchBy, take, skip, sortBy, sortDir);

            if (result == null)
            {
                return new List<dynamic>();
            }
            return result;
        }

        //public async Task<List<dynamic>> GetDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir)
        public async Task<List<dynamic>> GetDataFromDbase(string searchBy, int take, int skip, int page, string sortBy, bool sortDir, string numberpageee)
        {
            sortBy = FHIRSearchParameters_Chagne(DatatablesObjectDisplay_Change(sortBy));
            string q = "";
            //q += $"_total=accurate&_count={take}&_page={numberpageee}";
            q += $"_total=accurate&_count={take}&_page={page}";

            if (SearchParameter_query != null)
                q += "&" + SearchParameter_query;

            if (String.IsNullOrEmpty(searchBy))
            {
                if (String.IsNullOrEmpty(sortBy))
                {
                    sortBy = "_id";
                    sortDir = true;
                }
            }
            else
                q += "&_content=" + searchBy ?? "";

            q += "&_sort=" + ((sortDir == true) ? "" : "-") + sortBy;

            Bundle SearchBundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(q, FHIR_Resource));

            Bundle SearchBundle_total = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block("_total=accurate", FHIR_Resource));

            List<dynamic> Resources = new List<dynamic>();

            if (SearchBundle.entry != null)//資料十筆產生處
            {
                foreach (var entry in SearchBundle.entry)
                {
                    Resources.Add(entry.resource);
                }
            }

            filteredResultsCount = SearchBundle.total ?? 0;

            totalResultsCount = SearchBundle_total.total ?? 0;

            return Resources;
        }

        public string FHIRSearchParameters_Chagne(string parameter)
        {
            parameter = parameter.ToLower();
            switch (parameter)
            {
                case "id":
                    parameter = "_id";
                    break;
                case "lastupdated":
                    parameter = "_lastUpdated";
                    break;
                case "tag":
                    parameter = "_tag";
                    break;
                case "profile":
                    parameter = "_profile";
                    break;
                case "security":
                    parameter = "_security";
                    break;
                case "text":
                    parameter = "_text";
                    break;
                case "content":
                    parameter = "_content";
                    break;
                case "list":
                    parameter = "_list";
                    break;
                case "has":
                    parameter = "_has";
                    break;
                case "type":
                    parameter = "_type";
                    break;
                case "query":
                    parameter = "_query";
                    break;
            }
            return parameter;
        }

        public string DatatablesObjectDisplay_Change(string orderName)
        {
            string Pattern = @"\[, ].+";
            Regex regex = new Regex(Pattern);
            return regex.Replace(orderName, "") ?? "";
        }
    }
}