using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.Native;
using System.Collections.Generic;
using System.Linq;

namespace FrostweepGames.UniversalMicrophoneLibrary.Examples
{
    [RequireComponent(typeof(AudioSource))]
    public class Example : MonoBehaviour
    {
        private AudioClip _workingClip;

        public Text permissionStatusText;

        public Text recordingStatusText;

        public Dropdown devicesDropdown;

        public AudioSource audioSource;

        public Button startRecordButton,
                      stopRecordButton,
                      playRecordedAudioButton,
                      requestPermissionButton,
                      refreshDevicesButton;

        public List<AudioClip> recordedClips;

        public int frequency = 44100;

        [Range(0, 120)] // 120 sec is max to prevent AudioClip issues in Unity on WebGL
        public int recordingTime = 10; // only 10 seconds

        public string selectedDevice;

        public bool makeCopy = false;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            startRecordButton.onClick.AddListener(StartRecord);
            stopRecordButton.onClick.AddListener(StopRecord);
            playRecordedAudioButton.onClick.AddListener(PlayRecordedAudio);
            requestPermissionButton.onClick.AddListener(RequestPermission);
            refreshDevicesButton.onClick.AddListener(RefreshMicrophoneDevicesButtonOnclickHandler);

            devicesDropdown.onValueChanged.AddListener(DevicesDropdownValueChangedHandler);

            RefreshMicrophoneDevicesButtonOnclickHandler();
        }

		private void Update()
		{
            permissionStatusText.text = string.Format("Microphone permission: {1} for '{0}'", selectedDevice,
                CustomMicrophone.HasMicrophonePermission() ? "<color=green>granted</color>" : "<color=red>denined</color>");

            if (CustomMicrophone.devices.Length > 0)
            {
                recordingStatusText.text = string.Format("Microphone status: {0}",
                    CustomMicrophone.IsRecording(selectedDevice) ? "<color=green>recording</color>" : "<color=yellow>idle</color>");
            }
        }

        private void RefreshMicrophoneDevicesButtonOnclickHandler()
		{
            RequestPermission();
            CustomMicrophone.RefreshMicrophoneDevices();

            if (!CustomMicrophone.HasConnectedMicrophoneDevices())
                return;

            devicesDropdown.ClearOptions();
            devicesDropdown.AddOptions(CustomMicrophone.devices.ToList());
            DevicesDropdownValueChangedHandler(0);
        }

        private void RequestPermission()
        {
            CustomMicrophone.RequestMicrophonePermission();
        }

        private void StartRecord()
        {
            if (!CustomMicrophone.HasConnectedMicrophoneDevices())
                return;

            _workingClip = CustomMicrophone.Start(selectedDevice, false, recordingTime, frequency);
        }

        private void StopRecord()
        {
            if (!CustomMicrophone.HasConnectedMicrophoneDevices())
                return;

            if (!CustomMicrophone.IsRecording(selectedDevice))
                return;

            CustomMicrophone.End(selectedDevice);

            if (makeCopy)
            {
                recordedClips.Add(MakeCopy($"copy{recordedClips.Count}", recordingTime, frequency, _workingClip));
                audioSource.clip = recordedClips.Last();
            }
			else
			{
                audioSource.clip = _workingClip;
            }
     
            audioSource.Play();
        }

        private void PlayRecordedAudio()
        {
            if (_workingClip == null)
                return;

            audioSource.clip = _workingClip;
            audioSource.Play();
        }

        private AudioClip MakeCopy(string name, int recordingTime, int frequency, AudioClip sourceClip)
		{
            float[] array = new float[recordingTime * frequency];
            if (CustomMicrophone.GetRawData(ref array, sourceClip))
            {
                AudioClip audioClip = AudioClip.Create(name, recordingTime * frequency, 1, frequency, false);
                audioClip.SetData(array, 0);
                audioClip.LoadAudioData();

                return audioClip;
            }

            return null;
        }

        private void DevicesDropdownValueChangedHandler(int index)
		{
            if (index < CustomMicrophone.devices.Length)
            {
                selectedDevice = CustomMicrophone.devices[index];
            }
        }
    }
}