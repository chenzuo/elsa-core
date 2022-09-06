using System;
using Xunit;
using System.Threading.Tasks;
using Elsa.Services;
using Elsa.Activities.Signaling.Services;
using Microsoft.Extensions.DependencyInjection;



namespace UserTaskTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            // Create a service container with Elsa services.
            var services = new ServiceCollection()
                .AddElsa(options => options
                    .AddConsoleActivities())
                .BuildServiceProvider();

            // Run startup actions (not needed when registering Elsa with a Host).
            var startupRunner = services.GetRequiredService<IStartupRunner>();
            await startupRunner.StartupAsync();

            // Get a workflow runner.
            var workflowRunner = services.GetRequiredService<IBuildsAndStartsWorkflow>();

            // get ISignaler service
            var signaler = services.GetRequiredService<ISignaler>();
            Assert.NotNull(signaler);
        }
    }
}
