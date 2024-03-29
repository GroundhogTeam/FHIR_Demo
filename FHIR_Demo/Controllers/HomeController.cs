﻿using FHIR_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FHIR_Demo.Controllers
{
    public class HomeController : Controller
    {
        private ConnectHelper connecthelper = new ConnectHelper();

        public ActionResult Index()
        {
            return View();
        }

        private string FHIR_Resource;
        private string SearchParameter_query;
        private int filteredResultsCount;
        private int totalResultsCount;

        public async Task<dynamic> GetResource(DataTableAjaxPostModel model, string resource , string search = null)
        {
            FHIR_Resource = resource;
            SearchParameter_query = search;

            var res = await YourCustomSearchFunc(model);

            var result = new List<dynamic>(res.Count);

            foreach (var s in res)
            {
                result.Add(s);
            };

            return JsonConvert.SerializeObject(Json(new
            {
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = result
            }).Data);
            
        }

        public async Task<IList<dynamic>> YourCustomSearchFunc(DataTableAjaxPostModel model)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                sortBy = model.columns[model.order[0].column].name;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }

            var result = await GetDataFromDbase(searchBy, take, skip, model.draw, sortBy, sortDir);
            if (result == null)
            {
                return new List<dynamic>();
            }
            return result;
        }

        public async Task<List<dynamic>> GetDataFromDbase(string searchBy, int take, int skip, int page, string sortBy, bool sortDir)
        {
            sortBy = FHIRSearchParameters_Chagne(DatatablesObjectDisplay_Change(sortBy));
            string q = "";
            q += $"_total=accurate&_count={take}&_page={page}";
            if (SearchParameter_query != null)
                q += "&"+SearchParameter_query;

            if (String.IsNullOrEmpty(searchBy))
            {
                if (String.IsNullOrEmpty(sortBy)) 
                {
                    sortBy = "_id";
                    sortDir = true;
                }
            }
            else
                q+="&_content=" + searchBy ?? "";

            q += "&_sort="+ ((sortDir == true) ? "" : "-")+ sortBy;

            Bundle SearchBundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(q, FHIR_Resource));

            Bundle SearchBundle_total = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block("_total=accurate", FHIR_Resource));

            List<dynamic> Resources = new List<dynamic>();

            if (SearchBundle.entry != null) 
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

        public string DatatablesObjectDisplay_Change (string orderName) 
        {
            string Pattern = @"\[, ].+";
            Regex regex = new Regex(Pattern);
            return regex.Replace(orderName, "")??"";
        }
    }
}