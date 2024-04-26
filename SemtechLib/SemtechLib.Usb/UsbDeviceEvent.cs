using System;
using System.Management;

namespace SemtechLib.Usb
{
	internal sealed class UsbDeviceEvent
	{
		private readonly string deviceId = "";

		private ManagementEventWatcher creationEventWatcher;

		private ManagementEventWatcher deletionEventWatcher;

		public event EventHandler Attached;

		public event EventHandler Detached;

		public UsbDeviceEvent()
		{
			creationEventWatcher = null;
			_ = new ManagementOperationObserver();
			var scope = new ManagementScope("root\\CIMV2")
			{
				Options = 
				{
					EnablePrivileges = true
				}
			};
			try
			{
				var wqlEventQuery = new WqlEventQuery
				{
					EventClassName = "__InstanceCreationEvent",
					WithinInterval = new TimeSpan(0, 0, 3),
					Condition = "TargetInstance ISA 'Win32_USBControllerDevice'"
				};
				Console.WriteLine(wqlEventQuery.QueryString);
				creationEventWatcher = new ManagementEventWatcher(scope, wqlEventQuery);
				creationEventWatcher.EventArrived += creationEventWatcher_EventArrived;
				creationEventWatcher.Start();
				wqlEventQuery.EventClassName = "__InstanceDeletionEvent";
				wqlEventQuery.WithinInterval = new TimeSpan(0, 0, 3);
				wqlEventQuery.Condition = "TargetInstance ISA 'Win32_USBControllerdevice'";
				Console.WriteLine(wqlEventQuery.QueryString);
				deletionEventWatcher = new ManagementEventWatcher(scope, wqlEventQuery);
				deletionEventWatcher.EventArrived += deletionEventWatcher_EventArrived;
				deletionEventWatcher.Start();
			}
			catch
			{
				Dispose();
			}
		}

		public UsbDeviceEvent(string deviceId)
			: this()
		{
			this.deviceId = deviceId;
		}

		private void OnAttached()
		{
            Attached?.Invoke(this, EventArgs.Empty);
        }

		private void OnDetached()
		{
            Detached?.Invoke(this, EventArgs.Empty);
        }

		private void Dispose()
		{
			creationEventWatcher?.Stop();
			deletionEventWatcher?.Stop();
			creationEventWatcher = null;
			deletionEventWatcher = null;
		}

		private void creationEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
		{
			foreach (var property in e.NewEvent.Properties)
			{
				// ManagementBaseObject managementBaseObject = null;
				if (property.Value is not ManagementBaseObject managementBaseObject)
				{
					continue;
				}
				foreach (var property2 in managementBaseObject.Properties)
				{
					if (property2.Value is string text && text.Replace("\\", "").Contains(deviceId.Replace("\\", "")))
					{
						OnAttached();
					}
				}
			}
		}

		private void deletionEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
		{
			foreach (var property in e.NewEvent.Properties)
			{
				// ManagementBaseObject managementBaseObject = null;
				if (property.Value is not ManagementBaseObject managementBaseObject)
				{
					continue;
				}
				foreach (var property2 in managementBaseObject.Properties)
				{
					if (property2.Value is string text && text.Replace("\\", "").Contains(deviceId.Replace("\\", "")))
					{
						OnDetached();
					}
				}
			}
		}
	}
}
