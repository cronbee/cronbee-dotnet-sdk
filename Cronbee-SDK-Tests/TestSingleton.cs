using System.Net;
using System.Text.Json;
using Cronbee_SDK.Exceptions;
using Cronbee_SDK.Utils;
using Moq;

namespace Cronbee_SDK_Tests;

using Cronbee_SDK;
using RestSharp;

[TestFixture("my-monitor-id")]
public class TestSingleton(string monitorId)
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
    public void ShouldPing()
    {
        CronbeeSDK.Init(this._monitorId, _mockNetInterface.Object);

        Assert.DoesNotThrow(() =>
        {
            CronbeeSDK.Ping();
        });
    }

    [Test]
    public void ShouldCreateEvent()
    {
        CronbeeSDK.Init(this._monitorId, _mockNetInterface.Object);

        Assert.DoesNotThrow(() =>
        {
            CronbeeSDK.Event("test-event");
        });
    }

    [Test]
    public void ShouldStop()
    {
        CronbeeSDK.Init(this._monitorId, _mockNetInterface.Object);

        Assert.DoesNotThrow(() =>
        {
            CronbeeSDK.End();
        });
    }

    [Test]
    public void ShouldNotPingIfNotInitialized()
    {
        CronbeeException exception = Assert.Throws<CronbeeException>(() =>
        {
            CronbeeSDK.Ping();
        });
        
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Session is not initialized !"));
    }

    [Test]
    public void ShouldNotCreateEventIfNotInitialized()
    {
        CronbeeException exception = Assert.Throws<CronbeeException>(() =>
        {
            CronbeeSDK.Event("test-event");
        });

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Session is not initialized !"));
    }

    [Test]
    public void ShouldNotStopIfNotInitialized()
    {
        CronbeeException exception = Assert.Throws<CronbeeException>(() =>
        {
            CronbeeSDK.End();
        });

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Session is not initialized !"));
    }

    [Test]
    public void ShouldThrowExceptionIfMonitorIdInvalid()
    {
        CronbeeException ex = Assert.Throws<CronbeeException>(() =>
        {
            CronbeeSDK.CreateCronbeeSession("invalid-monitor-id", _mockNetInterface.Object);
        });

        Assert.That(ex!.Message, Is.EqualTo("This monitor doesn't exist."));
    }
}