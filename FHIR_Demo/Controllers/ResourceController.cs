using FHIR_Demo.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FHIR_Demo.Controllers
{
    public class ResourceController : Controller
    {
        private ConnectHelper connecthelper = new ConnectHelper();

        public async Task<ActionResult> Index(string res, string id)
        {
            string[] Resource = new string[] { "Patient", "Encounter", "Observation", "MedicationRequest", "Procedure", "Condition", "DiagnosticReport", "ServiceRequest" };
            if (Resource.Contains(res) && id != null)
            {
                string patient_id = "";
                string resourcetype = "";//判斷resourcetype的變數
                resourcetype = res;

                Bundle Res_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block("_id=" + id, res));


                List<dynamic> Res_Search = new List<dynamic>();

                foreach (var entry in Res_Bundle.entry)
                {
                    var entry_res = entry.resource;
                    Res_Search.Add(entry_res);
                    if (res == "Patient")
                    {
                        patient_id = id;
                    }
                    else if (entry_res["subject"] != null)
                    {
                        patient_id = entry.resource["subject"]["reference"].ToString().Split('/')[1];
                    }
                }

                ViewBag.Res_Search = Res_Search;

                string res_detail_query = "";
                var query_list = new List<string>();
                if (res == "Encounter")
                {
                    query_list.Add("_id=" + id);
                    query_list.Add("_revinclude=ServiceRequest:encounter");
                    query_list.Add("_revinclude=Observation:encounter");
                    query_list.Add("_revinclude=MedicationRequest:encounter");
                    query_list.Add("_revinclude=Procedure:encounter");
                    query_list.Add("_revinclude=Condition:encounter");
                    query_list.Add("_revinclude=DiagnosticReport:encounter");
                    query_list.Add("_total=accurate");
                }
                else if (res == "Observation")
                {
                    query_list.Add("patient=" + patient_id);
                    if (Res_Search[0].code.coding != null)
                    {
                        if (Res_Search[0].code.coding.Count > 0)
                        {
                            query_list.Add($"code={Res_Search[0].code.coding[0].code}");
                        }
                        else
                        {
                            query_list.Add($"code:text={Res_Search[0].code.text}");
                        }
                    }
                    else
                    {
                        query_list.Add($"code:text={Res_Search[0].code.text}");
                    }
                    query_list.Add("_count=200");
                    query_list.Add("_total=accurate");
                    query_list.Add("_sort=date");
                }
                res_detail_query = string.Join("&", query_list);

                Bundle Res_detail_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(res_detail_query, res));

                List<dynamic> Res_detail_list = new List<dynamic>();

                foreach (var entry in Res_detail_Bundle.entry)
                {
                    var entry_res = entry.resource;
                    Res_detail_list.Add(entry_res);
                }

                ViewBag.Resource_detail = Res_detail_list;


                List<dynamic> Patient_Search_reosurces = new List<dynamic>();

                if (patient_id != "" && Res_Bundle.entry.Count > 0)
                {
                    if(resourcetype == "Procedure")
                    {
                        var patient_query_list = new List<string>();
                        patient_query_list.Add("_id=" + patient_id);
                        //patient_query_list.Add("_revinclude=Encounter:patient");
                        //patient_query_list.Add("_revinclude=Observation:patient");
                        //patient_query_list.Add("_revinclude=MedicationRequest:patient");
                        patient_query_list.Add("_revinclude=Procedure:patient");
                        //patient_query_list.Add("_revinclude=Condition:patient");
                        //patient_query_list.Add("_revinclude=DiagnosticReport:patient");
                        patient_query_list.Add("_total=accurate");

                        var query = string.Join("&", patient_query_list);

                        Bundle Patient_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(query, "Patient"));


                        foreach (var entry in Patient_Bundle.entry)
                        {
                            Patient_Search_reosurces.Add(entry.resource);
                        }
                    }
                    else if (resourcetype == "Observation")
                    {
                        var patient_query_list = new List<string>();
                        patient_query_list.Add("_id=" + patient_id);
                        //patient_query_list.Add("_revinclude=Encounter:patient");
                        patient_query_list.Add("_revinclude=Observation:patient");
                        //patient_query_list.Add("_revinclude=MedicationRequest:patient");
                        //patient_query_list.Add("_revinclude=Procedure:patient");
                        //patient_query_list.Add("_revinclude=Condition:patient");
                        //patient_query_list.Add("_revinclude=DiagnosticReport:patient");
                        patient_query_list.Add("_total=accurate");

                        var query = string.Join("&", patient_query_list);

                        Bundle Patient_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(query, "Patient"));


                        foreach (var entry in Patient_Bundle.entry)
                        {
                            Patient_Search_reosurces.Add(entry.resource);
                        }
                    }
                    else if (resourcetype == "Patient")
                    {
                        var patient_query_list = new List<string>();
                        patient_query_list.Add("_id=" + patient_id);
                        //patient_query_list.Add("_revinclude=Encounter:patient");
                        //patient_query_list.Add("_revinclude=Observation:patient");
                        //patient_query_list.Add("_revinclude=MedicationRequest:patient");
                        //patient_query_list.Add("_revinclude=Procedure:patient");
                        //patient_query_list.Add("_revinclude=Condition:patient");
                        //patient_query_list.Add("_revinclude=DiagnosticReport:patient");
                        patient_query_list.Add("_total=accurate");

                        var query = string.Join("&", patient_query_list);

                        Bundle Patient_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(query, "Patient"));


                        foreach (var entry in Patient_Bundle.entry)
                        {
                            Patient_Search_reosurces.Add(entry.resource);
                        }
                    }
                    else if (resourcetype == "Encounter")
                    {
                        var patient_query_list = new List<string>();
                        patient_query_list.Add("_id=" + patient_id);
                        patient_query_list.Add("_revinclude=Encounter:patient");
                        //patient_query_list.Add("_revinclude=Observation:patient");
                        //patient_query_list.Add("_revinclude=MedicationRequest:patient");
                        //patient_query_list.Add("_revinclude=Procedure:patient");
                        //patient_query_list.Add("_revinclude=Condition:patient");
                        //patient_query_list.Add("_revinclude=DiagnosticReport:patient");
                        patient_query_list.Add("_total=accurate");

                        var query = string.Join("&", patient_query_list);

                        Bundle Patient_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(query, "Patient"));


                        foreach (var entry in Patient_Bundle.entry)
                        {
                            Patient_Search_reosurces.Add(entry.resource);
                        }
                    }
                    else if (resourcetype == "MedicationRequest")
                    {
                        var patient_query_list = new List<string>();
                        patient_query_list.Add("_id=" + patient_id);
                        //patient_query_list.Add("_revinclude=Encounter:patient");
                        //patient_query_list.Add("_revinclude=Observation:patient");
                        patient_query_list.Add("_revinclude=MedicationRequest:patient");
                        //patient_query_list.Add("_revinclude=Procedure:patient");
                        //patient_query_list.Add("_revinclude=Condition:patient");
                        //patient_query_list.Add("_revinclude=DiagnosticReport:patient");
                        patient_query_list.Add("_total=accurate");

                        var query = string.Join("&", patient_query_list);

                        Bundle Patient_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(query, "Patient"));


                        foreach (var entry in Patient_Bundle.entry)
                        {
                            Patient_Search_reosurces.Add(entry.resource);
                        }
                    }
                    else if (resourcetype == "Condition")
                    {
                        var patient_query_list = new List<string>();
                        patient_query_list.Add("_id=" + patient_id);
                        //patient_query_list.Add("_revinclude=Encounter:patient");
                        //patient_query_list.Add("_revinclude=Observation:patient");
                        //patient_query_list.Add("_revinclude=MedicationRequest:patient");
                        //patient_query_list.Add("_revinclude=Procedure:patient");
                        patient_query_list.Add("_revinclude=Condition:patient");
                        //patient_query_list.Add("_revinclude=DiagnosticReport:patient");
                        patient_query_list.Add("_total=accurate");

                        var query = string.Join("&", patient_query_list);

                        Bundle Patient_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(query, "Patient"));


                        foreach (var entry in Patient_Bundle.entry)
                        {
                            Patient_Search_reosurces.Add(entry.resource);
                        }
                    }
                    else if (resourcetype == "DiagnosticReport")
                    {
                        var patient_query_list = new List<string>();
                        patient_query_list.Add("_id=" + patient_id);
                        //patient_query_list.Add("_revinclude=Encounter:patient");
                        //patient_query_list.Add("_revinclude=Observation:patient");
                        //patient_query_list.Add("_revinclude=MedicationRequest:patient");
                        //patient_query_list.Add("_revinclude=Procedure:patient");
                        //patient_query_list.Add("_revinclude=Condition:patient");
                        patient_query_list.Add("_revinclude=DiagnosticReport:patient");
                        patient_query_list.Add("_total=accurate");

                        var query = string.Join("&", patient_query_list);

                        Bundle Patient_Bundle = JsonConvert.DeserializeObject<Bundle>(await connecthelper.GetandShare_Block(query, "Patient"));


                        foreach (var entry in Patient_Bundle.entry)
                        {
                            Patient_Search_reosurces.Add(entry.resource);
                        }
                    }

                }
                ViewBag.Resources = Patient_Search_reosurces;
                return View();
            }
            else
            {
                if (Request.UrlReferrer == null)
                    return RedirectToAction("Index", "Home");
                else
                    return Redirect(Request.UrlReferrer.ToString());
            }
        }
    }
}