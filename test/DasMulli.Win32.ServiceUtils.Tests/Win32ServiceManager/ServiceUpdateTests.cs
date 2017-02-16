using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace DasMulli.Win32.ServiceUtils.Tests.Win32ServiceManager
{
    public class ServiceUpdateTests
    {
        private const string TestMachineName = "TestMachine";
        private const string TestDatabaseName = "TestDatabase";
        private const string TestServiceName = "UnitTestService";
        private const string TestServiceDisplayName = "A Test Service";
        private const string TestServiceDescription = "This describes the Test Service";
        private const string TestServiceBinaryPath = @"C:\Some\Where\service.exe --run-as-service";
        private const ErrorSeverity TestServiceErrorSeverity = ErrorSeverity.Ignore;
        private static readonly Win32ServiceCredentials TestCredentials = new Win32ServiceCredentials(@"ADomain\AUser", "WithAPassword");

        private readonly INativeInterop nativeInterop = A.Fake<INativeInterop>();
        private readonly ServiceControlManager serviceControlManager;

        private readonly ServiceUtils.Win32ServiceManager sut;

        public ServiceUpdateTests()
        {
            serviceControlManager = A.Fake<ServiceControlManager>(o => o.Wrapping(new ServiceControlManager { NativeInterop = nativeInterop }));

            sut = new ServiceUtils.Win32ServiceManager(TestMachineName, TestDatabaseName, nativeInterop);
        }

        [Theory]
        [InlineData(true, ServiceStartType.AutoStart)]
        [InlineData(false, ServiceStartType.StartOnDemand)]
        internal void ItCanUpdateAnExistingService(bool autoStart, ServiceStartType serviceStartType)
        {
            // Given
            var existingService = GivenAServiceExists(TestServiceName, canBeUpdated: true);

            // When
            WhenATestServiceIsCreatedOrUpdated(TestServiceName, autoStart, startImmediately: true);

            // Then
            ThenTheServiceHasBeenUpdated(existingService, serviceStartType);
        }

        [Fact]
        public void ItThrowsIfServiceCannotBeUpdated()
        {
            // Given
            GivenAServiceExists(TestServiceName, canBeUpdated: false);

            // When
            Action action = () => WhenATestServiceIsCreatedOrUpdated(TestServiceName, autoStart: true, startImmediately: true);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItThrowsIfDescriptionCannotBeSet()
        {
            // Given
            var existingService = GivenAServiceExists(TestServiceName, canBeUpdated: true);
            GivenTheDescriptionOfAServiceCannotBeUpdated(existingService);

            // When
            Action action = () => WhenATestServiceIsCreatedOrUpdated(TestServiceName, autoStart: true, startImmediately: true);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItCanStartAServiceAfterChangingIt()
        {
            // Given
            var existingService = GivenAServiceExists(TestServiceName, canBeUpdated: true);

            // When
            WhenATestServiceIsCreatedOrUpdated(TestServiceName, autoStart: true, startImmediately: true);

            // Then
            A.CallTo(() => existingService.Start(false)).MustHaveHappened();
        }

        private void GivenTheServiceControlManagerCanBeOpened()
        {
            A.CallTo(() => serviceControlManager.IsInvalid).Returns(value: false);
            A.CallTo(() => nativeInterop.OpenSCManagerW(TestMachineName, TestDatabaseName, A<ServiceControlManagerAccessRights>._))
                .Returns(serviceControlManager);
        }

        private ServiceHandle GivenAServiceExists(string serviceName, bool canBeUpdated)
        {
            GivenTheServiceControlManagerCanBeOpened();

            var serviceHandle = A.Fake<ServiceHandle>(o => o.Wrapping(new ServiceHandle { NativeInterop = nativeInterop }));
            
            A.CallTo(() => serviceHandle.IsInvalid).Returns(value: false);

            ServiceHandle dummyServiceHandle;
            Win32Exception dummyWin32Exception;
            A.CallTo(()=>serviceControlManager.TryOpenService(serviceName, A<ServiceControlAccessRights>._, out dummyServiceHandle, out dummyWin32Exception))
                .Returns(value: true)
                .AssignsOutAndRefParameters(serviceHandle, null);

            if (canBeUpdated)
            {
                A.CallTo(
                        () =>
                            nativeInterop.ChangeServiceConfigW(serviceHandle, A<ServiceType>._, A<ServiceStartType>._, A<ErrorSeverity>._, A<string>._,
                                A<string>._, A<IntPtr>._, A<string>._, A<string>._, A<string>._, A<string>._))
                    .Returns(value: true);

                A.CallTo(() => nativeInterop.ChangeServiceConfig2W(serviceHandle, ServiceConfigInfoTypeLevel.ServiceDescription, A<IntPtr>._))
                    .Returns(value: true);

                A.CallTo(() => nativeInterop.StartServiceW(serviceHandle, A<uint>._, A<IntPtr>._))
                    .Returns(value: true);
            }

            return serviceHandle;
        }
        private void GivenTheDescriptionOfAServiceCannotBeUpdated(ServiceHandle existingService)
        {
            A.CallTo(() => nativeInterop.ChangeServiceConfig2W(existingService, ServiceConfigInfoTypeLevel.ServiceDescription, A<IntPtr>._))
                            .Returns(value: false);
        }

        private void WhenATestServiceIsCreatedOrUpdated(string testServiceName, bool autoStart, bool startImmediately)
        {
            sut.CreateOrUpdateService(testServiceName, TestServiceDisplayName, TestServiceDescription, TestServiceBinaryPath, TestCredentials, autoStart,
                startImmediately, TestServiceErrorSeverity);
        }

        private void ThenTheServiceHasBeenUpdated(ServiceHandle serviceHandle, ServiceStartType serviceStartType)
        {
            A.CallTo(
                    () =>
                        serviceHandle.ChangeConfig(TestServiceDisplayName, TestServiceBinaryPath, ServiceType.Win32OwnProcess,
                            serviceStartType, TestServiceErrorSeverity, TestCredentials))
                .MustHaveHappened();

            A.CallTo(
                    () =>
                        nativeInterop.ChangeServiceConfigW(serviceHandle, ServiceType.Win32OwnProcess, serviceStartType, TestServiceErrorSeverity,
                            TestServiceBinaryPath, null, IntPtr.Zero, null, TestCredentials.UserName, TestCredentials.Password, TestServiceDisplayName))
                .MustHaveHappened();

            A.CallTo(() => serviceHandle.SetDescription(TestServiceDescription))
                .MustHaveHappened();
            // interop logic for SetDescription() is covered in ServiceCreationTests
        }
    }
}
