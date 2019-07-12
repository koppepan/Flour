using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Example
{
	public class DebugHandler : MonoBehaviour
	{
		const int LogCacheCount = 50;

		DebugDialogCreator creator;
		readonly Dictionary<LogType, List<Tuple<string, string>>> logMap = new Dictionary<LogType, List<Tuple<string, string>>>();

		public void Initialize(DebugDialogCreator creator)
		{
			this.creator = creator;

			Observable.FromEvent(
				h => Application.logMessageReceived += LogMessageReceived,
				h => Application.logMessageReceived -= LogMessageReceived).Subscribe().AddTo(this);

#if UNITY_ANDROID || UNITY_IOS
			var mouseDownStream = Observable.EveryUpdate().Select(_ => Input.touchCount).Pairwise().Where(pair => pair.Current >= 3 && pair.Previous < 3);
			var mouseUpStream = Observable.EveryUpdate().Select(_ => Input.touchCount).Pairwise().Where(pair => pair.Current < 3 && pair.Previous >= 3);
#elif UNITY_EDITOR || UNITY_STANDALONE
			var mouseDownStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(1));
			var mouseUpStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(1));
#else
			var mouseDownStream = Observable.Empty<long>();
			var mouseUpStream = Observable.Empty<long>();
#endif

			mouseDownStream
				.SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(1)))
				.TakeUntil(mouseUpStream)
				.RepeatUntilDestroy(this)
#if UNITY_ANDROID || UNITY_IOS
				.Subscribe(_ => OpenDialog(Input.GetTouch(0).position))
#elif UNITY_EDITOR || UNITY_STANDALONE
				.Subscribe(_ => OpenDialog(Input.mousePosition))
#else
				.Subscribe()
#endif
				.AddTo(this);
		}

		public void OpenDialog(Vector2 position)
		{
			creator.OpenDebugDialog(position);
		}

		private void LogMessageReceived(string body, string stackTrace, LogType logType)
		{
			if (logType == LogType.Log)
			{
				return;
			}
			if (!logMap.ContainsKey(logType))
			{
				logMap[logType] = new List<Tuple<string, string>>();
			}
			logMap[logType].Add(Tuple.Create(body, stackTrace));

			if (logMap[logType].Count >= LogCacheCount)
			{
				logMap[logType].RemoveAt(0);
			}
		}
	}
}

