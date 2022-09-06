using Elsa.Activities.UserTask.Extensions;
using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services.WorkflowStorage;
using Elsa.Activities.Signaling.Services;
using Elsa.Activities.Signaling.Models;

namespace UserTaskTests
{
    public class UnitTest2
    {
        private readonly ITestOutputHelper _output;
        public UnitTest2(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Test2()
        {
            // Create a service container with Elsa services.
            var services = new ServiceCollection()
                .AddElsa(options => options
                    .AddConsoleActivities()
                    .AddUserTaskActivities()
                    .AddWorkflow<UserTaskWorkflow>())
                .BuildServiceProvider();

            // Run startup actions (not needed when registering Elsa with a Host).
            var startupRunner = services.GetRequiredService<IStartupRunner>();
            await startupRunner.StartupAsync();

            // Get a workflow runner.
            var workflowRunner = services.GetRequiredService<IBuildsAndStartsWorkflow>();

            // Get an interruptor.
            var workflowTriggerInterruptor = services.GetRequiredService<IWorkflowTriggerInterruptor>();

            // Execute the workflow.
            var runWorkflowResult = await workflowRunner.BuildAndStartWorkflowAsync<UserTaskWorkflow>();
            var workflowInstance = runWorkflowResult.WorkflowInstance!;

            var signalerService = services.GetRequiredService<ISignaler>();
            var tokenService = services.GetRequiredService<ITokenService>();
            Assert.NotNull(signalerService);

            var WorkflowInstanceId = workflowInstance.Id;

            // signalerService.TriggerSignalTokenAsync();
            var payload = new SignalModel("Accept", WorkflowInstanceId);
            var signalsToken = tokenService.CreateToken(payload);

            Assert.NotNull(signalsToken);

            // workflow instance id
            _output.WriteLine($"1.{workflowInstance.LastExecutedActivityId} 2.{workflowInstance.ContextId} 3.{workflowInstance.DefinitionId} 4.{workflowInstance.DefinitionVersionId} 5.{System.Text.Json.JsonSerializer.Serialize(workflowInstance.BlockingActivities.Select(x => x.ActivityId).ToList())}");

            var availableActions = new[]
            {
                "Accept",
                "Reject",
                "Needs Work"
            };

            var userAction = "Reject";

            var currentActivityId = workflowInstance.BlockingActivities.Select(i => i.ActivityId).First();

            // Update the workflow instance with input.
            var workflowStorageService = services.GetRequiredService<IWorkflowStorageService>();
            await workflowStorageService.UpdateInputAsync(workflowInstance, new WorkflowInput(userAction));
            await workflowTriggerInterruptor.InterruptActivityAsync(workflowInstance, currentActivityId);

            // await Task.CompletedTask;
        }
    }
}