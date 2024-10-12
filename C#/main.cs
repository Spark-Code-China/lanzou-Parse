using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
 
class LZY
{
   private static readonly HttpClient client = new HttpClient();
 
    static async Task Get_lzy(string url, string password)
    {
       var headers = new Dictionary<string, string>
       {
           { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36 Edg/121.0.0.0" }
       };
 
       var response = await client.GetStringAsync(url);
       var urlPattern = new Regex(@"url\s*:\s*'(/ajaxm\.php\?file=\d+)'");
       var urlMatch = urlPattern.Match(response).Groups[1].Value;
 
       var skdkldsPattern = new Regex(@"var\s+skdklds\s*=\s*'([^']*)';");
       var skdkldsMatch = skdkldsPattern.Match(response).Groups[1].Value;
 
       Console.WriteLine(urlMatch + " " + skdkldsMatch);
 
       var data = new FormUrlEncodedContent(new[]
       {
           new KeyValuePair<string, string>("action", "downprocess"),
           new KeyValuePair<string, string>("sign", skdkldsMatch),
           new KeyValuePair<string, string>("p", password)
       });
 
       headers["Referer"] = url;
       client.DefaultRequestHeaders.Clear();
       foreach (var header in headers)
       {
           client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
       }
 
       var response2 = await client.PostAsync($"https://{ReDomain(url)}{urlMatch}", data);
       var responseString = await response2.Content.ReadAsStringAsync();
       var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
 
       string dom = responseData.dom;
       string fileUrl = responseData.url;
       string fullUrl = $"{dom}/file/{fileUrl}";
       Console.WriteLine(fullUrl);
 
       var requestHeaders = new Dictionary<string, string>()
       {
           { "accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7" },
           { "accept-language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6" },
           { "sec-ch-ua", "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Microsoft Edge\";v=\"122\"" },
           { "sec-ch-ua-mobile", "?0" },
           { "sec-ch-ua-platform", "\"Windows\"" },
           { "sec-fetch-dest", "document" },
           { "sec-fetch-mode", "navigate" },
           { "sec-fetch-site", "none" },
           { "sec-fetch-user", "?1" },
           { "upgrade-insecure-requests", "1" },
           { "cookie", "down_ip=1" }
       };
 
       string finalRedirectUrl;
 
       var requestMessage = new HttpRequestMessage(HttpMethod.Get, fullUrl);
       foreach (var header in requestHeaders)
       {
           requestMessage.Headers.Add(header.Key, header.Value);
       }
 
       var response3 = await client.SendAsync(requestMessage);
 
       if (response3.IsSuccessStatusCode)
       {
           finalRedirectUrl = response3.RequestMessage.RequestUri.AbsoluteUri;
       }
       else
       {
           Console.WriteLine("请求失败，状态码：" + response3.StatusCode);
           return;
       }
 
       Console.WriteLine("最终的重定向URL: " + finalRedirectUrl);
       //DownloadFile(finalRedirectUrl);
   }
 
   private static string ReDomain(string url)
   {
       var patternDomain = @"https?://([^/]+)";
       var match = Regex.Match(url, patternDomain);
       return match.Success ? match.Groups[1].Value : null;
   }
}