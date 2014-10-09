﻿using System;
using System.Threading.Tasks;

namespace Banzai.Test
{
    public class SimpleTestNodeA1 : Node<TestObjectA>
    {
        private readonly bool _shouldExecute = true;

        public SimpleTestNodeA1()
        {
        }

        public SimpleTestNodeA1(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueString = "Completed";

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public class SimpleTestNodeA2 : Node<TestObjectA>
    {
        private readonly bool _shouldExecute = true;

        public SimpleTestNodeA2()
        {
        }
        
        public SimpleTestNodeA2(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueInt = 100;

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public class SubjectChangingNode1 : Node<TestObjectA>
    {
        private readonly bool _shouldExecute = true;

        public SubjectChangingNode1()
        {
        }

        public SubjectChangingNode1(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.ChangeSubject(new TestObjectA()
            {
                TestValueString = "New Instance"
            });

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public class FaultingTestNodeA : Node<TestObjectA>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueString = "Faulted";

            throw new Exception("Test Exception");
        }
    }

    public class FailingTestNodeA : Node<TestObjectA>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectA> context)
        {
            context.Subject.TestValueString = "Failed";

            return Task.FromResult(NodeResultStatus.Failed);
        }
    }

    public class SimpleTestNodeB1 : Node<TestObjectB>
    {
        private readonly bool _shouldExecute = true;

        public SimpleTestNodeB1()
        {
        }

        public SimpleTestNodeB1(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectB> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectB> context)
        {
            context.Subject.TestValueBool = true;

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public class SimpleTestNodeB2 : Node<TestObjectB>
    {
        private readonly bool _shouldExecute = true;

        public SimpleTestNodeB2()
        {
        }

        public SimpleTestNodeB2(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;
        }

        public override Task<bool> ShouldExecuteAsync(ExecutionContext<TestObjectB> context)
        {
            return Task.FromResult(_shouldExecute);
        }

        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectB> context)
        {
            context.Subject.TestValueBool = false;

            return Task.FromResult(NodeResultStatus.Succeeded);
        }
    }

    public class FaultingTestNodeB : Node<TestObjectB>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectB> context)
        {
            context.Subject.TestValueBool = false;

            throw new Exception("Test Exception");
        }
    }

    public class FailingTestNodeB : Node<TestObjectB>
    {
        protected override Task<NodeResultStatus> PerformExecuteAsync(ExecutionContext<TestObjectB> context)
        {
            context.Subject.TestValueBool = false;

            return Task.FromResult(NodeResultStatus.Failed);
        }
    }

    public class TestObjectA
    {
        public string TestValueString { get; set; }

        public int TestValueInt { get; set; }
    }

    public class TestObjectB
    {
        public bool TestValueBool { get; set; }

    }
}