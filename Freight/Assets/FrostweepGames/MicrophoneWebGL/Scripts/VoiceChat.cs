using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace VoiceChatClass
{
//#if UNITY_WEBGL
    public class VoiceChat : UnityEngine.MonoBehaviour
    {
#region __Internal
        [DllImport("__Internal")]
        private static extern void setupPeer();

        [DllImport("__Internal")]
        private static extern void startConnection(string foreignID);

        [DllImport("__Internal")]
        private static extern void getId();

#endregion

        private const char SEPARATOR = ',';

        private static VoiceChat _Instance;
        public static VoiceChat Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new UnityEngine.GameObject("[PeerJS]VoiceChat").AddComponent<VoiceChat>(); // Object should be named as '[PeerJS]VoiceChat'.

                return _Instance;
            }
        }

        private UnityEngine.AudioClip _peerClip;
        private bool _loopRecording = true;
        private string _status = "disconnected";
        private string _peerID = "";
        private string _foreignID = "";
        private float[] _audioDataArray;
        private int _samplePosition;
        private CultureInfo _provider;

        // Called when the peer receives an id
        public Action<string> OnIDUpdate;
        // Called when the status is updated
        public Action<string> OnStatusUpdate;

        private void Awake()
        {
            _provider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _provider.NumberFormat.NumberDecimalSeparator = ".";

        }

        private void Start() { }

        private void OnDestroy()
        {
            Dispose();
        }


        /// <summary>
        /// Create a Peer object for this client.
        /// </summary>
        public void InitialisePeer()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            setupPeer();
#endif
        }

        public void GetPeerId()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if(_peerID != "")
            {
                OnIDUpdate?.Invoke(_peerID);
            }
            else
            {
                getId();
            }
#endif
        }

        /// <summary>
        /// Start a connection.
        /// </summary>
        /// <param name="foreignID">Is the ID of the peer you want to connect to.</param>
        /// <param name="lengthSec">Is the length of the AudioClip produced by the recording.</param>
        /// <param name="frequency">The sample rate of the AudioClip produced by the recording.</param>
        /// <returns>The function returns null if the recording fails to start.</returns>
        public UnityEngine.AudioClip GetClip(int lengthSec, int frequency)
        {

            if(lengthSec > 120)
			{
                UnityEngine.Debug.LogWarning("TOO LONG AUDIO LENGTH! IT WILL CAUSE ISSUES! WAS RESET LENGTH TO 120 seconds");
                lengthSec = 120; // hardfix for long audio clips of Unity Engine.
            }

            Cleanup();

            _peerClip = UnityEngine.AudioClip.Create("PeerClip", frequency * lengthSec, 1, frequency, false);
            _audioDataArray = new float[frequency * lengthSec];

            return _peerClip;
        }

        public void Connect(string foreignID)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            _foreignID = foreignID;
            if(_peerID != "")
            {
                startConnection(foreignID);
            }
#endif
        }

        public void Disconnect()
        {
            _foreignID = "";
        }
//#endif

        /// <summary>
        /// Get the position in samples of the recording. 
        /// </summary>
        /// <param name="deviceName">The name of the device (not uses)</param>
        /// <returns></returns>
        public int GetPosition()
        {
            return _samplePosition;
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
            if (_peerClip != null)
                Destroy(_peerClip);
            _audioDataArray = null;
            _samplePosition = 0;
        }

        private float[] StringToFloatArray(string value)
        {
            return value.Split(SEPARATOR).Select(f => Convert.ToSingle(f, _provider)).ToArray();
        }

        /// Event handler from JS library
        /// </summary>
        /// <param name="data"></param>
        private void WriteBufferFromMessageHandler(string data)
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

            if (_peerClip != null && _peerClip)
            {
                _peerClip.SetData(_audioDataArray, 0);
            }
        }

        /// Event handler from JS library
        /// </summary>
        /// <param name="state"></param>
        private void StatusUpdate(string status)
        {
            _status = status;

            if (status == "destroyed")
            {
                // try to create a new peer 
                setupPeer();
            }

            OnStatusUpdate?.Invoke(_status);
        }

        /// Event handler from JS library
        /// </summary>
        /// <param name="state"></param>
        private void ReceivePeerIDHandler(string ID)
        {
            _peerID = ID;
            OnIDUpdate?.Invoke(_peerID);

            if(_foreignID != "")
            {
                startConnection(_foreignID);
            }
        }

    }

}
