mergeInto(LibraryManager.library, {

    getMicrophoneDevices: function() {
        if (document.microphoneDevices == undefined)
            document.microphoneDevices = new Array();

        if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
            console.log("enumerateDevices() not supported.");

            SendMessage('[FG]Microphone', 'SetMicrophoneDevices', JSON.stringify(document.microphoneDevices));
            return;
        }

        navigator.mediaDevices.enumerateDevices()
            .then(function(devices) {

                var outputDevicesArr = new Array();

                for (var i = 0; i < devices.length; i++) {
                    if (devices[i].kind === "audioinput") {
                        var deviceInfo = {
                            deviceId: devices[i].deviceId,
                            kind: devices[i].kind,
                            label: devices[i].label,
                            groupId: devices[i].groupId,
                            isGrantedAccess: true
                        };

                        if (deviceInfo.label === undefined || deviceInfo.label === null || deviceInfo.label.length === 0) {
                            deviceInfo.label = "Microphone " + (outputDevicesArr.length + 1);
                            deviceInfo.isGrantedAccess = false;
                        }

                        outputDevicesArr.push(deviceInfo);
                    }
                }

                document.microphoneDevices = outputDevicesArr;

                SendMessage('[FG]Microphone', 'SetMicrophoneDevices', JSON.stringify(document.microphoneDevices));
            })
            .catch(function(err) {
                console.log("get devices exception: " + err.name + ": " + err.message + "; " + err.stack);
            });
    },

    init: function() {
        var AudioContext = window.AudioContext || window.webkitAudioContext;
        document.audioContext = new AudioContext();

        setInterval(function(){
            if (document.audioContext.state === "suspended" || document.audioContext.state === "interrupted") {
                console.log("resuming audioContext. state: " + document.audioContext.state);
                document.audioContext.resume();
            }
        }, 500);
    },

    start: function(device, loop, length, frequency) {

        var deviceId = null;

        if (device != null) {
            deviceId = Pointer_stringify(device);
        }

        document.microphoneFrequency = frequency;

        function begin() {
            if (navigator.mediaDevices.getUserMedia) {
                var constraints = null;

                if (deviceId == null) {
                    constraints = {
                        audio: true
                    };
                } else {

					var deviceInfo = {
						exact: deviceId
					};
					
					var audioInfo = {
						deviceId: deviceInfo
					};

                    constraints = {
                        audio: audioInfo
                    };
                }

                navigator.mediaDevices.getUserMedia(constraints).then(GetUserMediaSuccess).catch(GetUserMediaFailed);
            }
        }

		begin();

        function GetUserMediaSuccess(stream) {
            document.microphone_stream = document.audioContext.createMediaStreamSource(stream);
            document.script_processor_node = document.audioContext.createScriptProcessor(0, 1, 1);
            document.script_processor_node.onaudioprocess = MicrophoneProcess;

            document.script_processor_node.connect(document.audioContext.destination);
            document.microphone_stream.connect(document.script_processor_node);

            document.isRecording = 1;

            console.log('record started');
        }

        function GetUserMediaFailed(error) {
            console.log('GetUserMedia failed with error ' + error);
        }

        function MicrophoneProcess(event) {
           if (event.inputBuffer.sampleRate === document.microphoneFrequency) {
                var leftFloat32Array = event.inputBuffer.getChannelData(0);
                var stringArray = "";

                for (var i = 0; i < leftFloat32Array.length; i++) {
                    stringArray = stringArray + leftFloat32Array[i];
                    if (i < leftFloat32Array.length - 1) {
                        stringArray = stringArray + ",";
                    }
                }

                SendMessage('[FG]Microphone', 'WriteBufferFromMicrophoneHandler', stringArray);
            } else {
                Resample(event.inputBuffer, document.microphoneFrequency);  
            }
        }

        function Resample(sourceAudioBuffer, TARGET_SAMPLE_RATE) {
            var offlineCtx = new OfflineAudioContext(sourceAudioBuffer.numberOfChannels, sourceAudioBuffer.duration * sourceAudioBuffer.numberOfChannels * TARGET_SAMPLE_RATE, TARGET_SAMPLE_RATE);
            var buffer = offlineCtx.createBuffer(sourceAudioBuffer.numberOfChannels, sourceAudioBuffer.length, sourceAudioBuffer.sampleRate);
            // Copy the source data into the offline AudioBuffer
            for (var channel = 0; channel < sourceAudioBuffer.numberOfChannels; channel++) {
                buffer.copyToChannel(sourceAudioBuffer.getChannelData(channel), channel);
            }
            // Play it from the beginning.
            var source = offlineCtx.createBufferSource();
            source.buffer = sourceAudioBuffer;
            source.connect(offlineCtx.destination);
            source.start(0);
            offlineCtx.oncomplete = function(e) {
                // `resampled` contains an AudioBuffer resampled at 16000Hz.
                // use resampled.getChannelData(x) to get an Float32Array for channel x.
                var resampled = e.renderedBuffer;
                var leftFloat32Array = resampled.getChannelData(0);
                // use this float32array to send the samples to the server or whatever
                var stringArray = "";

                for (var i = 0; i < leftFloat32Array.length; i++) {
                    stringArray = stringArray + leftFloat32Array[i];
                    if (i < leftFloat32Array.length - 1) {
                        stringArray = stringArray + ",";
                    }
                }

                SendMessage('[FG]Microphone', 'WriteBufferFromMicrophoneHandler', stringArray);
            }
            offlineCtx.startRendering();
        }
    },

    end: function(device) {
        if (document.microphone_stream != undefined) {
            document.microphone_stream.disconnect(document.script_processor_node);
            document.script_processor_node.disconnect();
        }

        document.microphone_stream = null;
        document.script_processor_node = null;

        document.isRecording = 0;

        console.log('record ended');
    },

    isRecording: function(device) {
        if (document.isRecording == undefined)
            document.isRecording = 0;
        return document.isRecording;
    },

    getDeviceCaps: function(device) {

        function IsSafari() {
            return navigator.userAgent.indexOf("Safari") > -1;
        }

        var array = new Array();

        array.push(IsSafari() ? 44100 : 16000);
        array.push(48000);

        var returnStr = JSON.stringify(array);
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    isAvailable: function() {
        return !!(navigator.mediaDevices.getUserMedia);
    },

    requestPermission: function() {
        if (navigator.mediaDevices.getUserMedia) {
			navigator.mediaDevices.getUserMedia({ audio: true }).then(GetUserMediaSuccess).catch(GetUserMediaFailed);

            function GetUserMediaSuccess(stream) {
                SendMessage('[FG]Microphone', 'PermissionUpdate', "granted");
            }

            function GetUserMediaFailed(error) {
                SendMessage('[FG]Microphone', 'PermissionUpdate', "denined");
            }
        }
    },

    hasUserAuthorizedPermission: function() {

        if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices || document.microphoneDevices == undefined) {
            SendMessage('[FG]Microphone', 'PermissionUpdate', "denined");
            return;
        }

        navigator.mediaDevices.enumerateDevices()
            .then(function(devices) {

                var outputDevicesArr = new Array();

                for (var i = 0; i < devices.length; i++) {
                    if (devices[i].kind === "audioinput") {
                        var deviceInfo = {
                            deviceId: devices[i].deviceId,
                            kind: devices[i].kind,
                            label: devices[i].label,
                            groupId: devices[i].groupId,
                            isGrantedAccess: true
                        };

                        if (deviceInfo.label === undefined || deviceInfo.label === null || deviceInfo.label.length === 0) {
                            deviceInfo.label = "Microphone " + (outputDevicesArr.length + 1);
                            deviceInfo.isGrantedAccess = false;
                        }

                        outputDevicesArr.push(deviceInfo);
                    }
                }

                document.microphoneDevices = outputDevicesArr;

                if (outputDevicesArr.length > 0 && outputDevicesArr[0].isGrantedAccess === true)
                    SendMessage('[FG]Microphone', 'PermissionUpdate', "granted");
                else
                    SendMessage('[FG]Microphone', 'PermissionUpdate', "denined");
            })
            .catch(function(err) {
                SendMessage('[FG]Microphone', 'PermissionUpdate', "denined");
            });
    }
});