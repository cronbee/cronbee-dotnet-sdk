using Cronbee_SDK.Exceptions;
using Cronbee_SDK.Utils;
using System.Net;
using System.Text.Json;
using RestSharp;

namespace Cronbee_SDK;

public class CronbeeSession
{
    private readonly string _token;
    private readonly string _monitorId;
    private readonly INetInterface _netInterface;

    internal CronbeeSession(string monitorId, string token, INetInterface netInterface)
    {
        _monitorId = monitorId;
        _token = token;
        _netInterface = netInterface;
    }

    /// <summary>
    /// Ping the specified monitor
    /// </summary>
    public void Ping()
    {
        ParseResponse(_netInterface.Get($"/monitor/{_monitorId}/ping"));
    }

    /// <summary>
    /// Trigger an event for the current monitor session
    /// </summary>
    /// <param name="eventName">The event name to be triggered</param>
    public void Event(string eventName)
    {
        ParseResponse(_netInterface.Get($"/monitor/{_monitorId}/event/{eventName}?token={_token}"));
    }

    /// <summary>
    /// Close the current monitor session
    /// </summary>
    public void End()
    {
        Event("stop");
    }

    internal static void ParseResponse(RestResponse response)
    {
        if (!response.IsSuccessStatusCode && response.StatusCode < HttpStatusCode.InternalServerError &&
            response.Content != null)
        {
            string errorMessage = "Unexpected error.";
            if (JsonDocument.Parse(response.Content).RootElement
                .TryGetProperty("error", out JsonElement tokenProperty))
            {
                errorMessage = tokenProperty.GetString();
            }
            throw new CronbeeException(errorMessage);
        }
    }
}