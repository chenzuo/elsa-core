using Elsa.Activities.UserTask.Extensions;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Services.WorkflowStorage;
using Elsa.Activities.Signaling.Services;
using Microsoft.Extensions.Logging;

namespace Elsa.Samples.UserTaskConsole
{
    class Program
    {
        private static async Task Main()
        {
            // Create a service container with Elsa services.
            var services = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace))
                .AddElsa(options => options
                // .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
                    .AddConsoleActivities()
                    .AddUserTaskActivities()
                    .AddWorkflow<UserTaskWorkflow>())
                .BuildServiceProvider();

            // Run startup actions (not needed when registering Elsa with a Host).
            var startupRunner = services.GetRequiredService<IStartupRunner>();
            await startupRunner.StartupAsync();

            // Get a workflow runner.
            var workflowRunner = services.GetRequiredService<IBuildsAndStartsWorkflow>();

            var availableActions = new[]
            {
                "Accept",
                "Reject",
                "Needs Work"
            };

            // Workflow is now halted on the user task activity. Ask user for input:
            Console.WriteLine($"What action will you take? Choose one of: {string.Join(", ", availableActions)}");
            var userAction = Console.ReadLine();

            // var signaler = services.GetRequiredService<ISignaler>();
            // await signaler.TriggerSignalAsync(userAction);


            // // Update the workflow instance with input.
            var workflowStorageService = services.GetRequiredService<IWorkflowStorageService>();

            // Execute the workflow.
            var runWorkflowResult = await workflowRunner.BuildAndStartWorkflowAsync<UserTaskWorkflow>();
            var workflowInstance = runWorkflowResult.WorkflowInstance!;

            // //执行命令动作
            await workflowStorageService.UpdateInputAsync(workflowInstance, new WorkflowInput(userAction));

            var currentActivityId = workflowInstance.BlockingActivities.Select(i => i.ActivityId).First();

            // Get an interruptor.
            var workflowTriggerInterruptor = services.GetRequiredService<IWorkflowTriggerInterruptor>();
            await workflowTriggerInterruptor.InterruptActivityAsync(workflowInstance, currentActivityId);
        }
    }
}