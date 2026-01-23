using RestSharp;

namespace Cronbee_SDK.Utils;

public interface INetInterface
{
    public RestResponse Get(string endpoint, int timeout = 100000);
}