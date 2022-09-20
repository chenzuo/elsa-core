using Elsa.Activities.Console;
using Elsa.Activities.ControlFlow;
using Elsa.Activities.Primitives;
using Elsa.Builders;


namespace UserTaskTests
{
    /// <summary>
    /// A workflow that gives Jack and Lucy a chance to review a submitted document.
    /// </summary>
    public class DocumentApprovalWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {

          var users = new[] { "john", "tom", "jenny" };

            builder
                .WriteLine("This demonstrates a simple workflow with switch.")
                .WriteLine("Using switch we can branch a workflow.")
                .Then<Fork>(fork => fork.WithBranches(users), fork =>
                {
                    foreach (var user in users)
                        fork.When(user)
                        .SignalReceived(user)
                        .WriteLine(context=>$"{context.GetVariable<string>("MyVariable")}")
                            .WriteLine("A fork created for " + user)
                            .ThenNamed("Join1");
                })
                .Then<Join>(join => join.WithMode(Join.JoinMode.WaitAll)).WithName("Join1")
                .WriteLine("Workflow finished.");

            // builder
            //     .WriteLine(context => $"Document received from {context.GetVariable<dynamic>("Document")!.Author.Name}")
            //     .Then<Fork>(fork1 => fork1.WithBranches("Jack", "Lucy"), fork1 =>
            //         {
            //             fork1
            //                 .When("Jack")
            //                 .WriteLine(context => $"Jack approve url: \n {context.GenerateSignalUrl("Approve:Jack")}")
            //                 .Then<Fork>(fork2 => fork2.WithBranches("Approve", "Reject"), fork2 =>
            //                 {
            //                     fork2
            //                         .When("Approve")
            //                         .SignalReceived("Approve:Jack")
            //                         .WriteLine("Jack Approved")
            //                         .SetVariable("Approved", context => context.SetVariable<int>("Approved", approved => approved == 0 ? 1 : approved))
            //                         .ThenNamed("JoinJack");

            //                     fork2
            //                         .When("Reject")
            //                         .SignalReceived("Reject:Jack")
            //                         .WriteLine("Jack Rejected")
            //                         .SetVariable<int>("Approved", 2)
            //                         .ThenNamed("JoinJack");
            //                 }).WithName("ForkJack")
            //                 .Add<Join>(x => x.WithMode(Join.JoinMode.WaitAny)).WithName("JoinJack")
            //                 .ThenNamed("JoinJackLucy");

            //             fork1.When("Lucy")
            //                 .WriteLine(context => $"Lucy approve url: \n {context.GenerateSignalUrl("Approve:Lucy")}")
            //                 .Then<Fork>(fork2 => fork2.WithBranches("Approve", "Reject"),
            //                     fork2 =>
            //                     {
            //                         fork2
            //                             .When("Approve")
            //                             .SignalReceived("Approve:Lucy")
            //                             .WriteLine("Lucy Approve")
            //                             .SetVariable("Approved", context => context.SetVariable<int>("Approved", approved => approved == 0 ? 1 : approved))
            //                             .ThenNamed("JoinLucy");

            //                         fork2
            //                             .When("Reject")
            //                             .SignalReceived("Reject:Lucy")
            //                             .WriteLine("Lucy Reject")
            //                             .SetVariable<int>("Approved", 2)
            //                             .ThenNamed("JoinLucy");
            //                     }).WithName("ForkLucy")
            //                 .Add<Join>(x => x.WithMode(Join.JoinMode.WaitAny)).WithName("JoinLucy")
            //                 .ThenNamed("JoinJackLucy");
            //         }
            //     )
            //     .Add<Join>(x => x.WithMode(Join.JoinMode.WaitAll)).WithName("JoinJackLucy")// wait all Approved or Rejected then to branch JoinJackLucy
            //     .WriteLine(context => $"Approved: {context.GetVariable<int>("Approved")}").WithName("AfterJoinJackLucy")
            //     .If(context => context.GetVariable<int>("Approved") == 1, @if =>
            //     {
            //         @if
            //             .When(OutcomeNames.True)
            //             .WriteLine(context => $"Document ${context.GetVariable<dynamic>("Document")!.Id} approved!`");

            //         @if
            //             .When(OutcomeNames.False)
            //             .WriteLine(context => $"Document ${context.GetVariable<dynamic>("Document")!.Id} rejected!`");
            //     });
        }
    }
}