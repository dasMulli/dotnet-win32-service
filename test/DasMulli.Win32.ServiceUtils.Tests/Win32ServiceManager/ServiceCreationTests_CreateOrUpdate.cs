using System;
using System.ComponentModel;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace DasMulli.Win32.ServiceUtils.Tests.Win32ServiceManager
{
    public partial class ServiceCreationTests
    {
        [Theory]
        [InlineData(true, ServiceStartType.AutoStart)]
        [InlineData(false, ServiceStartType.StartOnDemand)]
        internal void ItCanCreateAServiceOnCreateOrUpdate(bool autoStartArgument, ServiceStartType createdServiceStartType)
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            GivenServiceCreationIsPossible(createdServiceStartType);

            // When
            WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinitionBuilder().WithAutoStart(autoStartArgument).Build(), startImmediately: false);

            // Then
            createdServices.Should().Contain(TestServiceName);
        }

        [Fact]
        public void ItThrowsPlatformNotSupportedWhenApiSetDllsAreMissingOnCreateOrUpdate()
        {
            // Given
            A.CallTo(nativeInterop).Throws<DllNotFoundException>();

            // When
            Action action = () => WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            action.ShouldThrow<PlatformNotSupportedException>();
        }

        [Fact]
        private void ItThrowsIfServiceControlManagerCannotBeOpenedOnCreateOrUpdate()
        {
            // Given
            GivenTheServiceControlManagerCannotBeOpenend();

            // When
            Action action = () => WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItThrowsIfCreatingAServiceIsImpossibleOnCreateOrUpdate()
        {
            // Given
            GivenTheServiceControlManagerCanBeOpened();
            GivenAServiceDoesNotExist(TestServiceName);
            GivenCreatingAServiceIsImpossible();

            // When
            Action action = () => WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItCanStartTheCreatedServiceOnCreateOrUpdate()
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            var service = GivenServiceCreationIsPossible(ServiceStartType.AutoStart);
            GivenTheServiceCanBeStarted(service);

            // When
            WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinition(), startImmediately: true);

            // Then
            A.CallTo(() => service.Start(true)).MustHaveHappened();
        }

        [Fact]
        public void ItThrowsIfTheServiceCannotBeStartedOnCreateOrUpdate()
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            var service = GivenServiceCreationIsPossible(ServiceStartType.AutoStart);
            GivenTheServiceCannotBeStarted(service);

            // When
            Action action = () => WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinition(), startImmediately: true);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItCanSetServiceDescriptionOnCreateOrUpdate()
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            serviceDescriptions.Should().ContainKey(TestServiceName).WhichValue.Should().Be(TestServiceDescription);
        }

        [Fact]
        public void ItDoesNotCallApiForEmptyDescriptionOnCreateOrUpdate()
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinitionBuilder().WithDescription(string.Empty).Build(), startImmediately: false);

            // Then
            serviceDescriptions.Should().NotContainKey(TestServiceName);
            A.CallTo(() => nativeInterop.ChangeServiceConfig2W(A<ServiceHandle>._, A<ServiceConfigInfoTypeLevel>.That.Matches(level => level == ServiceConfigInfoTypeLevel.ServiceDescription), A<IntPtr>._)).MustNotHaveHappened();
        }


        [Fact]
        public void ItDoesNotCallApiForNullFailureActionsOnCreateOrUpdate()
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinitionBuilder().WithFailureActions(null).Build(), startImmediately: false);

            // Then
            failureActions.Should().NotContainKey(TestServiceName);
            A.CallTo(() => nativeInterop.ChangeServiceConfig2W(A<ServiceHandle>._, A<ServiceConfigInfoTypeLevel>.That.Matches(level => level == ServiceConfigInfoTypeLevel.FailureActions), A<IntPtr>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ItCanSetDelayedAutoStartOnCreateOrUpdate()
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinitionBuilder().WithDelayedAutoStart(true).Build(), startImmediately: false);

            // then
            delayedAutoStartInfoSetOnNativeInterop.Should().Be(true);
        }

        [Fact]
        public void ItDoesNotUnsetDelayedAutoStartOnCreationInCreateOrUpdate()
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            var handle = GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinitionBuilder().WithDelayedAutoStart(false).Build(), startImmediately: false);

            // then
            delayedAutoStartInfoSetOnNativeInterop.Should().Be(null);
            A.CallTo(() => handle.SetDelayedAutoStartFlag(A<bool>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ItDoesNotSetDelayedAutoStartFlagWhenAutoStartIsDisabledOnCreateOrUpdate()
        {
            // Given
            GivenAServiceDoesNotExist(TestServiceName);
            var handle = GivenServiceCreationIsPossible(ServiceStartType.StartOnDemand);

            // When
            WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinitionBuilder().WithAutoStart(false).WithDelayedAutoStart(true).Build(), startImmediately: false);

            // then
            delayedAutoStartInfoSetOnNativeInterop.Should().Be(null);
            A.CallTo(() => handle.SetDelayedAutoStartFlag(A<bool>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ItThrowsUnexpectedWin32ExceptionFromTryingToOpenServiceOnCreateOrUpdate()
        {
            // Given
            const int unkownWin32ErrorCode = -1;
            GivenTheServiceControlManagerCanBeOpened();
            GivenOpeningServiceReturnsWin32Error(TestServiceName, unkownWin32ErrorCode);

            // When
            Action action = () => WhenATestServiceIsCreatedOrUpdated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            action.ShouldThrow<Win32Exception>().Which.NativeErrorCode.Should().Be(unkownWin32ErrorCode);
        }

        private void GivenAServiceDoesNotExist(string serviceName)
        {
            GivenOpeningServiceReturnsWin32Error(serviceName, KnownWin32ErrorCoes.ERROR_SERVICE_DOES_NOT_EXIST);
        }

        private void GivenOpeningServiceReturnsWin32Error(string serviceName, int errorServiceDoesNotExist)
        {
            ServiceHandle tmpHandle;
            Win32Exception tmpWin32Exception;
            A.CallTo(() => serviceControlManager.TryOpenService(serviceName, A<ServiceControlAccessRights>._, out tmpHandle, out tmpWin32Exception))
                .Returns(value: false)
                .AssignsOutAndRefParameters(CreateInvalidServiceHandle(), new Win32Exception(errorServiceDoesNotExist));
        }

        private void WhenATestServiceIsCreatedOrUpdated(ServiceDefinition serviceDefinition, bool startImmediately)
        {
            sut.CreateOrUpdateService(serviceDefinition, startImmediately);
        }
    }
}
