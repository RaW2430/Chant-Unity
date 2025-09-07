mergeInto(LibraryManager.library, {
    audioContext: null,
    analyser: null,
    microphone: null,
    volumeThreshold: 0.02, // 可调节灵敏度
    recognition: null,
    isRecognitionActive: false,

    InitEnglishSpeech: function() {
        console.log("JS：语音识别模块已加载，等待用户点击页面启动...");

        // 浏览器兼容性检查
        if (!navigator.mediaDevices || !window.AudioContext) {
            console.error("JS：浏览器不支持音频功能");
            return;
        }

        // 用户点击事件（激活音频）
        const onUserClick = () => {
            console.log("JS：用户已点击页面，开始激活音频");

            const audioContext = new (window.AudioContext || window.webkitAudioContext)();
            this.audioContext = audioContext;

            const setupAudio = () => {
                const analyser = audioContext.createAnalyser();
                analyser.fftSize = 256;
                this.analyser = analyser;

                navigator.mediaDevices.getUserMedia({ audio: true, video: false })
                    .then(stream => {
                        console.log("JS：麦克风授权成功");
                        const mic = audioContext.createMediaStreamSource(stream);
                        this.microphone = mic;
                        mic.connect(analyser);

                        // 开始音量检测
                        const bufferLength = analyser.frequencyBinCount;
                        const dataArray = new Uint8Array(bufferLength);

                        const checkVolume = () => {
                            analyser.getByteFrequencyData(dataArray);
                            let sum = 0;
                            for (let i = 0; i < bufferLength; i++) sum += dataArray[i];
                            const volume = sum / bufferLength / 255;

                            if (volume > this.volumeThreshold) {
                                // 有声音时发送 success
                                if (typeof window.unityInstance !== "undefined") {
                                    window.unityInstance.SendMessage("SpeechHandler", "OnSpeechResultText1", "success");
                                }
                            }

                            requestAnimationFrame(checkVolume);
                        };
                        checkVolume();

                        // 初始化语音识别（英文）
                        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
                        if (SpeechRecognition) {
                            const recognition = new SpeechRecognition();
                            recognition.lang = "en-US";
                            recognition.continuous = true;
                            recognition.interimResults = false;
                            this.recognition = recognition;

                            recognition.onresult = (event) => {
                                const transcript = event.results[event.results.length - 1][0].transcript;
                                if (typeof window.unityInstance !== "undefined") {
                                    window.unityInstance.SendMessage("SpeechHandler", "OnSpeechResultText2", transcript);
                                }
                            };

                            recognition.onerror = (event) => {
                                console.warn("JS：语音识别错误", event.error);
                            };

                            recognition.onend = () => {
                                if (this.isRecognitionActive) recognition.start();
                            };

                            try {
                                recognition.start();
                                this.isRecognitionActive = true;
                                console.log("JS：语音识别已启动");
                            } catch (err) {
                                console.warn("JS：语音识别启动失败", err);
                            }
                        } else {
                            console.warn("JS：浏览器不支持 SpeechRecognition");
                        }

                    })
                    .catch(err => {
                        console.error("JS：麦克风授权失败：", err);
                        alert("请允许麦克风权限才能使用语音识别！");
                    });
            };

            if (audioContext.state === 'suspended') {
                audioContext.resume().then(setupAudio);
            } else {
                setupAudio();
            }
        };

        // 页面任意点击即可触发一次
        document.addEventListener('click', onUserClick, { once: true });
    },

    SetVolumeThreshold: function(threshold) {
        this.volumeThreshold = threshold;
        console.log(`JS：已更新阈值为 ${threshold}`);
    }
});
