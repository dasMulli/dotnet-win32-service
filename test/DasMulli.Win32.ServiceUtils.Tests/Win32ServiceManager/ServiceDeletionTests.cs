using System;
using System.ComponentModel;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace DasMulli.Win32.ServiceUtils.Tests.Win32ServiceManager
{
    public class ServiceDeletionTests
    {
        private const string TestMachineName = "TestMachine";
        private const string TestDatabaseName = "TestDatabase";
        private const string TestServiceName = "UnitTestService";

        private readonly INativeInterop nativeInterop = A.Fake<INativeInterop>();
        private readonly ServiceControlManager serviceControlManager;

        private readonly ServiceUtils.Win32ServiceManager sut;

        public ServiceDeletionTests()
        {
            serviceControlManager = A.Fake<ServiceControlManager>(o => o.Wrapping(new ServiceControlManager { NativeInterop = nativeInterop }));

            sut = new ServiceUtils.Win32ServiceManager(TestMachineName, TestDatabaseName, nativeInterop);
        }

        [Theory]
        [InlineData(true, ServiceStartType.AutoStart)]
        [InlineData(false, ServiceStartType.StartOnDemand)]
        internal void ItCanDeleteAService(bool autoStartArgument, ServiceStartType createdServiceStartType)
        {
            // Given
            GivenTheServiceControlManagerCanBeOpened();
            var service = GivenTheTestServiceExists();
            GivenTheServiceCanBeDeleted(service);

            // When
            sut.DeleteService(TestServiceName);

            // Then
            A.CallTo(() => service.Delete()).MustHaveHappened();
        }

        [Fact]
        public void ItThrowsPlatformNotSupportedWhenApiSetDllsAreMissing()
        {
            // Given
            A.CallTo(nativeInterop).Throws<DllNotFoundException>();

            // When
            Action action = () => sut.DeleteService(TestServiceName);

            // Then
            action.ShouldThrow<PlatformNotSupportedException>();
        }

        [Fact]
        private void ItThrowsIfServiceControlManagerCannotBeOpened()
        {
            // Given
            GivenTheServiceControlManagerCannotBeOpenend();

            // When
            Action action = () => sut.DeleteService(TestServiceName);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        private void ItThrowsIfServiceDoesNotExist()
        {
            // Given
            GivenTheServiceControlManagerCanBeOpened();
            GivenTheTestServiceDoesNotExist();

            // When
            Action action = () => sut.DeleteService(TestServiceName);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        private void ItThrowsIfServiceCannotBeDeleted()
        {
            // Given
            GivenTheServiceControlManagerCanBeOpened();
            var service = GivenTheTestServiceExists();
            GivenTheServiceCannotBeDeleted(service);

            // When
            Action action = () => sut.DeleteService(TestServiceName);

            // Then
            action.ShouldThrow<Win32Exception>();
        }
        
        private void GivenTheServiceControlManagerCanBeOpened()
        {
            A.CallTo(() => serviceControlManager.IsInvalid).Returns(value: false);
            A.CallTo(() => nativeInterop.OpenSCManagerW(TestMachineName, TestDatabaseName, A<ServiceControlManagerAccessRights>._))
                .Returns(serviceControlManager);
        }

        private void GivenTheServiceControlManagerCannotBeOpenend()
        {
            A.CallTo(() => serviceControlManager.IsInvalid).Returns(value: true);
            A.CallTo(() => nativeInterop.OpenSCManagerW(TestMachineName, TestDatabaseName, A<ServiceControlManagerAccessRights>._))
                .Returns(serviceControlManager);
        }

        private ServiceHandle GivenTheTestServiceExists()
        {
            var svc = A.Fake<ServiceHandle>(o => o.Wrapping(new ServiceHandle {NativeInterop = nativeInterop}));
            A.CallTo(() => svc.IsInvalid).Returns(value: false);
            A.CallTo(() => nativeInterop.OpenServiceW(serviceControlManager, TestServiceName, A<ServiceControlAccessRights>._))
                .Returns(svc);
            return svc;
        }
        
        private void GivenTheTestServiceDoesNotExist()
        {
            var svc = A.Fake<ServiceHandle>(o => o.Wrapping(new ServiceHandle { NativeInterop = nativeInterop }));
            A.CallTo(() => svc.IsInvalid).Returns(value: true);
            A.CallTo(() => nativeInterop.OpenServiceW(serviceControlManager, TestServiceName, A<ServiceControlAccessRights>._))
                .Returns(svc);
        }

        private void GivenTheServiceCanBeDeleted(ServiceHandle service)
        {
            A.CallTo(() => nativeInterop.DeleteService(service)).Returns(value: true);
        }

        private void GivenTheServiceCannotBeDeleted(ServiceHandle service)
        {
            A.CallTo(() => nativeInterop.DeleteService(service)).Returns(value: false);
        }
    }
}
