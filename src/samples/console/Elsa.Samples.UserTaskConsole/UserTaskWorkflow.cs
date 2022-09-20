using Elsa.Activities.Console;
using Elsa.Builders;
using Elsa.Activities.UserTask.Activities;
using Elsa.Activities.Primitives;
using Elsa.Activities.ControlFlow;

namespace Elsa.Samples.UserTaskConsole
{
    public class UserTaskWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder workflow)
        {
            workflow
                .WriteLine("Workflow started. Waiting for user action.")
                .WithName("Start")
                .Then<UserTask>(
                    activity => activity.Set(x => x.Actions, new[] { "Accept", "Reject", "Needs Work" }),
                    userTask =>
                    {
                        // userTask.WriteLine(context => $"Jack approve url: \n {context.GetVariable<string>("MyVariable")}");
                        userTask
                            .When("Accept")
                            .WriteLine(context => $"Great! Your work has been accepted.")
                            .ThenNamed("Exit");

                        userTask
                            .When("Reject")
                            // .SignalReceived("Reject")
                            .WriteLine(context => $"Sorry! Your work has been rejected.")
                            .ThenNamed("Exit");

                        userTask
                            .When("Needs Work")
                            // .SignalReceived("Needs Work")
                            .WriteLine(context => $"So close! Your work needs a little bit more work.")
                            .ThenNamed("Exit");
                    }
                )
                .WriteLine("Workflow finished.").WithName("Exit");
        }
    }
}