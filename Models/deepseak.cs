// DeepSeekApi.cs
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class DeepSeekApi
{
    private readonly string apiKey;
    private readonly HttpClient client;

    public DeepSeekApi(string apiKey)
    {
        this.apiKey = apiKey;
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<string> GenerateTextAsync(string modelName, string prompt)
    {
        var url = "https://api.deepseek.com/v1/chat/completions";
        var payload = new
        {
            model = modelName,
            messages = new[]
            {
                new { role = "system", content = "" },
                new { role = "user", content = prompt }
            },
            temperature = 0.7
        };

        var content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(jsonString);

        return json["choices"]?[0]?["message"]?["content"]?.ToString();
    }
}