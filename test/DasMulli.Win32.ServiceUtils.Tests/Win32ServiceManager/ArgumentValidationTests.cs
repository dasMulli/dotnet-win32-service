using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace DasMulli.Win32.ServiceUtils.Tests.Win32ServiceManager
{
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification = "Testing")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Testing ctors")]
    [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
    public class ArgumentValidationTests
    {
        private const string TestMachineName = "TestMachine";
        private const string TestDatabasename = "TestDatabase";
        private const string TestServiceName = "TestService";
        private const string TestDisplayName = "TestDisplayname";
        private const string TestDescription = "Test Description";
        private const string TestBinaryPath = "Test.exe";
        
        private static ServiceDefinitionBuilder CreateTestServiceDefinitionBuilder()
            => new ServiceDefinitionBuilder(TestServiceName)
                .WithDisplayName(TestDisplayName)
                .WithDescription(TestDescription)
                .WithBinaryPath(TestBinaryPath)
                .WithCredentials(Win32ServiceCredentials.LocalService);

        private readonly ServiceUtils.Win32ServiceManager sut = new ServiceUtils.Win32ServiceManager(TestMachineName, TestDatabasename);

        [Fact]
        public void ItShallThrowOnCreateServiceWithNullServiceName()
        {
            Action invocation = () => sut.CreateService(CreateTestServiceDefinitionBuilder().WithServiceName(null).BuildNonValidating());

            invocation.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("serviceDefinition");
        }

        [Fact]
        public void ItShallThrowOnCreateServiceWithNullBinaryPath()
        {
            Action invocation = () => sut.CreateService(CreateTestServiceDefinitionBuilder().WithBinaryPath(null).BuildNonValidating());

            invocation.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("serviceDefinition");
        }

        [Fact]
        public void ItShallThrowOnCreateOrUpdateServiceWithNullServiceName()
        {
            Action invocation = () => sut.CreateOrUpdateService(CreateTestServiceDefinitionBuilder().WithServiceName(null).BuildNonValidating());

            invocation.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("serviceDefinition");
        }

        [Fact]
        public void ItShallThrowOnCreateOrUpdateServiceWithNullBinaryPath()
        {
            Action invocation = () => sut.CreateOrUpdateService(CreateTestServiceDefinitionBuilder().WithBinaryPath(null).BuildNonValidating());

            invocation.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("serviceDefinition");
        }

        [Fact]
        public void ItShallThrowOnDeleteServiceWithEmptyServiceName()
        {
            Action invocation = () => sut.DeleteService(serviceName: string.Empty);

            invocation.ShouldThrow<ArgumentException>().Which.ParamName.Should().Be("serviceName");
        }

        [Fact]
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        private void ItShallNotThrowOnNullParameters()
        {
            Action when = () => new ServiceUtils.Win32ServiceManager(machineName: null, databaseName: null);

            when.ShouldNotThrow();
        }
    }
}
