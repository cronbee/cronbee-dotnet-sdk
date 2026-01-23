using Cronbee_SDK;
using Cronbee_SDK.Exceptions;
using Cronbee_SDK.Utils;
using Moq;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace Cronbee_SDK_Tests;

[TestFixture("my-monitor-id")]
public class TestInstance(string monitorId)
{
    private string _monitorId = monitorId;

    private Mock<INetInterface> _mockNetInterface;

    [SetUp]
    public void Setup()
    {
        _mockNetInterface = new Mock<INetInterface>();

        _mockNetInterface.Setup(n => n.Get($"/monitor/invalid-monitor-id"))
            .Returns(new RestResponse
            {
                IsSuccessStatusCode = false,
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = HttpStatusCode.NotFound,
                Content = JsonSerializer.Serialize(new { error = "This monitor doesn't exist." })
            });

        _mockNetInterface.Setup(n => n.Get($"/monitor/{_monitorId}"))
            .Returns(new RestResponse
            {
                IsSuccessStatusCode = true,
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = HttpStatusCode.OK,
                Content = JsonSerializer.Serialize(new { token = "mocked-token" })
            });


        _mockNetInterface.Setup(n => n.Get($"/monitor/{_monitorId}/ping"))
            .Returns(new RestResponse
            {
                IsSuccessStatusCode = true,
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = HttpStatusCode.OK,
                Content = "Pong!"
            });

        _mockNetInterface.Setup(n => n.Get(It.Is<string>(s => s.StartsWith($"/monitor/{_monitorId}/event/"))))
            .Returns(new RestResponse
            {
                IsSuccessStatusCode = true,
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = HttpStatusCode.OK,
                Content = "OK"
            });
        CronbeeSDK.Reset();
    }


    [Test]
    public void ShouldCreateSessionAndPing()
    {
        CronbeeSession session = CronbeeSDK.CreateCronbeeSession(_monitorId, _mockNetInterface.Object);
        Assert.That(session, Is.Not.Null);

        Assert.DoesNotThrow(() =>
        {
            session.Ping();
        });
    }

    [Test]
    public void ShouldCreateSessionAndCreateEvent()
    {
        CronbeeSession session = CronbeeSDK.CreateCronbeeSession(_monitorId, _mockNetInterface.Object);
        Assert.That(session, Is.Not.Null);

        Assert.DoesNotThrow(() =>
        {
            session.Event("test-event");
        });
    }

    [Test]
    public void ShouldCreateSessionAndStop()
    {
        CronbeeSession session = CronbeeSDK.CreateCronbeeSession(_monitorId, _mockNetInterface.Object);
        Assert.That(session, Is.Not.Null);

        Assert.DoesNotThrow(() =>
        {
            session.End();
        });
    }

    [Test]
    public void ShouldThrowIfMonitorIdIsInvalid()
    {
        CronbeeException ex = Assert.Throws<CronbeeException>(() =>
        {
            CronbeeSDK.CreateCronbeeSession("invalid-monitor-id", _mockNetInterface.Object);
        });

        Assert.That(ex!.Message, Is.EqualTo("This monitor doesn't exist."));
    }
}