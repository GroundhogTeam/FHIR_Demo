using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHIR_Demo.Controllers
{
    public class CookiesController
    {
        public string FHIR_url = "https://vtb02p.vghks.gov.tw/fhir-server/api/v4";
        public string FHIR_Token = "fhiruser:change-password";

        public FhirClientSettings settings = new FhirClientSettings
        {
            Timeout = 120000,
            PreferredFormat = ResourceFormat.Json,
        };

        public void Init_Cookie(HttpContextBase httpContext) 
        {
            FHIR_URL_Cookie(httpContext);
            FHIR_Token_Cookie(httpContext);
        }

        public string FHIR_URL_Cookie(HttpContextBase httpContext)
        {
            string FHIR_URL_Cookie;
            HttpCookie Cookie = httpContext.Request.Cookies["FHIR_url"] ?? null;
            if (Cookie == null || Cookie.Value == "")
            {
                Cookie = new HttpCookie("FHIR_url", FHIR_url);
            }
            FHIR_URL_Cookie = Cookie.Value;
            Cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(Cookie);
            return FHIR_URL_Cookie;
        }

        public string FHIR_URL_Cookie(HttpContextBase httpContext, string FHIR_url_update)
        {
            HttpCookie Cookie = httpContext.Request.Cookies["FHIR_url"];
            if (Cookie == null || Cookie.Value == "")
            {
                Cookie = new HttpCookie("FHIR_url", FHIR_url_update);
            }
            else
            {
                Cookie.Value = FHIR_url_update;
            }
            Cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(Cookie);
            return FHIR_url_update;
        }

        public string FHIR_Token_Cookie(HttpContextBase httpContext)
        {
            string FHIR_Token_Cookie;
            HttpCookie Cookie = httpContext.Request.Cookies["FHIR_Token"] ?? null;
            if (Cookie == null || Cookie.Value == "")
            {
                Cookie = new HttpCookie("FHIR_Token", FHIR_Token);
            }
            FHIR_Token_Cookie = Cookie.Value;
            Cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(Cookie);
            return FHIR_Token_Cookie;
        }

        public string FHIR_Token_Cookie(HttpContextBase httpContext, string FHIR_Token_update)
        {
            HttpCookie Cookie = httpContext.Request.Cookies["FHIR_Token"];
            if (Cookie == null || Cookie.Value == "")
            {
                Cookie = new HttpCookie("FHIR_Token", FHIR_Token_update);
            }
            else
            {
                Cookie.Value = FHIR_Token_update;
            }
            Cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(Cookie);
            return FHIR_Token_update;
        }

    }
}