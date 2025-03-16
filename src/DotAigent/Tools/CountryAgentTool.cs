//https://restcountries.com/v3.1/name/sweden

using DotAigent.Core;

namespace DotAigent.Tools;

public class CountryLookupTool : IFunctionTool
{
    public string Name => "LookupCountry";

    public IEnumerable<ToolParameterDescription> Parameters =>
        [
            new ToolParameterDescription("country_name", "The name of the country to lookup information about", "string", true),
        ];

    public string Description => "Looks up information about a country by name.";

    public async Task<string> ExecuteAsync(IEnumerable<ToolParameter> parameters)
    {
        var countryNameParameter = parameters.First(p => p.Name == "country_name").Value;

        string result = await LookupCountry(countryNameParameter);

        return result;
    }

    private async Task<string> LookupCountry(string countryNameParameter)
    {
            using (var client = new HttpClient())
            {
                // Get the webpage content as string
                string content = await client.GetStringAsync($"https://restcountries.com/v3.1/name/{countryNameParameter}");
                return content;
            }
    }
}
