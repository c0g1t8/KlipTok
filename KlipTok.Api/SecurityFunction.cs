using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KlipTok.Twitch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace KlipTok.Api {

  public class SecurityFunction {

    static string TwitchClientId = System.Environment.GetEnvironmentVariable("twitch.clientid");
    static string TwitchSecret = System.Environment.GetEnvironmentVariable("twitch.secret");
    private readonly HttpClient _Client;

    public SecurityFunction(IHttpClientFactory clientFactory)
    {
        _Client = clientFactory.CreateClient();
    }

    [FunctionName("auth")]
    public async Task<IActionResult> Authenticate([HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest request, ILogger log) {

      var code = request.Query["code"];
      var uri = request.Query["redirect_uri"];

      var targetUrl = $"https://id.twitch.tv/oauth2/token" +
          $"?client_id={TwitchClientId}" +
          $"&client_secret={TwitchSecret}" +
          $"&code={code}" +
          "&grant_type=authorization_code" +
          $"&redirect_uri={uri}";
      
      var results = await _Client.PostAsync(targetUrl, new StringContent(""));
      results.EnsureSuccessStatusCode();

      return new OkObjectResult(await results.Content.ReadAsStringAsync());

    } 

  }

}