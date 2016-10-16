using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FakeItEasy;
using Xunit;

namespace CSS.Win32Service.Tests
{
    [SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
    public class SimpleServiceStateMachineTests
    {
        private readonly ServiceStatusReportCallback statusReportCallback = A.Fake<ServiceStatusReportCallback>();
        private readonly IWin32Service serviceImplmentation = A.Fake<IWin32Service>();

        // subject under test
        private readonly IWin32ServiceStateMachine sut;

        public SimpleServiceStateMachineTests()
        {
            sut = new SimpleServiceStateMachine(serviceImplmentation);
        }

        [Fact]
        public void ItShallStartImplementationAndReportStarted()
        {
            // When
            sut.OnStart(statusReportCallback);

            // Then
            A.CallTo(() => serviceImplmentation.Start()).MustHaveHappened();
            A.CallTo(() => statusReportCallback(ServiceState.Running, ServiceAcceptedControlCommandsFlags.Stop, 0, 0)).MustHaveHappened();
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
            sut.OnStart(statusReportCallback);

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
            sut.OnStart(statusReportCallback);
        }

        
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static IEnumerable<object[]> UnsupportedCommandExamples
        {
            get
            {
                yield return new object[] {ServiceControlCommand.Pause};
                yield return new object[] {ServiceControlCommand.Continue};
                yield return new object[] {ServiceControlCommand.Shutdown};
                yield return new object[] {ServiceControlCommand.PowerEvent};
            }
        }
    }
}
