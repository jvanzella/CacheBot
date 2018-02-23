using NUnit.Framework;

namespace CacheBot.Tools.Tests
{
    [TestFixture]
    public class CommandFileParserFixture
    {
        private const string JsonString = @"{
                    ""@cacheBot"": {
                    ""commandType"": ""command"" ,
                    ""clearAll"": {
                        ""commandType"": ""command"",
                        ""redis"": {
                            ""commandType"": ""command"",
                            ""databaseId"": {
                                ""commandType"": ""variable""
                            }
                        }
                    }
                }
            }";
        [Test]
        public void ShouldParseAStringProperly()
        {
            var sut = new CommandFileParser();

            var result = sut.ParseString(JsonString);
        }
    }
}