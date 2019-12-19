using AuthenticationGoogle.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AuthenticationGoogle.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string data)
        {
            AuthorizeUser("developer.mechlintech@gmail.com");
            return View();
        }
        private void AuthorizeUser(string data)
        {
            string Url = GetAuthorizationUrl(data);
           Response.Redirect(Url, false);
        }

        [HttpGet]
        public ActionResult GoogleCallBack(string code)
        {
            var id = "developer.mechlintech@gmail.com";
            string AccessToken = string.Empty;
            string RefreshToken = ExchangeAuthorizationCode(id, code, out AccessToken);
            GoogleUserOutputData Obj = FetchEmailId(AccessToken);
            ViewBag.Email = Obj.Email;
            ViewBag.Token = Obj.Access_token;
            return View(Obj);
        }



        private string ExchangeAuthorizationCode(string userId, string code, out string accessToken)
        {
            accessToken = string.Empty;

            string ClientSecret = "BD_0fXnzRSmziaigTuwBqHHd";
            string ClientId = "436491746192-0c0nhgbbjrlfuuqntco9694toecc98bi.apps.googleusercontent.com";

            //get this value by opening your web app in browser.
            string RedirectUrl = "http://localhost:65235/Home/GoogleCallBack";

            var Content = "code=" + code +
                "&client_id=" + ClientId +
                "&client_secret=" + ClientSecret +
                "&redirect_uri=" + RedirectUrl +
                "&grant_type=authorization_code";

            var request = WebRequest.Create("https://accounts.google.com/o/oauth2/token");
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(Content);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            var Response = (HttpWebResponse)request.GetResponse();
            Stream responseDataStream = Response.GetResponseStream();
            StreamReader reader = new StreamReader(responseDataStream);
            string ResponseData = reader.ReadToEnd();
            reader.Close();
            responseDataStream.Close();
            Response.Close();
            if (Response.StatusCode == HttpStatusCode.OK)
            {
                var ReturnedToken = JsonConvert.DeserializeObject<Token>(ResponseData);

                if (ReturnedToken.refresh_token != null)
                {
                    accessToken = ReturnedToken.access_token;
                    ViewBag.AccessToken = accessToken;
                    return ReturnedToken.refresh_token;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        //private void SaveRefreshToken(int userId, string refreshToken)
        //{
        //    SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
        //    string Query = "insert into Member (UserId,RefreshToken) values(" + userId + ",'" + refreshToken + "')";
        //    SqlCommand Cmd = new SqlCommand(Query, Con);
        //    Con.Open();
        //    int Result = Cmd.ExecuteNonQuery();
        //    Con.Close();
        //}

        private GoogleUserOutputData FetchEmailId(string accessToken)
        {

            HttpClient client = new HttpClient();

            var urlProfile = "https://www.googleapis.com/oauth2/v2/userinfo?access_token="+accessToken;
            //var profile = "https://www.googleapis.com/auth/userinfo.profile?access_token=" + accessToken;
          client.CancelPendingRequests();
            GoogleUserOutputData obj = new GoogleUserOutputData();
           HttpResponseMessage output = client.GetAsync(urlProfile).Result;
            if (output.IsSuccessStatusCode)

            {

                string outputData = output.Content.ReadAsStringAsync().Result;

                obj = JsonConvert.DeserializeObject<GoogleUserOutputData>(outputData);
                obj.Access_token = accessToken;
                return obj;
            }
            return obj;

            

      }





        //}

        public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
    }




        public ActionResult LogOut(string token)
        {
            HttpClient client = new HttpClient();
            var url = "https://accounts.google.com/o/oauth2/revoke?token=" + token;
            client.CancelPendingRequests();
            GoogleUserOutputData obj = new GoogleUserOutputData();
            HttpResponseMessage output = client.GetAsync(url).Result;
            if (output.IsSuccessStatusCode)
            {
                string outputData = output.Content.ReadAsStringAsync().Result;
            }
            return View();
        }
    private string GetAuthorizationUrl(string data)
        {
            string ClientId = "436491746192-0c0nhgbbjrlfuuqntco9694toecc98bi.apps.googleusercontent.com";
            string Scopes = "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile";

            //get this value by opening your web app in browser.
            string RedirectUrl = "http://localhost:65235/Home/GoogleCallBack";

            string Url = "https://accounts.google.com/o/oauth2/auth?";
            StringBuilder UrlBuilder = new StringBuilder(Url);
            UrlBuilder.Append("client_id=" + ClientId);
            UrlBuilder.Append("&redirect_uri=" + RedirectUrl);
            UrlBuilder.Append("&response_type=" + "code");
            UrlBuilder.Append("&scope=" + Scopes);

            UrlBuilder.Append("&access_type=" + "offline");
            UrlBuilder.Append("&state=" + data);
            return UrlBuilder.ToString();
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
    }
}