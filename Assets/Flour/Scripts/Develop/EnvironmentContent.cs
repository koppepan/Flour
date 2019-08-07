using System;
using UnityEngine;

namespace Flour.Develop
{
	public class SystemMemory : IContent<string>
	{
		public string GetValue() => string.Format("s memory:\t{0:N0} M", SystemInfo.systemMemorySize);
	}

	public class UsedMemory : IContent<string>
	{
		public string GetValue() => string.Format("u memory:\t{0:N0} K", GC.GetTotalMemory(false) / 1024);
	}

	public class GCCount : IContent<string>
	{
		readonly int prevCount;
		public GCCount() => prevCount = GC.CollectionCount(0);
		public string GetValue() => string.Format("GC count:\t{0}", GC.CollectionCount(0) - prevCount);
	}

	public class FrameCount : IContent<string>
	{
		float prev;
		int frame = 0;

		float fps;

		public string GetValue() => string.Format("{0:00}/{1} fps", fps, Application.targetFrameRate);

		public void Update()
		{
			frame++;
			float time = Time.realtimeSinceStartup - prev;

			if (time >= 0.5f)
			{
				fps = frame / time;

				frame = 0;
				prev = Time.realtimeSinceStartup;
			}
		}
	}
}
