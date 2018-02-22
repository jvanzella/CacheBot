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
            var userName = "";
            var password = "";
            var domain = "";
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
