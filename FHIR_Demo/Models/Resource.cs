using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FHIR_Demo.Models
{
    public class Bundle
    {
        public string resourceType { get; set; }
        public string type { get; set; }
        public int? total { get; set; }
        public List<entry> entry { get; set; } = new List<entry>();
    }
    public class entry
    {
        public string fullUrl { get; set; }
        public dynamic resource { get; set; }
    }



}