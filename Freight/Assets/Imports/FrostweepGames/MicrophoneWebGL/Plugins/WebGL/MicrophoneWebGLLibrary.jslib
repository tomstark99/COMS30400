mergeInto(LibraryManager.library, {

    getMicrophoneDevices: function() {
        if (document.microphoneDevices == undefined)
            document.microphoneDevices = new Array();
        else {
            SendMessage('[FG]Microphone', 'SetMicrophoneDevices', JSON.stringify(document.microphoneDevices));
            return;
        }
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


        function setupConnection() {
            if(document.connection == null) {
                console.log("Setup Connection called without a connection existing");
                return;
            }
            
            console.log('Setting up a new connection');
            // on open will be launch when you successfully connect to the other Peer
            document.connection.on('open', function() {
                console.log('Connected to receiver');
                document.connected = true;
    
                SendMessage('[PeerJS]VoiceChat', 'StatusUpdate', "connected");
            });
    
            document.connection.on('data', function(data) {
                console.log('Received data');
                SendMessage('[PeerJS]VoiceChat', 'WriteBufferFromMessageHandler', data);
            });
    
            document.connection.on('error', function(err) {
                // doesn't have documentation on what it returns so just mark the connection as invalid
                console.log("create connection exception: " + err.name + ": " + err.message + "; " + err.stack);
                document.connection.close();
                document.connected = false;
                document.connection = null;
                SendMessage('[PeerJS]VoiceChat', 'StatusUpdate', "disconnected");
            });
        }

        function setupCall() {
            if(document.call == null) {
                console.log("Setup Call called without a call existing");
                return;
            }
    
            // Receive data
            document.call.on('stream', function (stream) {
                console.log('Connected to call');
                document.connected = true;
                // Store a global reference of the other user stream
                document.peer_stream = stream;
                // Display the stream of the other user in the peer-audio element !
                var audio = document.getElementById('peer-audio');
                // Set the given stream as the audio source 
                audio.srcObject = stream;

                SendMessage('[PeerJS]VoiceChat', 'StatusUpdate', "connected");
            });

            document.call.on('error', function(err) {
                console.log("create call exception: " + err.name + ": " + err.message + "; " + err.stack);
                document.call.close();
                document.call = false;
                document.call= null;

                SendMessage('[PeerJS]VoiceChat', 'StatusUpdate', "disconnected");
            });
        }

        // Initialise PeerJS variables
        document.peer = null;
        document.connected = false;
        document.hasId = false
        document.connection = null;
        document.call = null;
        document.setupConnection = setupConnection;
        document.setupCall = setupCall;
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
                // navigator.mediaDevices.getUserMedia({audio: true}).then(GetUserMediaSuccess).catch(GetUserMediaFailed);
            }
        }

		begin();

        function GetUserMediaSuccess(stream) {
            document.localStream = stream;
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
            navigator.mediaDevices.getUserMedia({audio: true}).then(GetUserMediaSuccess).catch();
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

                // if we have a connection, also send the microphone buffer to the other connection
                // if(document.connected) {
                //     document.connection.send(stringArray);
                // }
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

                // if we have a connection, also send the microphone buffer to the other connection
                // if(document.connected) {
                //     document.connection.send(stringArray);
                // }
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

    setupPeer: function() {
        if(document.peer != null) {
            document.peer.destroy();
            document.peer = null;
        }
        console.log('Creating a new peer');
        // create new Peer object and return it's ID
        // document.peer = new Peer(null, {secure: true, debug: 2}); 
        document.peer = new Peer(null, {debug: 2}); 

        document.peer.on('open', function (id) {
            // return the id of the Peer as a string
            document.hasId = true
            console.log("received peer id: " + id);
            SendMessage('[PeerJS]VoiceChat', 'ReceivePeerIDHandler', id);
        });

        document.peer.on('close', function () {
            console.log('closing peer');
            document.peer.destroy();
            document.peer = null;
            document.connection = null;
            document.connected = false;
            document.hasId = false;
            SendMessage('[PeerJS]VoiceChat', 'StatusUpdate', "destroyed");
        });

        document.peer.on('disconnected', function () {
            console.log("Disconnected from the server");
            document.peer.reconnect();
        });

        document.peer.on('error', function (err) {
            console.log("peer error exception: " + err.name + ": " + err.message + "; " + err.stack);
            document.peer = null;
            document.connection = null;
            document.connected = false;
            document.hasId = false;
            SendMessage('[PeerJS]VoiceChat', 'StatusUpdate', "destroyed");
        });

        // called when a remote Peer tries to connect to you
        document.peer.on('connection', function(conn) {
            console.log("getting called by another peer");
            if(document.connection != null) {
                document.connection.close();
                document.connection = null;
            }

            document.connection = conn;
            document.setupConnection();
        });

        document.peer.on('call', function (call) {
            document.call = call;
            document.call.answer(document.localStream);
    
            document.setupCall();
        });
    },

    startConnection: function(receiverIdPointer) {
        if(document.peer == null){
            console.log("Start Connection called without a Peer existing");
            return;
        }

        receiverId = Pointer_stringify(receiverIdPointer);

        if(document.connection != null) {
            document.connection.close();
            document.connection = null;
        }

        if(document.hasId) {
            console.log("calling peer with id: " + receiverId);
        
            // document.connection = document.peer.connect(receiverId, {reliable:true});
            document.connection = document.peer.connect(receiverId);
            document.setupConnection();
        } else {
            console.log('wait for own id');
            document.peer.on('open', function (id) {
                console.log("calling peer with id: " + receiverId);
        
                // document.connection = document.peer.connect(receiverId, {reliable:true});
                document.connection = document.peer.connect(receiverId);
                document.setupConnection();
            });
        }
    },

    endConnection: function() {
        console.log("ending connection");
        if(document.connection != null) {
            document.connection.close();
        }
        document.connection = null;
        document.connected = false;
    },

    startCall: function(receiverIdPointer) {
        if(document.peer == null){
            console.log("Start Connection called without a Peer existing");
            return;
        }

        receiverId = Pointer_stringify(receiverIdPointer);

        if(document.call != null) {
            document.call.close();
            document.call = null;
        }

        if(document.hasId) {
            console.log("calling peer with id: " + receiverId);
        
            document.call = document.peer.call(receiverId, document.localStream);
            document.setupCall();
        } else {
            console.log('wait for own id');
            document.peer.on('open', function (id) {
                console.log("calling peer with id: " + receiverId);
        
                document.call = document.peer.call(receiverId, document.localStream);
                document.setupCall();
            });
        }
    },

    endCall: function() {
        console.log("ending call");
        if(document.call != null) {
            document.call.close();
        }
        document.call = null;
        document.connected = false;
    },

    setVolume: function(newVolume) {
        var audio = document.getElementById('peer-audio');
        // Set the given stream as the audio source 
        audio.volume = newVolume;
    },

    getId: function() {
        if(document.hasId) {
            SendMessage('[PeerJS]VoiceChat', 'ReceivePeerIDHandler', document.peer.id);
        } else {
            document.peer.on('open', function (id) {
                SendMessage('[PeerJS]VoiceChat', 'ReceivePeerIDHandler', id);
            });
        }
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