using System.IO;
using Newtonsoft.Json.Linq;

namespace CacheBot.Tools
{
    public class CommandFileParser
    {
        public void Parse(string filename)
        {
            using (var file = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                var jsonStream = file.ReadToEnd();
                var jObject = JObject.Parse(jsonStream);
            }
        }
    }
}