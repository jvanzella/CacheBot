using HtmlTemplateCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHtmlTemplateCache
{
    class Program
    {
        static void Main(string[] args)
        {
            LoginService htmlTemplateCache = new LoginService();
            htmlTemplateCache.Login("foo1234@mailinator.com", "foo1234@mailinator.com").Wait();
        }
    }
}
