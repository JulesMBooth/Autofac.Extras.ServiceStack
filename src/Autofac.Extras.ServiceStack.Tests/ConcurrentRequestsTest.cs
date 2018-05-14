using Autofac.Extras.ServiceStack.Tests.Utils;
using ServiceStack;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Autofac.Extras.ServiceStack.Tests
{
    public class ConcurrentRequestsTest : IDisposable
    {
        private ServiceStackHost _appHost;
        public const int ServicePort = 62184;

        public ConcurrentRequestsTest()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<ValueHolder>().AsSelf().InstancePerRequest();

            var container = containerBuilder.Build();

            _appHost = new AppHost()
                .UseAutofac(container)
                .Init()
                .Start($"http://*:{ServicePort}/");
        }

        [Fact]
        public async Task With_Concurrent_Requests_Each_Should_Have_Separate_Lifetime_Scope()
        {
            var tasks = Enumerable.Range(0, 99)
                .Select(i => new JsonHttpClient($"http://localhost:{ServicePort}"))
                .Select((c, i) => c.GetAsync(new GetValue { Value = i }))
                .ToArray();

            await Task.WhenAll(tasks);

            var responses = tasks.Select(t => t.Result);

            var expectedResponses = Enumerable.Range(0, 99)
                .Select(i => new GetValueResponse { Value = i })
                .ToList();

            responses.Should().BeEquivalentTo(expectedResponses);
        }

        public void Dispose()
        {
            _appHost?.Dispose();
        }
    }
}
