using System.Text.RegularExpressions;

var urls = new List<string?>();
var messages = new Queue<Message>();

Console.WriteLine("Please enter a URL address");
var basisUrl = Console.ReadLine();

messages.Enqueue(new Message(basisUrl));

while (messages.Count > 0)
{
    var message = messages.Dequeue();
    Console.WriteLine("Called : " + message.Url);
    UrlCollection(message.Url);
}

Console.WriteLine("Messaging Queue is empty");

Console.ReadLine();

async Task<string> CallUrl(Uri clientUrl)
{
    var httpClient = CreateClient();
    var httpResponseMessage = await httpClient.GetAsync(clientUrl);
    var contents = await httpResponseMessage.Content.ReadAsStringAsync();

    return contents;
}

HttpClient CreateClient()
{
    return new HttpClient();
}

void UrlCollection(string? url)
{
    if (url == null) return;
    var clientUrl = new Uri(url);
    var result = CallUrl(clientUrl);

    var m = Regex.Matches(result.Result, "(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");

    foreach (var match in m)
    {
        var matchedUrl = match.ToString();
        var linkedUrl = GetLinkedUrl(matchedUrl);
        if (linkedUrl == null) continue;
        if (urls == null || urls.Contains(linkedUrl)) continue;
        urls?.Add(linkedUrl);
        messages.Enqueue(new Message(linkedUrl));
    }
}

string? GetLinkedUrl(string? url)
{
    if (url == null) return url;
    
    if (url.Contains("https://"))
    {
        return url.Contains(basisUrl) ? url : null;
    }
    
    if (url.StartsWith("/"))
    {
        url = basisUrl + url;
    }
    else
    {
        url = basisUrl + "/" + url; 
    }

    return url; 
}

internal class Message
{
    public string? Url { get; }

    public Message(string? url)
    {
        this.Url = url;
    }
}