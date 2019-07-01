using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Flour.Develop
{
	class DebugInputDate : MonoBehaviour, IContent<DateTime>
	{
		[SerializeField] InputField inputField = default;

		[Header("date")]
		[SerializeField] Dropdown yearDropdown = default;
		[SerializeField] Dropdown monthDropdown = default;
		[SerializeField] Dropdown dayDropdown = default;

		[Header("time")]
		[SerializeField] Dropdown hourDropdown = default;
		[SerializeField] Dropdown minuteDropdown = default;
		[SerializeField] Dropdown secondsDropdown = default;

		private int Year { get { return ToInt(yearDropdown); } }
		private int Month { get { return ToInt(monthDropdown); } }
		private int Day { get { return ToInt(dayDropdown); } }

		private int Hour { get { return ToInt(hourDropdown); } }
		private int Minute { get { return ToInt(minuteDropdown); } }
		private int Seconds { get { return ToInt(secondsDropdown); } }

		private Action<DateTime> onChanged;

		private DateTime _currentTime;
		private DateTime CurrentTime
		{
			get
			{
				return _currentTime;
			}
			set
			{
				_currentTime = value;
				onChanged?.Invoke(value);
			}
		}

		int ToInt(Dropdown dropdown)
		{
			return int.Parse(dropdown.options[dropdown.value].text);
		}

		public DateTime GetValue()
		{
			return CurrentTime;
		}

		public void Setup(DateTime now, Action<DateTime> onChanged)
		{
			this.onChanged = onChanged;

			_currentTime = now;
			inputField.text = now.ToString();

			yearDropdown.options = Enumerable.Range(2018, 10).Select(p => new Dropdown.OptionData(string.Format("{0:0000}", p))).ToList();
			monthDropdown.options = Enumerable.Range(1, 12).Select(p => new Dropdown.OptionData(string.Format("{0:00}", p))).ToList();
			UpdateDays(now.Year, now.Month);

			hourDropdown.options = Enumerable.Range(0, 24).Select(p => new Dropdown.OptionData(string.Format("{0:00}", p))).ToList();
			minuteDropdown.options = Enumerable.Range(0, 60).Select(p => new Dropdown.OptionData(string.Format("{0:00}", p))).ToList();
			secondsDropdown.options = Enumerable.Range(0, 60).Select(p => new Dropdown.OptionData(string.Format("{0:00}", p))).ToList();

			UpdateDropdown(_currentTime);

			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(_ => UpdateDropdown());

			yearDropdown.onValueChanged.RemoveAllListeners();
			yearDropdown.onValueChanged.AddListener(_ =>
			{
				UpdateDays();
				UpdateInputField();
			});

			monthDropdown.onValueChanged.RemoveAllListeners();
			monthDropdown.onValueChanged.AddListener(_ =>
			{
				UpdateDays();
				UpdateInputField();
			});

			dayDropdown.onValueChanged.RemoveAllListeners();
			dayDropdown.onValueChanged.AddListener(_ => UpdateInputField());

			hourDropdown.onValueChanged.RemoveAllListeners();
			hourDropdown.onValueChanged.AddListener(_ => UpdateInputField());

			minuteDropdown.onValueChanged.RemoveAllListeners();
			minuteDropdown.onValueChanged.AddListener(_ => UpdateInputField());

			secondsDropdown.onValueChanged.RemoveAllListeners();
			secondsDropdown.onValueChanged.AddListener(_ => UpdateInputField());
		}

		void UpdateDays()
		{
			var year = int.Parse(yearDropdown.options[yearDropdown.value].text);
			var month = int.Parse(monthDropdown.options[monthDropdown.value].text);
			UpdateDays(year, month);
		}
		void UpdateDays(int year, int month)
		{
			var days = DateTime.DaysInMonth(year, month);
			var currentDay = dayDropdown.value;
			dayDropdown.options = Enumerable.Range(1, days).Select(p => new Dropdown.OptionData(string.Format("{0:00}", p))).ToList();
			dayDropdown.value = Mathf.Clamp(currentDay, 0, days - 1);
		}

		void UpdateDropdown(DateTime dateTime)
		{
			yearDropdown.value = yearDropdown.options.FindIndex(x => x.text == dateTime.Year.ToString("0000"));
			monthDropdown.value = monthDropdown.options.FindIndex(x => x.text == dateTime.Month.ToString("00"));
			dayDropdown.value = dayDropdown.options.FindIndex(x => x.text == dateTime.Day.ToString("00"));

			hourDropdown.value = hourDropdown.options.FindIndex(x => x.text == dateTime.Hour.ToString("00"));
			minuteDropdown.value = minuteDropdown.options.FindIndex(x => x.text == dateTime.Minute.ToString("00"));
			secondsDropdown.value = secondsDropdown.options.FindIndex(x => x.text == dateTime.Second.ToString("00"));
		}
		void UpdateDropdown()
		{
			if (DateTime.TryParse(inputField.text, out var result))
			{
				CurrentTime = result;
				UpdateDropdown(result);
			}
			inputField.text = CurrentTime.ToString();
		}

		void UpdateInputField()
		{
			var date = new DateTime(Year, Month, Day, Hour, Minute, Seconds);
			inputField.text = date.ToString();
			CurrentTime = date;
		}
	}
}
