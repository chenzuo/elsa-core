using Xunit;
using System.Threading.Tasks;
using Elsa.Services;
using Elsa.Activities.Signaling.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace UserTaskTests
{
    public class UnitTest3
    {
        private readonly ITestOutputHelper _output;
        public UnitTest3(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public async Task Test1()
        {
            // Create a service container with Elsa services.
            var services = new ServiceCollection()
                .AddElsa(options => options
                    .AddConsoleActivities()
                    .AddWorkflow<DocumentApprovalWorkflow>())
                .BuildServiceProvider();

            // Get a workflow runner.
            var workflowRunner = services.GetRequiredService<IBuildsAndStartsWorkflow>();

            // Run the workflow.
            await workflowRunner.BuildAndStartWorkflowAsync<DocumentApprovalWorkflow>();
        }
    }
}