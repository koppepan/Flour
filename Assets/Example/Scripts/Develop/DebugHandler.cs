using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UniRx;
using Flour.Develop;

namespace Example
{
	public class DebugHandler : MonoBehaviour
	{
		const int LogCacheCount = 50;

		DebugDialogCreator creator;
		readonly Dictionary<LogType, List<Tuple<string, string>>> logMap = new Dictionary<LogType, List<Tuple<string, string>>>();

		bool environmentEnabled = false;
		readonly StringBuilder stringBuilder = new StringBuilder();
		readonly FrameCount frameCount = new FrameCount();
		IContent<string>[] environmentContetns;

		public void Initialize(DebugDialogCreator creator)
		{
			this.creator = creator;

			environmentContetns = new IContent<string>[]
			{
				frameCount,
				new SystemMemory(),
				new UsedMemory(),
				new GCCount(),
			};

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
				.Subscribe(_ => OpenDialog(GetInputPosition()))
				.AddTo(this);

			mouseDownStream
				.TakeUntil(mouseUpStream)
				.RepeatUntilDestroy(this)
				.Where(_ => GetInputPosition().x < 100 && GetInputPosition().y > Screen.height - 100)
				.Subscribe(_ => environmentEnabled = !environmentEnabled).AddTo(this);
		}

		private Vector2 GetInputPosition()
		{
#if UNITY_ANDROID || UNITY_IOS
			return Input.GetTouch(0).position;
#elif UNITY_EDITOR || UNITY_STANDALONE
			return Input.mousePosition;
#else
			return default;
#endif
		}

		private void OpenDialog(Vector2 position)
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

		private void Update()
		{
			if (!environmentEnabled) return;
			frameCount.Update();
		}

		private void OnGUI()
		{
			if (!environmentEnabled) return;

			stringBuilder.Clear();

			for (int i = 0; i < environmentContetns.Length; i++)
			{
				stringBuilder.Append(environmentContetns[i].GetValue());
				stringBuilder.Append(i == environmentContetns.Length - 1 ? string.Empty : Environment.NewLine);
			}

			using (var scope = new GUILayout.HorizontalScope("box")) GUILayout.Label(stringBuilder.ToString());
		}
	}
}

