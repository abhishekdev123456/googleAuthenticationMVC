using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthenticationGoogle.Models
{
    public class GoogleUserOutputData
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Picture { get; set; }

        public string name { get; set; }

        public string given_name { get; set; }

        public string Access_token { get; set; }
    }
}