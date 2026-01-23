using Cronbee_SDK.Exceptions;
using Cronbee_SDK.Utils;
using RestSharp;
using System.Text.Json;

namespace Cronbee_SDK
{
    public class CronbeeSDK
    {
        private static CronbeeSession? _session;

        /// <summary>
        /// Initialize the Cronbee-SDK singleton with the specified monitor ID
        /// </summary>
        /// <param name="monitorId">The monitor ID to use for the session</param>
        /// <param name="netInterface">The instance of NetInterface needed to perform HTTP requests</param>
        /// <returns></returns>
        public static CronbeeSession Init(string monitorId, INetInterface? netInterface = null)
        {
            _session = CreateCronbeeSession(monitorId, netInterface);
            return _session;
        }

        /// <summary>
        /// Create a new 
        /// </summary>
        /// <param name="monitorId"></param>
        /// <param name="netInterface">The instance of NetInterface needed to perform HTTP requests</param>
        /// <returns></returns>
        public static CronbeeSession CreateCronbeeSession(string monitorId, INetInterface? netInterface = null)
        {
            INetInterface _interface = netInterface ?? new NetUtils();
            RestResponse response = _interface.Get($"/monitor/{monitorId}");
            CronbeeSession.ParseResponse(response);
            string? token = string.Empty;


            if (JsonDocument.Parse(response.Content).RootElement
                    .TryGetProperty("token", out JsonElement tokenProperty))
            {
                token = tokenProperty.GetString();
            }

            if (string.IsNullOrEmpty(token))
                throw new CronbeeException("Unable to retrieve Cronbee token !");

            return new CronbeeSession(monitorId, token, _interface);
        }

        /// <summary>
        /// Ping the specified monitor
        /// </summary>
        public static void Ping()
        {
            if (_session is null)
                throw new CronbeeException("Session is not initialized !");
            _session?.Ping();
        }

        /// <summary>
        /// Trigger an event for the current monitor session
        /// </summary>
        /// <param name="eventName">The event name to be triggered</param>
        public static void Event(string eventName)
        {
            if (_session is null)
                throw new CronbeeException("Session is not initialized !");
            _session?.Event(eventName);
        }

        /// <summary>
        /// Close the current monitor session
        /// </summary>
        public static void End()
        {
            Event("stop");
        }

        /// <summary>
        /// Reset the current session
        /// </summary>
        public static void Reset()
        {
            _session = null;
        }
    }
}
