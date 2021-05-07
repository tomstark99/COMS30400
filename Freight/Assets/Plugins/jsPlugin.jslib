mergeInto(LibraryManager.library, {

  StringReturnValueFunction: function () {
    var returnStr = getPoseAsString();
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  
    GetGestureAsString: function () {
      var returnStr = getGestureAsString();
      var bufferSize = lengthBytesUTF8(returnStr) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(returnStr, buffer, bufferSize);
      return buffer;
    },
    
    LoadOverlayPlugin: function (relativePath) {
        loadOverlay(Pointer_stringify(relativePath));
    },
    
    ClearOverlayPlugin: function () {
        clearOverlay();
    },
    
    TurnOffPose: function () {
        turnOffPose();
    }
});

