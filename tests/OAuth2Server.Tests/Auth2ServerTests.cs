using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OAuth2Server.Test
{
    public class OAuth2ServerTests
    {
        private readonly ITestOutputHelper output;
        private const string testDataFolder = @"TestData\";
        private IEnumerable<object> testData;

        public OAuth2ServerTests(ITestOutputHelper output)
        {
            this.output = output;
            this.testData = this.ReadFromDataFile<IEnumerable<object>>("exampleFile.json").Result;
        }

        private async Task<T> ReadFromDataFile<T>(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"{testDataFolder}{fileName}");
            string source = "";

            using (StreamReader reader = File.OpenText(path))
            {
                source = await reader.ReadToEndAsync();
            }

            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(source));
        }

        [Fact]
        public async Task Mock_GetRegisteredClients()
        {
           // place holder for initial test for now
        }
    }
}
