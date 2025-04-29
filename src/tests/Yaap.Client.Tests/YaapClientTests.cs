namespace Yaap.Client.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;
using Xunit.Abstractions;

using Yaap.Client;
using Yaap.Client.Abstractions;
using Yaap.Core.Models;
using Yaap.TestCommon;

public class YaapClientTests(ITestOutputHelper outputHelper) : YaapTest(outputHelper)
{
    private sealed class TestYaapClient(IConfiguration appconfig, YaapClientDetail clientDetail) : BaseYaapClient(appconfig, clientDetail, null)
    {
        public int HelloCalledCount { get; private set; }
        public override Task SayHelloAsync(CancellationToken cancellationToken)
        {
            HelloCalledCount++;
            return Task.CompletedTask;
        }

        public int GoodbyeCalledCount { get; private set; }
        public override Task SayGoodbyeAsync(CancellationToken cancellationToken)
        {
            GoodbyeCalledCount++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(new YaapClientDetail("TestClient", "TestDescription", new("http://localhost/callback")));
        this.Mocker.Use(configurationMock);

        // Act
        var client = this.Mocker.CreateInstance<TestYaapClient>();

        // Assert
        var clientDetail = client.Detail;
        Assert.Equal("TestClient", clientDetail.Name);
        Assert.Equal("TestDescription", clientDetail.Description);
        Assert.Equal(new Uri("http://localhost/callback"), clientDetail.CallbackUrl);
        Assert.Equal(new Uri("http://localhost/server"), client.YaapServerEndpoint);
    }

    [Fact]
    public void Detail_ShouldThrowException_WhenConfigurationHasNoName()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns((string?)null);
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(new YaapClientDetail(null, "TestDescription", new("http://localhost/callback")));
        this.Mocker.Use(configurationMock);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.Mocker.CreateInstance<TestYaapClient>());
    }

    [Fact]
    public void Detail_ShouldThrowException_WhenConfigurationHasNoDescription()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns((string?)null);
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(new YaapClientDetail("TestClient", null, new("http://localhost/callback")));
        this.Mocker.Use(configurationMock);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.Mocker.CreateInstance<TestYaapClient>());
    }

    [Fact]
    public void Detail_ShouldBeOk_WhenConfigurationHasNoCallback()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns((string?)null);
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(new YaapClientDetail("TestClient", "TestDescription", null));
        this.Mocker.Use(configurationMock);

        // Act
        var client = this.Mocker.CreateInstance<TestYaapClient>();

        // Assert
        var clientDetail = client.Detail;
        Assert.Equal("TestClient", clientDetail.Name);
        Assert.Equal("TestDescription", clientDetail.Description);
        Assert.Null(clientDetail.CallbackUrl);
        Assert.Equal(new Uri("http://localhost/server"), client.YaapServerEndpoint);
    }

    [Fact]
    public void Detail_ShouldThrowException_WhenConfigurationHasNoServerEndpoint()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns((string?)null);

        this.Mocker.Use(new YaapClientDetail("TestClient", "TestDescription", new("http://localhost/callback")));
        this.Mocker.Use(configurationMock);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.Mocker.CreateInstance<TestYaapClient>());
    }

    [Fact]
    public async Task HostedApplication_ShouldCall_HelloAndGoodbye_WhenDisposed()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(new YaapClientDetail("TestClient", "TestDescription", new("http://localhost/callback")));

        var clientMock = new Mock<BaseYaapClient>(configurationMock.Object, this.Mocker.GetRequiredService<YaapClientDetail>(), (ILoggerFactory?)null);
        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Services.AddHostedService(_ => clientMock.Object);

        // Act
        using (var host = builder.Build())
        {
            host.RunAsync();
            await Task.Delay(TimeSpan.FromSeconds(3), this.TimeMock.Object); // Simulate some delay to allow the hosted service to start
        }

        // Assert
        clientMock.Verify(c => c.SayHelloAsync(It.IsAny<CancellationToken>()), Times.Once);
        clientMock.Verify(c => c.SayGoodbyeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HostedApplication_ShouldCall_HelloAndGoodbye_WhenShutdownGracefully()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(new YaapClientDetail("TestClient", "TestDescription", new("http://localhost/callback")));

        var clientMock = new Mock<BaseYaapClient>(configurationMock.Object, this.Mocker.GetRequiredService<YaapClientDetail>(), (ILoggerFactory?)null);
        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Services.AddHostedService(_ => clientMock.Object);

        // Act
        var host = builder.Build();
        host.RunAsync();
        await Task.Delay(TimeSpan.FromSeconds(3), this.TimeMock.Object); // Simulate some delay to allow the hosted service to start
        await host.StopAsync();

        // Assert
        clientMock.Verify(c => c.SayHelloAsync(It.IsAny<CancellationToken>()), Times.Once);
        clientMock.Verify(c => c.SayGoodbyeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
