using System.Net;
using Cronbee_SDK.Exceptions;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Cronbee_SDK.Utils;

internal class NetUtils: INetInterface
{
    public const string CronbeeUrl = "https://api.cronbee.com";
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static RestClient GetClient(string url)
    {
        RestClient client = new(url, configureSerialization: config => config.UseNewtonsoftJson());
        return client;
    }

    /// <summary>
    /// get the URL with GET parameters
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static string GetRequestUrl(string url)
    {
        if (url != "/" && url.EndsWith("/"))
        {
            url = url.Remove(-1, 1);
        }

        return url;
    }

    /// <summary>
    /// Perform a GET request
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public RestResponse Get(string endpoint, int timeout = 100000)
    {
        RestRequest request = new(GetRequestUrl(endpoint))
        {
            Timeout = TimeSpan.FromMilliseconds(timeout),
            RequestFormat = DataFormat.Json
        };

        RestResponse response = GetClient(CronbeeUrl).Execute(request);

        if (!response.IsSuccessStatusCode && response.StatusCode >= HttpStatusCode.InternalServerError)
            throw new CronbeeException("Unable to contact Cronbee server.");

        return response;
    }
}