using NUnit.Framework;

namespace CacheBot.Tools.Tests
{
    [TestFixture]
    public class BotCommandParserFixture
    {
        [Test]
        public void DoesItWork()
        {
            var sut = new BotCommandParser();

            var result = sut.Parse("@CacheBot clearAll redis 12");
        }
    }
}