﻿using System.Threading.Tasks;
using NUnit.Framework;
using Should;

namespace Banzai.Test
{
    [TestFixture]
    public class WhenRunningTransitionNode
    {
        [Test]
        public async void Successful_Transition_Run_Status_Is_Succeeded()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTransitionNode{ChildNode = new SimpleTestNodeB1()});
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Failed_Transition_Node_Child_Results_In_Failed_Parent()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTransitionNode { ChildNode = new FailingTestNodeB() });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
        }

        [Test]
        public async void Faulted_Transition_Node_Child_Results_In_Failed_Parent_With_Exception()
        {
            var pipelineNode = new PipelineNode<TestObjectA>();

            pipelineNode.AddChild(new SimpleTestNodeA1());
            pipelineNode.AddChild(new SimpleTransitionNode { ChildNode = new FaultingTestNodeB() });
            pipelineNode.AddChild(new SimpleTestNodeA2());

            var testObject = new TestObjectA();
            NodeResult result = await pipelineNode.ExecuteAsync(testObject);

            result.Status.ShouldEqual(NodeResultStatus.Failed);
            pipelineNode.Status.ShouldEqual(NodeRunStatus.Completed);
            result.Exception.ShouldNotBeNull();
        }

        public class SimpleTransitionNode : TransitionNode<TestObjectA, TestObjectB>
        {
            protected override Task<TestObjectB> TransitionSourceAsync(IExecutionContext<TestObjectA> sourceContext)
            {
                return Task.FromResult(new TestObjectB());
            }

            protected override Task<TestObjectA> TransitionResultAsync(IExecutionContext<TestObjectA> sourceContext, NodeResult result)
            {
                return Task.FromResult(sourceContext.Subject);
            }
        }

    }
}