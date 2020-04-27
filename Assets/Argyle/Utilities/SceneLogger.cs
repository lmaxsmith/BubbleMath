using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Argyle.Utilities
{
	public class SceneLogger : MonoBehaviour
	{
		public TextMeshProUGUI textUI;
		//public Scrollbar scrollbar;
		//public Text textUI;
		public bool displayStackTrace;

		// Start is called before the first frame update
		void Awake()
		{
			
			textUI.text = "";
		}
		private void OnEnable()
		{
			Application.logMessageReceived += LogHandler;
		}
		private void OnDisable()
		{
			Application.logMessageReceived -= LogHandler;
		}

		public void LogHandler(string message, string stackTrace, LogType logtype)
		{
			string displayString = "";

			//Add a prefix to differentiate log types. 
			switch (logtype)
			{
				case LogType.Error:
					displayString = "! ERROR: ";
					break;
				case LogType.Assert:
					displayString = "Assert: ";
					break;
				case LogType.Warning:
					displayString = "WARNING: ";
					break;
				case LogType.Log:
					displayString = "Log: ";
					break;
				case LogType.Exception:
					displayString = "!!! EXCEPTION: ";
					break;
				default:
					break;
			}

			displayString += message + Environment.NewLine;
			if (displayStackTrace)
			{
				displayString += "...stack trace: " + stackTrace;
			}


			Log(displayString);
		}

		public void Log(string logString)
		{
			textUI.text += logString + Environment.NewLine;
			//scrollbar.value = 0;
		}

		//Just so you can add content for testing in editor
		[ContextMenu("Add Test Line")]
		public void AddTestLine()
		{
			Debug.Log("This is just a test line that doesn't mean anything. ");
		}

	}

}

