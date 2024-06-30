'use strict'
import { QrcodeDecoder } from "./qrcode.js";

let Scan = {
    qr: null,
    video: null,
    ui: null,
    stop() {
        console.log("退出扫描");
        try {
            if (this.ui) {
                try {
                    this.ui.classList.remove("scanning");
                } catch { }
            }
            if (this.video!=null && this.video.srcObject) {
                let tracks = this.video.srcObject.getTracks();
                tracks.forEach(function (track) {
                    track.stop();
                });
            }

            try {
                this.qr.stop();
            } catch (e) {

            }
            try {
                this.video.pause();
            } catch (e) {

            }

        } catch (e) {
            console.error("退出出错", e);
        }


    },
    init(razorPage) {

        this.ui = document.querySelector(".ui-scanner");
        this.qr = new QrcodeDecoder();
        this.video = document.getElementById('scan_video');

        let constraints = {
            video: {
                facingMode: { exact: "environment" }
            },
            audio: false
        };
        console.log(this.video);

        var that = this;
        navigator.mediaDevices.getUserMedia(constraints).then((stream) => {
            console.log("stream", stream);

            //注意这里要用静态变量保存流
            try {
                that.video.srcObject = stream;
                that.video.play();
            } catch (e) { }
            try {
                console.log(that.ui.innerHTML);
                that.ui.classList.add("scanning");
            } catch { }
            that.qr.decodeFromVideo(that.video).then(t => {
                that.stop();
                razorPage.invokeMethodAsync("ProcessScan", t.data);

            });


        }).catch((err) => {
            fair.ui.Toast("没有发现摄像头");
            that.stop();
        });
    }
};

export { Scan };

