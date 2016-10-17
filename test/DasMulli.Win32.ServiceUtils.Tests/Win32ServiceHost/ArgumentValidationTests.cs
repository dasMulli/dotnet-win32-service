using System;
using System.Diagnostics.CodeAnalysis;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace DasMulli.Win32.ServiceUtils.Tests.Win32ServiceHost
{
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification = "Testing")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing ctors")]
    public class ArgumentValidationTests
    {
        [Fact]
        public void ItShallThrowOnNullService()
        {
            Action ctor = () => new ServiceUtils.Win32ServiceHost(service: null);

            ctor.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("service");
        }

        [Fact]
        public void ItShallThrowOnNullServiceName()
        {
            Action ctor = () => new ServiceUtils.Win32ServiceHost(serviceName: null, stateMachine: A.Fake<IWin32ServiceStateMachine>());

            ctor.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("serviceName");
        }

        [Fact]
        public void ItShallThrowOnNullServiceStateMachine()
        {
            Action ctor = () => new ServiceUtils.Win32ServiceHost("Test Service", stateMachine: null);

            ctor.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("stateMachine");
        }
    }
}
