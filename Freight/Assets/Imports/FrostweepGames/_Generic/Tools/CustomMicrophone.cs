using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;	
#endif

namespace FrostweepGames.Plugins.Native
{
	public class CustomMicrophone
	{
		public static string[] devices
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
			get { return Microphone.devices; }
#elif UNITY_WEBGL
			get { return FrostweepGames.Plugins.WebGL.Microphone.Instance.GetMicrophoneDevices(); }
#else
			get { return new string[0]; }
#endif
		}

		public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency)
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
			return Microphone.Start(deviceName, loop, lengthSec, frequency);
#elif UNITY_WEBGL
			return FrostweepGames.Plugins.WebGL.Microphone.Instance.Start(deviceName, loop, lengthSec, frequency);
#else
			throw new System.NotImplementedException("microphone not implemented yet");
#endif
		}

		public static bool IsRecording(string deviceName)
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
			return Microphone.IsRecording(deviceName);
#elif UNITY_WEBGL
			return FrostweepGames.Plugins.WebGL.Microphone.Instance.IsRecording(deviceName);
#else
			return false;
#endif
		}

		public static void GetDeviceCaps(string deviceName, out int minFreq, out int maxfreq)
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
			Microphone.GetDeviceCaps(deviceName, out minFreq, out maxfreq);
#elif UNITY_WEBGL
			FrostweepGames.Plugins.WebGL.Microphone.Instance.GetDeviceCaps(deviceName, out minFreq, out maxfreq);
#else
			minFreq = 0;
			maxfreq = 0;
#endif
		}

		public static int GetPosition(string deviceName)
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
			return Microphone.GetPosition(deviceName);
#elif UNITY_WEBGL
			return FrostweepGames.Plugins.WebGL.Microphone.Instance.GetPosition(deviceName);
#else
			return 0;
#endif
		}

		public static void End(string deviceName)
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
			Microphone.End(deviceName);
#elif UNITY_WEBGL
			FrostweepGames.Plugins.WebGL.Microphone.Instance.End(deviceName);
#else
			throw new System.NotImplementedException("microphone not implemented yet");
#endif
		}

		public static bool HasConnectedMicrophoneDevices()
		{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_EDITOR || UNITY_WSA
			return Microphone.devices.Length > 0;
#elif UNITY_WEBGL
			return FrostweepGames.Plugins.WebGL.Microphone.Instance.HasConnectedMicrophoneDevices();
#else
			return false;
#endif
		}

		public static void RequestMicrophonePermission()
		{
			if (!HasMicrophonePermission())
			{
#if UNITY_ANDROID
				Permission.RequestUserPermission(Permission.Microphone);
#elif UNITY_WEBGL && !UNITY_EDITOR
				FrostweepGames.Plugins.WebGL.Microphone.Instance.RequestPermission();
#endif
			}
		}

		public static bool HasMicrophonePermission()
		{
#if UNITY_ANDROID
			return Permission.HasUserAuthorizedPermission(Permission.Microphone);
#elif UNITY_WEBGL && !UNITY_EDITOR
			return FrostweepGames.Plugins.WebGL.Microphone.Instance.HasUserAuthorizedPermission();
#else
			return true;
#endif
		}

		public static bool GetRawData(ref float[] output, AudioClip source = null)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			output = FrostweepGames.Plugins.WebGL.Microphone.Instance.GetRawData();
			return true;
#else
			if (source == null)
				return false;

			source.GetData(output, 0);
			return true;
#endif
		}

		public static void RefreshMicrophoneDevices()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			FrostweepGames.Plugins.WebGL.Microphone.Instance.RefreshMicrophoneDevices();
#endif
		}
	}
}