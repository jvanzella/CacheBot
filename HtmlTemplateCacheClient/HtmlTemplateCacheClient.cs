using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlTemplateCache
{
    public class HtmlTemplateCacheClient
    {

        public async Task Clear()
        {
            var userName = "rc.tester103@nowhere.com";
            var password = "Password123";
            var domain = "https://qadesign.staples.com";
            LoginService loginService = new LoginService(domain);
            var client = await loginService.Login(userName, password);

            var response = await client.GetAsync("/SiteAdmin/ClearHTMLTemplateCache");

            if(response.RequestMessage.RequestUri.GetLeftPart(UriPartial.Path) != domain + "/SiteAdmin/ClearHTMLTemplateCache" )
            {
                throw new Exception("Clearing cache failed");
            }
        }
    }
}
