mergeInto(LibraryManager.library, {
    audioContext: null,
    analyser: null,
    microphone: null,
    volumeThreshold: 0.12,
    attackVolumeThreshold: 0.22,

    // 初始化音量检测（简化上下文处理）
    InitVolumeDetection: function() {
        console.log("JS：开始初始化音量检测");
        console.log(`JS：volumeThreshold=${this.volumeThreshold}`);
        console.log(`JS：attackVolumeThreshold=${this.attackVolumeThreshold}`);
        // 浏览器兼容性检查
        if (!navigator.mediaDevices || !window.AudioContext) {
            console.error("JS：浏览器不支持音频检测");
            return;
        }

        // 定义用户点击处理函数（使用局部函数避免this问题）
        const onUserClick = () => {
            console.log("JS：用户已交互，开始激活音频");
            
            // 创建音频上下文（兼容写法）
            const audioContext = new (window.AudioContext || window.webkitAudioContext)();
            
            // 激活音频上下文
            const setupAudio = () => {
                const analyser = audioContext.createAnalyser();
                analyser.fftSize = 256;
                console.log("JS：分析器初始化完成");

                // 请求麦克风权限
                navigator.mediaDevices.getUserMedia({ audio: true, video: false })
                    .then(stream => {
                        console.log("JS：麦克风授权成功");
                        const microphone = audioContext.createMediaStreamSource(stream);
                        microphone.connect(analyser);
                        
                        // 开始检测音量（使用闭包保存上下文）
                        const bufferLength = analyser.frequencyBinCount;
                        const dataArray = new Uint8Array(bufferLength);
                        
                        const checkVolume = () => {
                            analyser.getByteFrequencyData(dataArray);
                            
                            let sum = 0;
                            for (let i = 0; i < bufferLength; i++) {
                                sum += dataArray[i];
                            }
                            const volume = (sum / bufferLength) / 255;
                            // console.log(`JS：当前音量=${volume.toFixed(2)}，阈值=${this.volumeThreshold}`);
                            
                            if (volume > this.volumeThreshold) {
                                console.log("JS：音量=${volume.toFixed(2)}超过阈值，触发跳跃");
                                SendMessage("Player", "TriggerJumpBySound", "");
                            }

                            if (volume > this.attackVolumeThreshold) {
                                console.log("JS：音量=${volume.toFixed(2)}超过阈值，触发攻击");
                                SendMessage("Player", "TriggerAttackBySound", "");
                            }
                            
                            requestAnimationFrame(checkVolume);
                        };
                        
                        checkVolume();
                    })
                    .catch(err => {
                        console.error("JS：麦克风授权失败：", err);
                        alert("请允许麦克风权限才能使用语音控制！");
                    });
            };

            // 处理音频上下文状态
            if (audioContext.state === 'suspended') {
                audioContext.resume().then(setupAudio).catch(err => {
                    console.error("JS：激活音频上下文失败：", err);
                });
            } else {
                setupAudio();
            }
        };

        // 绑定点击事件（直接使用局部函数，避免this问题）
        console.log("JS：请点击页面授权麦克风（浏览器安全要求）");
        document.addEventListener('click', onUserClick, { once: true });
    },

    // 设置阈值
    SetVolumeThreshold: function(threshold) {
        this.volumeThreshold = threshold;
        console.log(`JS：jump 已更新阈值为 ${threshold}`);
    },

    SetAttackVolumeThreshold: function(threshold) {
        this.attackVolumeThreshold = threshold;
        console.log(`JS：attack 已更新阈值为 ${threshold}`);
    }
});
