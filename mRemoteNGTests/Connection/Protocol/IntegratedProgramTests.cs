﻿using System.Collections;
using mRemoteNG.App;
using mRemoteNG.Connection;
using mRemoteNG.Connection.Protocol;
using mRemoteNG.Tools;
using mRemoteNG.UI.Window;
using NUnit.Framework;
using WeifenLuo.WinFormsUI.Docking;

namespace mRemoteNGTests.Connection.Protocol
{
	public class IntegratedProgramTests
	{
		private readonly ExternalTool _extTool = new ExternalTool
		{
			DisplayName = "notepad",
			FileName = @"%windir%\system32\notepad.exe",
			Arguments = "",
			TryIntegrate = true
		};

		
		[Test]
		public void CanStartExternalApp()
		{
			SetExternalToolList(_extTool);
			var sut = new IntegratedProgram();
			sut.InterfaceControl = BuildInterfaceControl("notepad", sut);
			sut.Initialize();
			var appStarted = sut.Connect();
			sut.Disconnect();
			Assert.That(appStarted);
		}

		[Test]
		public void ConnectingToExternalAppThatDoesntExistDoesNothing()
		{
			SetExternalToolList(_extTool);
			var sut = new IntegratedProgram();
			sut.InterfaceControl = BuildInterfaceControl("doesntExist", sut);
			var appInitialized = sut.Initialize();
			Assert.That(appInitialized, Is.False);
		}

		private void SetExternalToolList(ExternalTool externalTool)
		{
			Runtime.ExternalTools = new ArrayList {externalTool};
		}

		private InterfaceControl BuildInterfaceControl(string extAppName, ProtocolBase sut)
		{
			var connectionWindow = new ConnectionWindow(new DockContent());
			var connectionInfo = new ConnectionInfo {ExtApp = extAppName};
			return new InterfaceControl(connectionWindow, sut, connectionInfo);
		}
	}
}