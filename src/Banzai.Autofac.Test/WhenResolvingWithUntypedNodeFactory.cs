﻿using System.Linq;
using Autofac;
using Autofac.Core.Registration;
using Banzai.Factories;
using NUnit.Framework;
using Should;

namespace Banzai.Autofac.Test
{
    [TestFixture]
    public class WhenResolvingWithUntypedNodeFactory
    {
        [Test]
        public void Node_Is_Retrieved_As_Self()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory>();
            var node = nodeFactory.GetNode<TestNode>();

            node.ShouldNotBeNull();
        }

        [Test]
        public void Node_Is_Retrieved_As_Primary_Interface()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory>();
            var node = nodeFactory.GetNode<ITestNode<object>>();

            node.ShouldNotBeNull();
        }

        [Test]
        public void All_INodes_Are_Retrieved()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory>();
            var nodes = nodeFactory.GetAllNodes<INode<object>>().ToList();

            nodes.Count.ShouldBeGreaterThan(1);
        }

        [Test]
        public void Resolution_Of_Named_Node_Succeeds()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            containerBuilder.RegisterType<TestNode>().Named<ITestNode<object>>("TestName");

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory>();
            var node = nodeFactory.GetNode<ITestNode<object>>("TestName");

            node.ShouldNotBeNull();

            Assert.Throws<ComponentNotRegisteredException>(() => nodeFactory.GetNode<ITestNode<object>>("TestName2"));
        }

        [Test]
        public void Resolution_Of_Transition_Node_Succeeds()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var container = containerBuilder.Build();
            var nodeFactory = container.Resolve<INodeFactory>();
            var node = nodeFactory.GetNode<ITestTransitionNode1>();

            node.ShouldNotBeNull();
        }


    }

}