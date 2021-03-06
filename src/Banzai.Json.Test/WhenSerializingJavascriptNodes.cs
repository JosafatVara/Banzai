﻿using System;
using Autofac;
using Banzai.Autofac;
using Banzai.Factories;
using Banzai.JavaScript.Factories;
using NUnit.Framework;
using Should;

namespace Banzai.Json.Test
{
    [TestFixture, Explicit]
    public class WhenSerializingJavascriptNodes
    {
        [Test]
        public void Simple_Flow_Is_Serialized()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestJsNode>().SetExecutedJavaScript("var x = 1;")
                .AddChild<ITestNode2>();

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            definition.ShouldNotBeNull().ShouldNotBeEmpty();
           
        }

        [Test]
        public void Simple_Flow_With_ExecutedJavaScript_Is_Deserialized()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestJsNode>()
                .AddChild<ITestNode2>()
                    .ForChild<ITestJsNode>()
                    .SetExecutedJavaScript("var x = 1;");

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

            deserializedComponent.ShouldNotBeNull();

            var pipelineComponent = deserializedComponent.Children[0];
            pipelineComponent.Children.Count.ShouldEqual(2);

            var jsNode = pipelineComponent.Children[0];
            jsNode.GetExecutedJavaScript().ShouldEqual("var x = 1;");
        }

        [Test]
        public void Simple_Flow_With_ShouldExecuteJavaScript_Is_Deserialized()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);

            var flowBuilder = new FlowBuilder<object>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<object>>()
                .AddChild<ITestJsNode>()
                .AddChild<ITestNode2>()
                    .ForChild<ITestJsNode>()
                    .SetShouldExecuteJavaScript("var x = 1;");

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            FlowComponent<object> deserializedComponent = serializer.Deserialize<object>(definition);

            deserializedComponent.ShouldNotBeNull();

            var pipelineComponent = deserializedComponent.Children[0];
            pipelineComponent.Children.Count.ShouldEqual(2);

            var jsNode = pipelineComponent.Children[0];
            jsNode.GetShouldExecuteJavaScript().ShouldEqual("var x = 1;");
        }

        [Test]
        public async void Deserialized_Flow_Component_Can_Be_Built_And_Run()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterBanzaiNodes(GetType().Assembly, true);
            containerBuilder.RegisterBanzaiNodes(typeof(JavaScriptMetaDataBuilder).Assembly, true);

            var flowBuilder = new FlowBuilder<TestObjectA>(new AutofacFlowRegistrar(containerBuilder));

            flowBuilder.CreateFlow("TestFlow1")
                .AddRoot<IPipelineNode<TestObjectA>>()
                .AddChild<ITestJsNode2>()
                .AddChild<ITestNode2>()
                    .ForChild<ITestJsNode2>()
                    .SetExecutedJavaScript("context.Subject.TestValueString = 'Hello JavaScript';");

            var rootComponent = flowBuilder.RootComponent;

            var serializer = new JsonComponentSerializer();

            var definition = serializer.Serialize(rootComponent);

            Console.WriteLine(definition);

            FlowComponent<TestObjectA> deserializedComponent = serializer.Deserialize<TestObjectA>(definition);

            deserializedComponent.ShouldNotBeNull();

            flowBuilder.RootComponent = deserializedComponent;

            flowBuilder.Register();

            var container = containerBuilder.Build();

            var factory = container.Resolve<INodeFactory<TestObjectA>>();

            var flowRootNode = factory.BuildFlow("TestFlow1");

            NodeResult result = await flowRootNode.ExecuteAsync(new TestObjectA());

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);

            var subject = result.GetSubjectAs<TestObjectA>();

            subject.TestValueString.ShouldEqual("Hello JavaScript");

        }

    }
    
}