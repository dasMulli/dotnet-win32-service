using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FakeItEasy;
using Xunit;

namespace DasMulli.Win32.ServiceUtils.Tests
{
    [SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
    public class PausableServiceStateMachineTests
    {
        private static readonly string[] TestStartupArguments = new string[] { "Arg1", "Arg2" };

        private readonly ServiceStatusReportCallback statusReportCallback = A.Fake<ServiceStatusReportCallback>();
        private readonly IPausableWin32Service serviceImplmentation = A.Fake<IPausableWin32Service>();

        // subject under test
        private readonly IWin32ServiceStateMachine sut;

        private ServiceStoppedCallback serviceStoppedCallbackPassedToImplementation;

        public PausableServiceStateMachineTests()
        {
            sut = new PausableServiceStateMachine(serviceImplmentation);
        }

        [Fact]
        public void ItShallStartImplementationAndReportStarted()
        {
            // When
            sut.OnStart(TestStartupArguments, statusReportCallback);

            // Then
            A.CallTo(() => serviceImplmentation.Start(TestStartupArguments, A<ServiceStoppedCallback>._)).MustHaveHappened();
            A.CallTo(() => statusReportCallback(ServiceState.Running, ServiceAcceptedControlCommandsFlags.PauseContinueStop, 0, 0)).MustHaveHappened();
        }

        [Fact]
        public void ItShallStopImplementationAndReportStopped()
        {
            // Given
            GivenTheServiceHasBeenStarted();

            // When
            sut.OnCommand(ServiceControlCommand.Stop, 0);

            // Then
            A.CallTo(() => serviceImplmentation.Stop()).MustHaveHappened();
            A.CallTo(() => statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, 0, 0)).MustHaveHappened();
        }

        [Fact]
        public void ItShallReportStoppedImplmentationThrowsOnStartup()
        {
            // Given
            A.CallTo(serviceImplmentation).Throws<Exception>();

            // When
            sut.OnStart(TestStartupArguments, statusReportCallback);

            // Then
            A.CallTo(() => statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, -1, 0))
                .MustHaveHappened();
        }

        [Fact]
        public void ItShallReportStoppedEvenIfServiceImplementationThrowsOnStop()
        {
            // Given
            GivenTheServiceHasBeenStarted();
            A.CallTo(serviceImplmentation).Throws<Exception>();

            // When
            sut.OnCommand(ServiceControlCommand.Stop, 0);

            // Then
            A.CallTo(() => statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, -1, 0))
                .MustHaveHappened();
        }

        [Fact]
        public void ItShallReportStoppedWhenServiceStoppedCallbackIsInvoked()
        {
            // Given
            GivenTheServiceHasBeenStarted();

            // When the stopped callback is invoked
            serviceStoppedCallbackPassedToImplementation();

            // Then
            A.CallTo(() => statusReportCallback(ServiceState.Stopped, ServiceAcceptedControlCommandsFlags.None, 0, 0))
                .MustHaveHappened();
        }

        [Fact]
        public void ItShallPauseImplementationAndReportPaused()
        {
            // Given
            GivenTheServiceHasBeenStarted();

            // When
            sut.OnCommand(ServiceControlCommand.Pause, 0);

            // Then
            A.CallTo(() => serviceImplmentation.Pause()).MustHaveHappened();
            A.CallTo(() => statusReportCallback(ServiceState.Paused, ServiceAcceptedControlCommandsFlags.PauseContinueStop, 0, 0)).MustHaveHappened();
        }

        [Fact]
        public void ItShallContinueImplementationAndReportStarted()
        {
            // Given
            GivenTheServiceHasBeenStarted();

            // When
            sut.OnCommand(ServiceControlCommand.Continue, 0);

            // Then
            A.CallTo(() => serviceImplmentation.Continue()).MustHaveHappened();
            A.CallTo(() => statusReportCallback(ServiceState.Running, ServiceAcceptedControlCommandsFlags.PauseContinueStop, 0, 0)).MustHaveHappened();
        }

        [Theory, MemberData(nameof(UnsupportedCommandExamples))]
        public void ItShallIgnoreUnsupportedCommands(ServiceControlCommand unsupportedCommand)
        {
            // Given
            GivenTheServiceHasBeenStarted();

            // When
            sut.OnCommand(unsupportedCommand, 0);

            // Then no other calls than the startup calls must have been made
            A.CallTo(statusReportCallback).MustHaveHappened(Repeated.NoMoreThan.Once);
            A.CallTo(serviceImplmentation).MustHaveHappened(Repeated.NoMoreThan.Once);
        }

        private void GivenTheServiceHasBeenStarted()
        {
            A.CallTo(() => serviceImplmentation.Start(null, null))
                .WithAnyArguments()
                .Invokes((string[] args, ServiceStoppedCallback stoppedCallback) =>
                {
                    serviceStoppedCallbackPassedToImplementation = stoppedCallback;
                });

            sut.OnStart(TestStartupArguments, statusReportCallback);
        }


        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Needed by test framework.")]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Needed by test framework.")]
        public static IEnumerable<object[]> UnsupportedCommandExamples
        {
            get
            {
                yield return new object[] { ServiceControlCommand.Shutdown };
                yield return new object[] { ServiceControlCommand.PowerEvent };
            }
        }
    }
}
