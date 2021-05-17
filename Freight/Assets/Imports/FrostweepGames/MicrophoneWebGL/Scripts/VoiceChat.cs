using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace VoiceChatClass
{
    public class VoiceChat : UnityEngine.MonoBehaviour
    {
#region __Internal
        [DllImport("__Internal")]
        private static extern void setupPeer();

        [DllImport("__Internal")]
        private static extern void startConnection(string foreignID);

        [DllImport("__Internal")]
        private static extern void getId();

        [DllImport("__Internal")]
        private static extern void startCall(string foreignID);

        [DllImport("__Internal")]
        private static extern void endCall();

        [DllImport("__Internal")]
        private static extern void setVolume(float newVolume);

        [DllImport("__Internal")]
        private static extern void endConnection();
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

        public string Status
        {
            get
            {
                return _status;
            }
        }

        private string _status;
        private string _peerID;
        private string _foreignID;
        private bool dataConnection;
        private bool mediaConnection;
        private CultureInfo _provider;

        // Called when the peer receives an id
        public Action<string> OnIDUpdate;
        // Called when the status is updated
        public Action<string> OnStatusUpdate;
        public Action<float[]> OnDataReceive;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _provider = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _provider.NumberFormat.NumberDecimalSeparator = ".";

            _status = "disconnected";
            _peerID = "";
            _foreignID = "";
            dataConnection = false;
            mediaConnection = false;
        }

        private void OnDestroy()
        {
            Dispose();
        }


        /// <summary>
        /// Create a Peer object for this client.
        /// </summary>
        public void InitializePeer()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if(_status != "connected")
            {
                setupPeer();
            }
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

        public void Connect(string foreignID)
        {
            _foreignID = foreignID;
            dataConnection = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            if (_peerID != "")
            {
                startConnection(foreignID);
            }
#endif
        }

        public void Call(string foreignID)
        {
            _foreignID = foreignID;
            mediaConnection = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            if (_peerID != "")
            {
                startCall(foreignID);
            }
#endif
        }

        public void EndCall()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if(_peerID != "" && _foreignID != "" && mediaConnection)
            {
                endCall();
                _foreignID = "";
            }
#endif
            mediaConnection = false;
        }

        public void SetVolumeOfCall(float newVolume)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            setVolume(newVolume);
#endif
        }

        public void Disconnect()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (_peerID != "" && _foreignID != "" && dataConnection)
            {
                endConnection();
                _foreignID = "";
            }
#endif
            dataConnection = false;
        }

        /// <summary>
        /// Cleanups service
        /// </summary>
        public void Dispose()
        {
            _Instance = null;
            EndCall();
            Disconnect();
            _status = "disconnected";
            _peerID = "";
            _foreignID = "";
            dataConnection = false;
            mediaConnection = false;
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
            OnDataReceive?.Invoke(array);
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

#if UNITY_WEBGL && !UNITY_EDITOR
            if(_foreignID != "")
            {
                if(dataConnection)
                    startConnection(_foreignID);

                if(mediaConnection)
                    startCall(_foreignID);
            }
#endif
        }
    }
}
