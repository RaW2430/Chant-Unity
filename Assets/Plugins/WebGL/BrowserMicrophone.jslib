mergeInto(LibraryManager.library, {
    RequestMicrophoneAccess: function() {
        if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
            navigator.mediaDevices.getUserMedia({ audio: true })
                .then(stream => { console.log("麦克风权限已授权"); })
                .catch(err => { console.warn("麦克风权限被拒绝", err); });
        } else {
            console.warn("浏览器不支持麦克风");
        }
    }
});
