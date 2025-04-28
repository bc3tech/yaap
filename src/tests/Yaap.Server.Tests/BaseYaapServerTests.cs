namespace Yaap.Server.Tests;

using System.Threading.Channels;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xunit.Abstractions;

using Yaap.Core.Models;
using Yaap.Server.Abstractions;
using Yaap.TestCommon;

public class BaseYaapServerTests(ITestOutputHelper outputHelper) : YaapTest(outputHelper)
{
    private sealed class TestYaapServer(IDistributedCache cache) : BaseYaapServer(cache, null)
    {
        public int HelloCalledCount { get; private set; }
        protected override Task<string> HandleHelloCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
        {
            HelloCalledCount++;
            return Task.FromResult(string.Empty);
        }

        public int GoodbyeCalledCount { get; private set; }
        protected override Task HandleGoodbyeCustomAsync(YaapClientDetail clientDetail, CancellationToken cancellationToken)
        {
            GoodbyeCalledCount++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task StartAsyncShouldInvokeHandlers()
    {
        // Arrange  
        HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Services
            .AddDistributedMemoryCache()
            .AddKeyedSingleton("HelloChannel", Channel.CreateUnbounded<YaapClientDetail>())
            .AddKeyedSingleton("GoodbyeChannel", Channel.CreateUnbounded<YaapClientDetail>())
            .AddHostedService<TestYaapServer>();

        // Act  
        using IHost host = builder.Build();
        host.RunAsync();
        await Task.Delay(TimeSpan.FromSeconds(1), this.TimeMock.Object); // Simulate some delay to allow the hosted service to start

        // Assert  
        // No exceptions should be thrown, and handlers should be invoked (tested indirectly here).  
    }

    [Fact]
    public async Task HandleHelloAsyncShouldAddClientToCache()
    {
        // Arrange  
        var helloChannel = Channel.CreateUnbounded<YaapClientDetail>();
        HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Services
            .AddDistributedMemoryCache()
            .AddKeyedSingleton("HelloChannel", helloChannel)
            .AddKeyedSingleton("GoodbyeChannel", Channel.CreateUnbounded<YaapClientDetail>())
            .AddHostedService<TestYaapServer>();

        // Act & Assert
        using IHost host = builder.Build();
        var server = host.Services.GetRequiredService<IHostedService>() as TestYaapServer;
        Assert.NotNull(server);

        IDistributedCache cache = host.Services.GetRequiredService<IDistributedCache>();
        Assert.NotNull(cache);

        host.RunAsync();

        await server.HandleHelloAsync(new YaapClientDetail("TestClient", "TestDescription", null), CancellationToken.None);

        Assert.Equal(1, server.HelloCalledCount);
        Assert.NotNull(cache.GetString("TestClient")); // Check if the client was added to the cache
    }

    [Fact]
    public async Task HandleHelloAsyncShouldThrowIfClientAlreadyExists()
    {
        // Arrange  
        var helloChannel = Channel.CreateUnbounded<YaapClientDetail>();
        HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Services
            .AddDistributedMemoryCache()
            .AddKeyedSingleton("HelloChannel", helloChannel)
            .AddKeyedSingleton("GoodbyeChannel", Channel.CreateUnbounded<YaapClientDetail>())
            .AddHostedService<TestYaapServer>();
        using IHost host = builder.Build();

        // Act & Assert
        var server = host.Services.GetRequiredService<IHostedService>() as TestYaapServer;
        Assert.NotNull(server);

        IDistributedCache cache = host.Services.GetRequiredService<IDistributedCache>();
        Assert.NotNull(cache);

        host.RunAsync();

        await server.HandleHelloAsync(new YaapClientDetail("TestClient", "TestDescription", null), CancellationToken.None);
        await Assert.ThrowsAsync<ArgumentException>(async () => await server.HandleHelloAsync(new YaapClientDetail("TestClient", "TestDescription", null), CancellationToken.None));

        // Act & Assert  
        Assert.Equal(1, server.HelloCalledCount);
        Assert.NotNull(cache.GetString("TestClient")); // Check if the client was added to the cache
    }

    [Fact]
    public async Task HandleGoodbyeAsyncShouldRemoveClientFromCache()
    {
        // Arrange  
        HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Services
            .AddDistributedMemoryCache()
            .AddSingleton<TestYaapServer>()
            .AddHostedService(sp => sp.GetRequiredService<TestYaapServer>());
        using IHost host = builder.Build();

        // Act & Assert
        var server = host.Services.GetRequiredService<IHostedService>() as TestYaapServer;
        Assert.NotNull(server);

        IDistributedCache cache = host.Services.GetRequiredService<IDistributedCache>();
        Assert.NotNull(cache);

        host.RunAsync();

        await server.HandleHelloAsync(new YaapClientDetail("TestClient", "TestDescription", null), CancellationToken.None);

        Assert.NotNull(cache.GetString("TestClient")); // Check if the client was added to the cache

        await server.HandleGoodbyeAsync(new YaapClientDetail("TestClient", "TestDescription", null), CancellationToken.None);

        Assert.Equal(1, server.HelloCalledCount);
        Assert.Equal(1, server.GoodbyeCalledCount);
        Assert.Null(cache.GetString("TestClient"));
    }
}
