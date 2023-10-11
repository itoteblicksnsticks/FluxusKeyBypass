using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient Client = new HttpClient();

    static async Task Main()
    {
        Console.Write("Enter your HWID: ");
        string hwid = Console.ReadLine();

        var urls = new[]
        {
            $"https://flux.li/windows/start.php?updated_browser=true&HWID={hwid}",
            "https://fluxteam.net/windows/checkpoint/check1.php",
            "https://fluxteam.net/windows/checkpoint/check2.php",
            "https://fluxteam.net/windows/checkpoint/main.php"
        };

        // HTTP Requests
        try
        {
            await Task.WhenAll(urls.Select(Request)); // Concurrent HTTP requests
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            Console.ReadKey();
            return;
        }

        // Load & Parse HTML
        var document = new HtmlDocument();
        try
        {
            document.LoadHtml((await Request(urls.Last())).Content);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Failed to load the document: {e.Message}");
            Console.ReadKey();
            return;
        }

        // Extract Key and Display to User
        var keyNode = document.DocumentNode.SelectSingleNode("//main/code[2]");
        Console.WriteLine($"\nYour key is: {keyNode?.InnerText.Trim() ?? "(unknown error)"}\nPress any key to exit...");
        Console.ReadKey();
    }

    static async Task<Response> Request(string url)
    {
        // Configure HTTP Client Headers
        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36");
        Client.DefaultRequestHeaders.Referrer = new Uri("https://linkvertise.com/");

        // Perform HTTP Request
        try
        {
            var content = await Client.GetStringAsync(url);
            Console.WriteLine($"Bypassed {url}\n");
            return new Response(url, content);
        }
        catch (HttpRequestException e)
        {
            throw new HttpRequestException($"Failed to fetch the URL: {url}. Error: {e.Message}");
        }
    }

    class Response
    {
        public string Url { get; }
        public string Content { get; }
        public Response(string url, string content) => (Url, Content) = (url, content);
    }
}
