namespace Yaap.Client.Tests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using System;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

using Yaap.Client;
using Yaap.Models;
using Yaap.TestCommon;

public class YaapClientTests(ITestOutputHelper outputHelper) : YaapTest(outputHelper)
{
    private class TestYaapClient : YaapClient
    {
        public TestYaapClient(IServiceProvider services) : base(services) { }

        protected override Task SayHelloAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected override Task SayGoodbyeAsync(CancellationToken cancellationToken) => Task.CompletedTask;
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

        this.Mocker.Use(configurationMock);

        // Act
        var client = new TestYaapClient(this.Mocker);

        // Assert
        Assert.NotNull(client.GetPrivatePropertyValue<IServiceProvider>("Services"));
        var clientDetail = client.GetPrivatePropertyValue<YaapClientDetail>("Detail");
        Assert.Equal("TestClient", clientDetail.Name);
        Assert.Equal("TestDescription", clientDetail.Description);
        Assert.Equal(new Uri("http://localhost/callback"), clientDetail.CallbackUrl);
        Assert.Equal(new Uri("http://localhost/server"), client.GetPrivatePropertyValue<Uri>("YaapServerEndpoint"));
    }

    [Fact]
    public void Detail_ShouldThrowException_WhenConfigurationHasNoName()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns((string)null);
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(configurationMock);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestYaapClient(this.Mocker));
    }

    [Fact]
    public void Detail_ShouldThrowException_WhenConfigurationHasNoDescription()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns((string)null);
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(configurationMock);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestYaapClient(this.Mocker));
    }

    [Fact]
    public void Detail_ShouldBeOk_WhenConfigurationHasNoCallback()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns((string)null);
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns("http://localhost/server");

        this.Mocker.Use(configurationMock);

        // Act
        var client = new TestYaapClient(this.Mocker);

        // Assert
        Assert.NotNull(client.GetPrivatePropertyValue<IServiceProvider>("Services"));
        var clientDetail = client.GetPrivatePropertyValue<YaapClientDetail>("Detail");
        Assert.Equal("TestClient", clientDetail.Name);
        Assert.Equal("TestDescription", clientDetail.Description);
        Assert.Null(clientDetail.CallbackUrl);
        Assert.Equal(new Uri("http://localhost/server"), client.GetPrivatePropertyValue<Uri>("YaapServerEndpoint"));
    }

    [Fact]
    public void Detail_ShouldThrowException_WhenConfigurationHasNoServerEndpoint()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c["Yaap:Client:Name"]).Returns("TestClient");
        configurationMock.Setup(c => c["Yaap:Client:Description"]).Returns("TestDescription");
        configurationMock.Setup(c => c["Yaap:Client:CallbackUrl"]).Returns("http://localhost/callback");
        configurationMock.Setup(c => c["Yaap:Server:Endpoint"]).Returns((string)null);

        this.Mocker.Use(configurationMock);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestYaapClient(this.Mocker));
    }
}
