using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FHIR_Demo.Models
{
    public class ConnectHelper
    {
        public async Task<string> GetandShare_Block(string query, string Resource)
        {
            var url = ConfigurationManager.AppSettings.Get("FHIRAPI") + "/" + Resource + "?" + query;
            var Username = ConfigurationManager.AppSettings.Get("Username");
            var Password = ConfigurationManager.AppSettings.Get("Password");

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)/*回傳500*/
            {
                return "500";
            }
            else
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            //return response.Content.ReadAsStringAsync().Result;
        }
    }
}