using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.WebGL
{
#if UNITY_WEBGL
    public class Microphone : UnityEngine.MonoBehaviour
    {
        #region __Internal
        [DllImport("__Internal")]
        private static extern void init();
        [DllImport("__Internal")]
        private static extern void getMicrophoneDevices();
        [DllImport("__Internal")]
        private static extern void start(string device, int loop, int length, int frequency);
        [DllImport("__Internal")]
        private static extern void end(string device);
        [DllImport("__Internal")]
        private static extern int isRecording(string device);
        [DllImport("__Internal")]
        private static extern string getDeviceCaps(string device);
        [DllImport("__Internal")]
        private static extern int isAvailable();
        [DllImport("__Internal")]
        private static extern void requestPermission();
        [DllImport("__Internal")]
        private static extern int hasUserAuthorizedPermission();

        #endregion

        private const char SEPARATOR = ',';

        private static Microphone _Instance;
        public static Microphone Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new UnityEngine.GameObject("[FG]Microphone").AddComponent<Microphone>(); // Object should be named as '[FG]Microphone'.

                return _Instance;
            }
        }

        private UnityEngine.AudioClip _microphoneClip;
        private bool _loopRecording;
        private int _frequency;
        private float[] _audioDataArray;
        private CultureInfo _provider;
        private string[] _microphoneDevices;
        private bool _permissionGranted;
        private int _samplePosition;
        private List<MicrophoneDeviceInfo> _microphoneDeviceInfos;

        private void Awake()
        {
            _provider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _provider.NumberFormat.NumberDecimalSeparator = ".";
            _microphoneDevices = new string[0];
            _microphoneDeviceInfos = new List<MicrophoneDeviceInfo>();

#if UNITY_WEBGL && !UNITY_EDITOR
            init();

            getMicrophoneDevices();
            hasUserAuthorizedPermission();
#endif
        }

        private void Start() { }

        private void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// Start Recording with device.
        /// </summary>
        /// <param name="deviceName">The name of the device. (not uses)</param>
        /// <param name="loop">Indicates whether the recording should continue recording if lengthSec is reached, and wrap around and record from the beginning of the AudioClip.</param>
        /// <param name="lengthSec">Is the length of the AudioClip produced by the recording.</param>
        /// <param name="frequency">The sample rate of the AudioClip produced by the recording.</param>
        /// <returns>The function returns null if the recording fails to start.</returns>
        public UnityEngine.AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency)
        {
            if (IsRecording(deviceName))
                return _microphoneClip;

            if(lengthSec > 120)
			{
                UnityEngine.Debug.LogWarning("TOO LONG AUDIO LENGTH! IT WILL CAUSE ISSUES! WAS RESET LENGTH TO 120 seconds");
                lengthSec = 120; // hardfix for long audio clips of Unity Engine.
            }

            Cleanup();

            _frequency = frequency;
            _loopRecording = loop;
            _microphoneClip = UnityEngine.AudioClip.Create("Microphone", _frequency * lengthSec, 1, _frequency, false);
            _audioDataArray = new float[_frequency * lengthSec];

            string deviceId = _microphoneDeviceInfos.Find(item => item.label == deviceName)?.deviceId;

            start(deviceId, loop ? 1 : 0, lengthSec, _frequency);

            return _microphoneClip;
        }

        /// <summary>
        /// Query if a device is currently recording.
        /// </summary>
        /// <param name="deviceName">The name of the device. (not uses)</param>
        /// <returns></returns>
        public bool IsRecording(string deviceName)
        {
            return isRecording(deviceName) == 1 ? true : false;
        }

        public void GetDeviceCaps(string deviceName, out int minFreq, out int maxfreq)
        {
            int[] array = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(getDeviceCaps(deviceName));

            minFreq = array[0];
            maxfreq = array[1];
        }

        /// <summary>
        /// Get the position in samples of the recording. 
        /// </summary>
        /// <param name="deviceName">The name of the device (not uses)</param>
        /// <returns></returns>
        public int GetPosition(string deviceName)
        {
            return _samplePosition;
        }

        /// <summary>
        /// Stops recording.
        /// </summary>
        /// <param name="deviceName">The name of the device. (not uses)</param>
        public void End(string deviceName)
        {
            if (!IsRecording(deviceName) || !HasConnectedMicrophoneDevices())
                return;

            end(deviceName);
        }

        public bool HasConnectedMicrophoneDevices()
        {
            return GetMicrophoneDevices().Length > 0;
        }

        /// <summary>
        /// Returns a list of available microphone devices, identified by name
        /// </summary>
        /// <returns></returns>
        public string[] GetMicrophoneDevices()
        {
            return _microphoneDevices;
        }

        /// <summary>
        /// Requests permission for using media devices
        /// </summary>
        public void RequestPermission()
        {
            requestPermission();
        }

        public bool HasUserAuthorizedPermission()
        {
            hasUserAuthorizedPermission();
            return _permissionGranted;
        }

        public void RefreshMicrophoneDevices()
        {
            getMicrophoneDevices();
        }

        /// <summary>
        /// Returns RAW data (samples array) of an AudioClip; This is the full array of samples that could be not filled fully by audio stream.
        /// </summary>
        /// <returns></returns>
        public float[] GetRawData()
        {
            return _audioDataArray;
        }

        /// <summary>
        /// Cleanups service
        /// </summary>
        public void Dispose()
        {
            _Instance = null;
            Cleanup();
        }

        private void Cleanup()
        {
            if (_microphoneClip != null)
                Destroy(_microphoneClip);
            _audioDataArray = null;
            _samplePosition = 0;
        }

        private float[] StringToFloatArray(string value)
        {
            return value.Split(SEPARATOR).Select(f => Convert.ToSingle(f, _provider)).ToArray();
        }

        /// <summary>
        /// Event handler from JS library
        /// </summary>
        /// <param name="json"></param>
        private void SetMicrophoneDevices(string json)
        {
            _microphoneDeviceInfos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MicrophoneDeviceInfo>>(json);

            _microphoneDevices = new string[_microphoneDeviceInfos.Count];

            for (int i = 0; i < _microphoneDeviceInfos.Count; i++)
            {
                _microphoneDevices[i] = _microphoneDeviceInfos[i].label;
            }
        }

        /// Event handler from JS library
        /// </summary>
        /// <param name="state"></param>
        private void PermissionUpdate(string state)
        {
            _permissionGranted = state == "granted" ? true : false;
        }

        /// Event handler from JS library
        /// </summary>
        /// <param name="data"></param>
        private void WriteBufferFromMicrophoneHandler(string data)
        {
            float[] array = StringToFloatArray(data);

            for (int i = 0; i < array.Length; i++)
            {
                if (!_loopRecording && _samplePosition == _audioDataArray.Length)
                    break;

                _audioDataArray[_samplePosition] = array[i];

                if (_loopRecording)
                {
                    _samplePosition = (int)UnityEngine.Mathf.Repeat(_samplePosition + 1, _audioDataArray.Length);
                }
                else
                {
                    _samplePosition++;
                }
            }

            if (_microphoneClip != null && _microphoneClip)
            {
                _microphoneClip.SetData(_audioDataArray, 0);
            }
        }


        private class MicrophoneDeviceInfo
        {
            public string deviceId { get; set; }
            public string kind { get; set; }
            public string label { get; set; }
            public string groupId { get; set; }

#if UNITY_2018_4_OR_NEWER
            [UnityEngine.Scripting.Preserve]
#endif
            public MicrophoneDeviceInfo()
            {
            }
#if UNITY_2018_4_OR_NEWER
            [UnityEngine.Scripting.Preserve]
#endif
            public MicrophoneDeviceInfo(string deviceId, string kind, string label, string groupId)
            {
                this.deviceId = deviceId;
                this.kind = kind;
                this.label = label;
                this.groupId = groupId;
            }
        }
    }
#endif
}