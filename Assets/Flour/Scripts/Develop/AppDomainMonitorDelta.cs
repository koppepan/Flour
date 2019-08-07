using System;
using UnityEngine;

namespace Flour.Develop
{
	public sealed class AppDomainMonitorDelta : IDisposable
	{

		private readonly AppDomain appDomain;
		private readonly TimeSpan thisADCpu;
		private readonly long thisADMemoryInUse;
		private readonly long thisADMemoryAllocated;

		static AppDomainMonitorDelta()
		{
			AppDomain.MonitoringIsEnabled = true;
		}

		public AppDomainMonitorDelta(AppDomain domain)
		{
			appDomain = domain ?? AppDomain.CurrentDomain;
			thisADCpu = appDomain.MonitoringTotalProcessorTime;
			thisADMemoryInUse = appDomain.MonitoringSurvivedMemorySize;
			thisADMemoryAllocated = appDomain.MonitoringTotalAllocatedMemorySize;
		}

		public void Dispose()
		{
			GC.Collect();

			Debug.LogFormat("FriendlyName={0}, CPU={1}ms", appDomain.FriendlyName, appDomain.MonitoringTotalProcessorTime - thisADCpu);
			Debug.LogFormat("  Allocated {0:N0} bytes of which {1:N0} survived GCs",
				appDomain.MonitoringTotalAllocatedMemorySize - thisADMemoryAllocated,
				appDomain.MonitoringSurvivedMemorySize - thisADMemoryInUse);
		}
	}
}
