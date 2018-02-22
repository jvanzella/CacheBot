using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HtmlTemplateCacheClient
{
    public class LoginService
    {

        private string _domain;

        public LoginService(string domain)
        {
            _domain = domain;
        }

        public async Task<HttpClient> Login(string userName, string password)
        {

            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_domain)
            };
            var response = await httpClient.GetAsync("/account/login");
            var text = await response.Content.ReadAsStringAsync();

            var html = new HtmlDocument();
            html.LoadHtml(text);

            var form = html.GetElementbyId("sign-in-form");
            var requestVerificationField = form
                .ChildNodes
                .Where(x => x.Name == "input" && x
                    .Attributes
                    .Any(attr => attr.Name == "name" && attr.Value == "__RequestVerificationToken"))
                .FirstOrDefault();

            var requestVerificationToken = requestVerificationField.Attributes["value"].Value;

            var postResponse = await httpClient.PostAsync("/account/login?returnUrl=%2ffoo", new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "__RequestVerificationToken", requestVerificationToken },
                {"UserName", userName },
                {"Password", password },
                {"RememberMe", "false" }
            }));

            if(postResponse.RequestMessage.RequestUri.GetLeftPart(UriPartial.Path) == _domain + "/foo")
            {
                return httpClient;
            }
            throw new Exception($"Logging in failed for user {userName}");
        }
    }
}
