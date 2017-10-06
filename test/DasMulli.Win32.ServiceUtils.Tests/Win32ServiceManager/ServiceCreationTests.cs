using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace DasMulli.Win32.ServiceUtils.Tests.Win32ServiceManager
{
    public partial class ServiceCreationTests
    {
        private const string TestMachineName = "TestMachine";
        private const string TestDatabaseName = "TestDatabase";
        private const string TestServiceName = "UnitTestService";
        private const string TestServiceDisplayName = "A Test Service";
        private const string TestServiceDescription = "This describes the Test Service";
        private const string TestServiceBinaryPath = @"C:\Some\Where\service.exe --run-as-service";
        private const ErrorSeverity TestServiceErrorSeverity = ErrorSeverity.Ignore;
        private static readonly Win32ServiceCredentials TestCredentials = new Win32ServiceCredentials(@"ADomain\AUser", "WithAPassword");
        
        private static readonly ServiceFailureActions TestServiceFailureActions = new ServiceFailureActions(TimeSpan.FromDays(1), "A reboot message",
            "A restart Command",
            new List<ScAction>()
            {
                new ScAction {Delay = TimeSpan.FromSeconds(10), Type = ScActionType.ScActionRestart},
                new ScAction {Delay = TimeSpan.FromSeconds(30), Type = ScActionType.ScActionRestart},
                new ScAction {Delay = TimeSpan.FromSeconds(60), Type = ScActionType.ScActionRestart}
            });
        private static ServiceDefinitionBuilder CreateTestServiceDefinitionBuilder()
            => new ServiceDefinitionBuilder(TestServiceName)
                .WithDisplayName(TestServiceDisplayName)
                .WithDescription(TestServiceDescription)
                .WithBinaryPath(TestServiceBinaryPath)
                .WithCredentials(TestCredentials)
                .WithErrorSeverity(TestServiceErrorSeverity)
                .WithFailureActions(TestServiceFailureActions)
                .WithFailureActionsOnNonCrashFailures(true)
                .WithAutoStart(true);

        private static ServiceDefinition CreateTestServiceDefinition() => CreateTestServiceDefinitionBuilder().Build();

        private readonly INativeInterop nativeInterop = A.Fake<INativeInterop>();
        private readonly ServiceControlManager serviceControlManager;

        private readonly ServiceUtils.Win32ServiceManager sut;

        private readonly List<string> createdServices = new List<string>();
        private readonly Dictionary<string, string> serviceDescriptions = new Dictionary<string, string>();
        private readonly Dictionary<string, ServiceFailureActions> failureActions = new Dictionary<string, ServiceFailureActions>();
        private readonly Dictionary<string, bool> failureActionsFlags = new Dictionary<string, bool>();
        private bool? delayedAutoStartInfoSetOnNativeInterop;

        public ServiceCreationTests()
        {
            serviceControlManager = A.Fake<ServiceControlManager>(o => o.Wrapping(new ServiceControlManager {NativeInterop = nativeInterop}));

            sut = new ServiceUtils.Win32ServiceManager(TestMachineName, TestDatabaseName, nativeInterop);
        }

        [Theory]
        [InlineData(true, ServiceStartType.AutoStart)]
        [InlineData(false, ServiceStartType.StartOnDemand)]
        internal void ItCanCreateAService(bool autoStartArgument, ServiceStartType createdServiceStartType)
        {
            // Given
            GivenServiceCreationIsPossible(createdServiceStartType);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinitionBuilder().WithAutoStart(autoStartArgument).Build(), startImmediately: false);

            // Then
            createdServices.Should().Contain(TestServiceName);
        }

        [Fact]
        public void ItThrowsPlatformNotSupportedWhenApiSetDllsAreMissing()
        {
            // Given
            A.CallTo(nativeInterop).Throws<DllNotFoundException>();

            // When
            Action action = () => WhenATestServiceIsCreated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            action.ShouldThrow<PlatformNotSupportedException>();
        }

        [Fact]
        private void ItThrowsIfServiceControlManagerCannotBeOpened()
        {
            // Given
            GivenTheServiceControlManagerCannotBeOpenend();

            // When
            Action action = () => WhenATestServiceIsCreated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItThrowsIfCreatingAServiceIsImpossible()
        {
            // Given
            GivenTheServiceControlManagerCanBeOpened();
            GivenCreatingAServiceIsImpossible();

            // When
            Action action = () => WhenATestServiceIsCreated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItCanStartTheCreatedService()
        {
            // Given
            var service = GivenServiceCreationIsPossible(ServiceStartType.StartOnDemand);
            GivenTheServiceCanBeStarted(service);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinitionBuilder().WithAutoStart(false).Build(), startImmediately: true);

            // Then
            A.CallTo(() => service.Start(true)).MustHaveHappened();
        }

        [Fact]
        public void ItThrowsIfTheServiceCannotBeStarted()
        {
            // Given
            var service = GivenServiceCreationIsPossible(ServiceStartType.AutoStart);
            GivenTheServiceCannotBeStarted(service);

            // When
            Action action = () => WhenATestServiceIsCreated(CreateTestServiceDefinition(), startImmediately: true);

            // Then
            action.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItCanSetServiceDescription()
        {
            // Given
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            serviceDescriptions.Should().ContainKey(TestServiceName).WhichValue.Should().Be(TestServiceDescription);
        }

        [Fact]
        public void ItDoesNotCallApiForEmptyDescription()
        {
            // Given
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinitionBuilder().WithDescription(string.Empty).Build(), startImmediately: false);

            // Then
            serviceDescriptions.Should().NotContainKey(TestServiceName);
            A.CallTo(() => nativeInterop.ChangeServiceConfig2W(A<ServiceHandle>._, A<ServiceConfigInfoTypeLevel>.That.Matches(level => level == ServiceConfigInfoTypeLevel.ServiceDescription), A<IntPtr>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ItCanSetFailureActions()
        {
            // Given
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinition(), startImmediately: false);

            // Then
            failureActions.Should().ContainKey(TestServiceName).WhichValue.Should().Be(TestServiceFailureActions);

            failureActionsFlags.Should().ContainKey(TestServiceName).WhichValue.Should().Be(true);
        }

        [Fact]
        public void ItDoesNotCallApiForEmptyFailureActions()
        {
            // Given
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinitionBuilder().WithFailureActions(null).Build(), startImmediately: false);

            // Then
            failureActions.Should().NotContainKey(TestServiceName);
            A.CallTo(() => nativeInterop.ChangeServiceConfig2W(A<ServiceHandle>._, A<ServiceConfigInfoTypeLevel>.That.Matches(level => level == ServiceConfigInfoTypeLevel.FailureActions), A<IntPtr>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ItCanSetDelayedAutoStart()
        {
            // Given
            GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinitionBuilder().WithDelayedAutoStart(true).Build(), startImmediately: false);

            // then
            delayedAutoStartInfoSetOnNativeInterop.Should().Be(true);
        }

        [Fact]
        public void ItDoesNotUnsetDelayedAutoStartOnCreation()
        {
            // Given
            var handle = GivenServiceCreationIsPossible(ServiceStartType.AutoStart);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinitionBuilder().WithDelayedAutoStart(false).Build(), startImmediately: false);

            // then
            delayedAutoStartInfoSetOnNativeInterop.Should().Be(null);
            A.CallTo(()=>handle.SetDelayedAutoStartFlag(A<bool>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ItDoesNotSetDelayedAutoStartFlagWhenAutoStartIsDisabledOnCreation()
        {
            // Given
            var handle = GivenServiceCreationIsPossible(ServiceStartType.StartOnDemand);

            // When
            WhenATestServiceIsCreated(CreateTestServiceDefinitionBuilder().WithAutoStart(false).WithDelayedAutoStart(true).Build(), startImmediately: false);

            // then
            delayedAutoStartInfoSetOnNativeInterop.Should().Be(null);
            A.CallTo(() => handle.SetDelayedAutoStartFlag(A<bool>._)).MustNotHaveHappened();
        }

        private void WhenATestServiceIsCreated(ServiceDefinition serviceDefinition, bool startImmediately)
        {
            sut.CreateService(serviceDefinition, startImmediately);
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

        private ServiceHandle GivenServiceCreationIsPossible(ServiceStartType serviceStartType)
        {
            GivenTheServiceControlManagerCanBeOpened();

            var serviceHandle = A.Fake<ServiceHandle>(o => o.Wrapping(new ServiceHandle { NativeInterop = nativeInterop }));
            A.CallTo(() => serviceHandle.IsInvalid).Returns(value: false);

            A.CallTo(
                    () =>
                        nativeInterop.CreateServiceW(serviceControlManager, TestServiceName, TestServiceDisplayName, A<ServiceControlAccessRights>._,
                            ServiceType.Win32OwnProcess, serviceStartType, TestServiceErrorSeverity, TestServiceBinaryPath, null, IntPtr.Zero, null,
                            TestCredentials.UserName, TestCredentials.Password))
                .ReturnsLazily(call =>
                {
                    var serviceName = (string)call.Arguments[argumentIndex: 1];
                    createdServices.Add(serviceName);
                    A.CallTo(() => nativeInterop.ChangeServiceConfig2W(serviceHandle, A<ServiceConfigInfoTypeLevel>._, A<IntPtr>._))
                        .ReturnsLazily(CreateChangeService2WHandler(serviceName));
                    return serviceHandle;
                });
            return serviceHandle;
        }

        private Func<ServiceHandle, ServiceConfigInfoTypeLevel, IntPtr, bool> CreateChangeService2WHandler(string serviceName)
        {
            return (handle, infoLevel, info) =>
            {
                switch (infoLevel)
                {
                    case ServiceConfigInfoTypeLevel.ServiceDescription:
                        var serviceDescription = Marshal.PtrToStructure<ServiceDescriptionInfo>(info);
                        if (string.IsNullOrEmpty(serviceDescription.ServiceDescription))
                        {
                            serviceDescriptions.Remove(serviceName);
                        }
                        else
                        {
                            serviceDescriptions[serviceName] = serviceDescription.ServiceDescription;
                        }
                        return true;
                    case ServiceConfigInfoTypeLevel.FailureActions:
                        var failureAction = Marshal.PtrToStructure<ServiceFailureActionsInfo>(info);
                        if (failureAction.Actions?.Length == 0)
                        {
                            failureActions.Remove(serviceName);
                        }
                        else
                        {
                            failureActions[serviceName] = new ServiceFailureActions(failureAction.ResetPeriod, failureAction.RebootMsg, failureAction.Command, failureAction.Actions);
                        }
                        return true;
                    case ServiceConfigInfoTypeLevel.FailureActionsFlag:
                        var failureActionFlag = Marshal.PtrToStructure<ServiceFailureActionsFlag>(info);
                        failureActionsFlags[serviceName] = failureActionFlag.Flag;
                        return true;
                    case ServiceConfigInfoTypeLevel.DelayedAutoStartInfo:
                        if (info != IntPtr.Zero)
                        {
                            delayedAutoStartInfoSetOnNativeInterop = Marshal.ReadInt32(info) > 0;
                        }
                        else
                        {
                            delayedAutoStartInfoSetOnNativeInterop = null;
                        }
                        return true;
                    case ServiceConfigInfoTypeLevel.ServiceSidInfo:
                        break;
                    case ServiceConfigInfoTypeLevel.RequiredPrivilegesInfo:
                        break;
                    case ServiceConfigInfoTypeLevel.PreShutdownInfo:
                        break;
                    case ServiceConfigInfoTypeLevel.TriggerInfo:
                        break;
                    case ServiceConfigInfoTypeLevel.PreferredNode:
                        break;
                    case ServiceConfigInfoTypeLevel.LaunchProtected:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(infoLevel), infoLevel, null);
                }

                return false;
            };
        } 
        private void GivenCreatingAServiceIsImpossible()
        {
            A.CallTo(
                    () =>
                        nativeInterop.CreateServiceW(A<ServiceControlManager>._, A<string>._, A<string>._, A<ServiceControlAccessRights>._,
                            A<ServiceType>._, A<ServiceStartType>._, A<ErrorSeverity>._, A<string>._, A<string>._, IntPtr.Zero, A<string>._,
                            A<string>._, A<string>._))
                .Returns(CreateInvalidServiceHandle());
        }

        private ServiceHandle CreateInvalidServiceHandle()
        {
            var invalidServiceHandle = A.Fake<ServiceHandle>(o => o.Wrapping(new ServiceHandle { NativeInterop = nativeInterop }));
            A.CallTo(() => invalidServiceHandle.IsInvalid).Returns(value: true);
            return invalidServiceHandle;
        }

        private void GivenTheServiceCanBeStarted(ServiceHandle service)
        {
            A.CallTo(() => nativeInterop.StartServiceW(service, A<uint>._, A<IntPtr>._))
                .Returns(value: true);
        }

        private void GivenTheServiceCannotBeStarted(ServiceHandle service)
        {
            A.CallTo(() => nativeInterop.StartServiceW(service, A<uint>._, A<IntPtr>._))
                .Returns(value: false);
        }
    }
}
