using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace DasMulli.Win32.ServiceUtils.Tests.Win32ServiceHost
{
    public class ServiceLifecycleTests
    {
        private const string TestServiceName = "UnitTestService";
        private static readonly string[] TestServiceStartupArguments = {"Arg1", "Arg2"};

        private readonly INativeInterop nativeInterop = A.Fake<INativeInterop>();
        private readonly IWin32ServiceStateMachine serviceStateMachine = A.Fake<IWin32ServiceStateMachine>();
        private readonly ServiceStatusHandle serviceStatusHandle = A.Fake<ServiceStatusHandle>();

        private readonly List<ServiceStatus> reportedServiceStatuses = new List<ServiceStatus>();

        private ServiceStatusReportCallback statusReportCallback;
        private ServiceControlHandler serviceControlHandler;
        private IntPtr serviceControlContext;
        private readonly ServiceUtils.Win32ServiceHost sut;

        public ServiceLifecycleTests()
        {
            A.CallTo(() => serviceStateMachine.OnStart(A<string[]>._, A<ServiceStatusReportCallback>._))
                .Invokes((string[] args, ServiceStatusReportCallback callback) =>
                {
                    statusReportCallback = callback;
                });

            var dummy = new ServiceStatus();
            A.CallTo(() => nativeInterop.SetServiceStatus(null, ref dummy))
                .WithAnyArguments()
                .Returns(value: true)
                .AssignsOutAndRefParametersLazily((ServiceStatusHandle handle, ServiceStatus status) =>
                {
                    if (handle == serviceStatusHandle)
                    {
                        reportedServiceStatuses.Add(status);
                    }
                    return new object[] {status};
                });

            sut = new ServiceUtils.Win32ServiceHost(TestServiceName, serviceStateMachine, nativeInterop);
        }

        [Fact]
        public void ItCanStartServices()
        {
            // Given
            GivenServiceControlManagerIsExpectingService();

            // When
            sut.RunAsync();

            // Then
            A.CallTo(() => serviceStateMachine.OnStart(A<string[]>.That.IsSameSequenceAs(TestServiceStartupArguments), A<ServiceStatusReportCallback>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
            reportedServiceStatuses.Should().Contain(status => status.State == ServiceState.StartPending && status.AcceptedControlCommands == ServiceAcceptedControlCommandsFlags.None);
        }

        [Fact]
        public void ItCanStopServicesWhenRequestedByOS()
        {
            // Given
            GivenTheServiceHasBeenStarted();

            // When
            WhenTheOSSendsControlCommand(ServiceControlCommand.Stop, commandSpecificEventType: 0);

            // Then
            A.CallTo(() => serviceStateMachine.OnCommand(ServiceControlCommand.Stop, 0)).MustHaveHappened();
        }

        [Fact]
        public void ItResolvesRunAsyncTaskWhenServiceIsStopped()
        {
            // Given
            var runTask = GivenTheServiceIsShuttingDown();
            runTask.IsCompleted.Should().BeFalse();

            // When the service implementation reports stopped via callback
            statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 123, waitHint: 0);

            // Then
            runTask.IsCompleted.Should().BeTrue();
            runTask.Result.Should().Be(expected: 123);
        }

        [Fact]
        public void ItStopsWhenTheServiceStateMachineFailsOnStartup()
        {
            // Given
            GivenServiceControlManagerIsExpectingService();
            GivenTheStateMachineStartupCodeIsFaulty();

            // When
            var runTask = sut.RunAsync();

            // Then
            runTask.IsCompleted.Should().BeTrue();
            runTask.Result.Should().Be(expected: -1);
            reportedServiceStatuses.Should().HaveCount(expected: 2);
            reportedServiceStatuses[index: 0].State.Should().Be(ServiceState.StartPending);
            reportedServiceStatuses[index: 1].State.Should().Be(ServiceState.Stopped);
            reportedServiceStatuses[index: 1].Win32ExitCode.Should().Be(expected: -1);
        }

        [Fact]
        public void ItIgnoresStateChangesAfterStopHasBeenReported()
        {
            // Given
            var runTask = GivenTheServiceIsShuttingDown();

            // When
            statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 123, waitHint: 0);
            statusReportCallback(ServiceState.Running, ServiceAcceptedControlCommandsFlags.None, win32ExitCode: 123, waitHint: 0);

            // Then
            reportedServiceStatuses.Last().State.Should().Be(ServiceState.Stopped);
            runTask.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void ItThrowsPlatformNotSupportedWhenApiSetDllsAreMissing()
        {
            // Given
            A.CallTo(nativeInterop).Throws<DllNotFoundException>();

            // When
            Func<Task<int>> when = sut.RunAsync;

            // Then
            when.ShouldThrow<PlatformNotSupportedException>();
        }

        [Fact]
        public void ItThrowsWin32ExceptionWhenStartingServiceControlDispatcherFails()
        {
            // Given
            GivenStartingServiceControlDispatcherIsImpossible();

            // When
            Func<Task<int>> when = sut.RunAsync;

            // Then
            when.ShouldThrow<Win32Exception>();
        }

        [Fact]
        public void ItThrowsWin32ExceptionWhenRegisteringServiceControlHandlerFails()
        {
            // Given
            GivenRegisteringServiceControlHandlerIsImpossible();

            // When
            Func<Task<int>> when = sut.RunAsync;

            // Then
            when.ShouldThrow<Win32Exception>();
        }

        private void GivenTheStateMachineStartupCodeIsFaulty()
        {
            A.CallTo(() => serviceStateMachine.OnStart(A<string[]>._, A<ServiceStatusReportCallback>._))
                .Throws<Exception>();
        }

        private Task<int> GivenTheServiceHasBeenStarted()
        {
            GivenServiceControlManagerIsExpectingService();
            return sut.RunAsync();
        }

        private void GivenServiceControlManagerIsExpectingService()
        {
            A.CallTo(() => nativeInterop.StartServiceCtrlDispatcherW(A<ServiceTableEntry[]>._))
                .Invokes(new Action<ServiceTableEntry[]>(HandleNativeStartServiceCtrlDispatcherW))
                .Returns(value: true);
            A.CallTo(() => nativeInterop.RegisterServiceCtrlHandlerExW(TestServiceName, A<ServiceControlHandler>._, A<IntPtr>._))
                .ReturnsLazily((string serviceName, ServiceControlHandler controlHandler, IntPtr context) =>
                {
                    serviceName.Should().Be(TestServiceName);
                    serviceControlHandler = controlHandler;
                    serviceControlContext = context;
                    
                    return serviceStatusHandle;
                });
        }

        private void WhenTheOSSendsControlCommand(ServiceControlCommand controlCommand, uint commandSpecificEventType)
        {
            serviceControlHandler(controlCommand, commandSpecificEventType, IntPtr.Zero, serviceControlContext);
        }

        private Task<int> GivenTheServiceIsShuttingDown()
        {
            var task = GivenTheServiceHasBeenStarted();
            WhenTheOSSendsControlCommand(ServiceControlCommand.Stop, commandSpecificEventType: 0);
            return task;
        }

        private void GivenStartingServiceControlDispatcherIsImpossible()
        {
            A.CallTo(() => nativeInterop.StartServiceCtrlDispatcherW(A<ServiceTableEntry[]>._))
                .Returns(value: false);
        }

        private void GivenRegisteringServiceControlHandlerIsImpossible()
        {
            var statusHandle = new ServiceStatusHandle {NativeInterop = nativeInterop};
            A.CallTo(() => nativeInterop.RegisterServiceCtrlHandlerExW(A<string>._, A<ServiceControlHandler>._, A<IntPtr>._))
                .Returns(statusHandle);
        }

        private void HandleNativeStartServiceCtrlDispatcherW(ServiceTableEntry[] serviceTable)
        {
            var serviceTableEntry = serviceTable.FirstOrDefault(entry => entry.serviceName == TestServiceName);
            serviceTableEntry.Should().NotBeNull();

            var serviceMainFunction = Marshal.GetDelegateForFunctionPointer<ServiceMainFunction>(serviceTableEntry.serviceMainFunction);

            serviceMainFunction.Should().NotBeNull();
            
            var memoryBlocks = new IntPtr[TestServiceStartupArguments.Length + 1];
            memoryBlocks[0] = Marshal.StringToHGlobalUni(TestServiceName);
            var pointerBlock = Marshal.AllocHGlobal(IntPtr.Size * memoryBlocks.Length);
            Marshal.WriteIntPtr(pointerBlock, memoryBlocks[0]);
            
            for (var i = 0; i < TestServiceStartupArguments.Length; i++)
            {
                var pStr = Marshal.StringToHGlobalUni(TestServiceStartupArguments[i]);
                memoryBlocks[i+1] = pStr;
                Marshal.WriteIntPtr(pointerBlock, (i+1) * IntPtr.Size, pStr);
            }

            try
            {
                serviceMainFunction.Invoke(memoryBlocks.Length, pointerBlock);
            }
            finally
            {
                Marshal.FreeHGlobal(pointerBlock);
                foreach (var ptr in memoryBlocks)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }
}
