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
    })
    .catch(function(err) {
        console.log("get devices exception: " + err.name + ": " + err.message + "; " + err.stack);
    });