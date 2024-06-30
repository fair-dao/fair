/*fair Js库 */
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export class base {
    /**
     * 获取指定字符串的hash值
     * @param str
     */
    static getHash(str) {
        var hash = 1315423911, i, ch;
        for (i = str.length - 1; i >= 0; i--) {
            ch = str.charCodeAt(i);
            hash ^= ((hash << 5) + ch + (hash >> 2));
        }
        return (hash & 0x7FFFFFFF);
    }
    /**
     * 写日志
     * @param msg
     * @param obj
     */
    static log(msg, ...obj) {
        console.log(msg, obj);
    }
    static playAudio(url, loop = false) {
        let audio = document.createElement("audio");
        audio.setAttribute("autoplay", "autoplay");
        var src = document.createElement("source");
        let type = "mp3";
        if (url.indexOf(".wav") > 0) {
            type = "wav";
        }
        src.setAttribute("type", "audio/" + type);
        src.src = url;
        var embed = document.createElement("embed");
        embed.setAttribute("height", "0");
        embed.setAttribute("width", "0");
        embed.setAttribute("src", "");
        audio.appendChild(src);
        audio.appendChild(embed);
        document.body.appendChild(audio);
        audio.loop = loop;
        audio.onload = function () {
            audio.play();
        };
        audio.onended = function () {
            audio.remove();
        };
        return audio;
    }
    /**
     * 将dataurl转换为blob
     * @param base64
     */
    static B64ToBlob(base64) {
        let type = base64.split(",")[0].match(/:(.*?);/)[1]; //提取数据类型( image/png等)     
        let bytes = window.atob(base64.split(',')[1]); //提取数据
        let buffer = new ArrayBuffer(bytes.length); //创建数据缓存
        let bts = new Uint8Array(buffer);
        for (var i = 0; i < bytes.length; i++) {
            bts[i] = bytes.charCodeAt(i);
        }
        return new Blob([buffer], { type: type });
    }
    /**
     * 将dataurl转换uint8
     * @param base64
     */
    static B64ToU8(base64) {
        let type = base64.split(",")[0].match(/:(.*?);/)[1]; //提取数据类型( image/png等)     
        let bytes = window.atob(base64.split(',')[1]); //提取数据
        let buffer = new ArrayBuffer(bytes.length); //创建数据缓存
        let bts = new Uint8Array(buffer);
        for (var i = 0; i < bytes.length; i++) {
            bts[i] = bytes.charCodeAt(i);
        }
        return bts;
    }
    //压缩图片
    static ZipImg(imgDataUrl) {
        return __awaiter(this, void 0, void 0, function* () {
            return new Promise(function (r1, r2) {
                try {
                    let a = new Promise(function (resolve, reject) {
                        const img1 = new Image();
                        img1.src = imgDataUrl;
                        img1.onload = function () {
                            resolve(img1);
                        };
                        img1.onerror = function (e) {
                            reject(e);
                        };
                    });
                    let data;
                    a.then(function (e) {
                        var canvas = document.createElement('canvas');
                        var context = canvas.getContext('2d');
                        let originWidth = e.width;
                        let originHeight = e.height;
                        // 最大尺寸限制
                        var maxWidth = 600, maxHeight = 600;
                        // 目标尺寸
                        var targetWidth = originWidth, targetHeight = originHeight;
                        // 图片尺寸超出限制
                        if (originWidth > maxWidth || originHeight > maxHeight) {
                            if (originWidth / originHeight > maxWidth / maxHeight) {
                                // 更宽，按照宽度限定尺寸
                                targetWidth = maxWidth;
                                targetHeight = Math.round(maxWidth * (originHeight / originWidth));
                            }
                            else {
                                targetHeight = maxHeight;
                                targetWidth = Math.round(maxHeight * (originWidth / originHeight));
                            }
                        }
                        // canvas对图片进行缩放
                        canvas.width = targetWidth;
                        canvas.height = targetHeight;
                        // 清除画布
                        context.clearRect(0, 0, targetWidth, targetHeight);
                        // 图片压缩
                        context.drawImage(e, 0, 0, targetWidth, targetHeight);
                        // canvas转为File并上传
                        r1(canvas.toDataURL("image/png"));
                    });
                }
                catch (ex) {
                    r2(ex);
                }
            });
        });
    }
    /**
  * 移除指定的元素,用于移除loadJs,playAudio返回的元素
  * @param id
  */
    static removeNode(id) {
        if (id) {
            if (id.parentNode) {
                id.parentNode.removeChild(id);
            }
        }
    }
    /**
     * 加载脚本
     * @param id 加载脚本的元素Id,可以为空，默认为app节点（存在） 或 body节点
     * @param jslist 脚本列表
     * 实例 fair.base.loadJs("app","/lib/jquery.js","/lib/aaa.js","alert('ok');");
     */
    static loadJs(...jss) {
        let element = document.createElement("div");
        element.style.display = "none";
        document.body.appendChild(element);
        base._loadJs(element, jss);
        return element;
    }
    static _loadJs(element, jslist) {
        try {
            let js = jslist[0];
            jslist.shift();
            if (js !== "" && js !== "app") {
                let name = base.getHash(js).toString();
                if (js.indexOf('(') > 0) { /*代码*/
                    var script = document.createElement("script");
                    script.type = "text/javascript";
                    if (name) {
                        script.setAttribute("name", name);
                    }
                    try {
                        // firefox、safari、chrome和Opera
                        script.appendChild(document.createTextNode(js));
                    }
                    catch (ex) {
                        // IE早期的浏览器 ,需要使用script的text属性来指定javascript代码。
                        script.text = js;
                    }
                    element.appendChild(script);
                    if (jslist.length > 0) {
                        base._loadJs(element, jslist);
                    }
                }
                else {
                    var head = document.getElementsByTagName('head')[0];
                    var script = document.createElement('script');
                    script.type = 'text/javascript';
                    script.setAttribute("name", name);
                    script.src = js;
                    if (jslist.length > 0) {
                        script.onload = function (ev) {
                            base._loadJs(element, jslist);
                            script.onload = null;
                        };
                    }
                    element.appendChild(script);
                }
            }
            else
                base._loadJs(element, jslist);
        }
        catch (e) {
            console.error(e);
        }
    }
    //public static OpenSiderBar(): void {
    //    var sidebar = document.querySelector(".com-sidebar");
    //    if (sidebar) {
    //        sidebar.setAttribute("style", "display:block");
    //    }
    //}
    /**
     * 将元素内容复制到剪贴板
     * @param selector 元素选择器
     */
    static CopyData(selector) {
        let element;
        element = document.querySelector(selector);
        element.select();
        document.execCommand("Copy");
    }
    /**
     * 复制文本到剪贴板
     * @param text 内容
     */
    static CopyText(text) {
        navigator.clipboard.writeText(text).then(function () {
            ui.Toast(text + ' 复制成功', "");
        }).catch(function (err) {
            ui.Toast(err + ' 复制失败', "err");
        });
    }
    /**
     * 从指定网址中获取Query值 ，如:http://w.a.com/?ac=1111,读取aa的值:GetQuery(url,"aa")
     * @param url
     * @param paramName 参数名
     */
    static GetQuery(url, paramName) {
        var p = "?" + paramName + "=";
        var index = url.indexOf(p);
        if (index < 0)
            index = url.indexOf("&" + paramName + "=");
        if (index >= 0) {
            var u = url.substr(index + p.length);
            index = u.indexOf("&");
            if (index === 0)
                return "";
            if (index > 0) {
                return u.substr(0, index);
            }
            else
                return u;
        }
        else {
            return null;
        }
    }
}
export class env {
    /**
     * 是否为PC
     * */
    static IsPC() {
        var userAgentInfo = navigator.userAgent.toLowerCase();
        var agents = ["and", "phone", "symbianos", "windows phone", "ipad"];
        var flag = true;
        for (var v = 0; v < agents.length; v++) {
            if (userAgentInfo.indexOf(agents[v]) > 0) {
                flag = false;
                break;
            }
        }
        return flag;
    }
    /**
     * 是否为触屏
     * */
    static IsTouch() {
        return document.hasOwnProperty("ontouchstart");
    }
    /**
     * 是否为android终端
     * */
    static IsAndroid() {
        return navigator.userAgent.indexOf('Android') > -1 || navigator.userAgent.indexOf('Adr') > -1;
    }
    static IsIOS() {
        return /(iPhone|iPad|iPod|iOS)/i.test(navigator.userAgent);
    }
    /**
     * 是否为微信浏览器
     * */
    static IsWeChat() {
        var userAgent = navigator.userAgent.toLowerCase();
        return userAgent.indexOf("micromessenger") >= 0;
    }
    /**
     * 当前网页是否在后台运行
     * */
    static RunInBackground() {
        return document.visibilityState !== "visible";
    }
}
export class ui {
    /**
     * 显示信息
     * @param msg
     * @param level err:错误信息 warn:警告信息 always:一直显示
     */
    static Toast(msg, level) {
        var bb = document.createElement("div");
        bb.innerHTML = '<span class=\"' + level + '\">' + msg + '</span>';
        bb.setAttribute("class", "com-toast");
        bb.setAttribute("style", "");
        var len = msg.length;
        len = len / 6;
        if (len < 4)
            len = 4;
        else if (len > 20)
            len = 20;
        switch (level) {
            case "err":
                len = len * 2;
                break;
            case "warn":
                len = len * 1.2;
                break;
            case "always":
                len = 0;
                break;
        }
        //len = 0;
        document.body.appendChild(bb);
        bb.style.left = "calc((100vw - " + bb.clientWidth + "px)/2)";
        bb.style.top = "calc((100vh - " + bb.clientHeight + "px)/2)";
        var hide;
        hide = function () {
            try {
                document.body.removeChild(bb);
            }
            catch (e) { }
        };
        /*自动隐藏信息层*/
        if (len > 0)
            setTimeout(hide, len * 500);
        bb.addEventListener("click", function () {
            try {
                clearTimeout(hide);
            }
            catch (e) { }
            this.parentNode.removeChild(this);
        });
    }
    /**
     * 显示消息
     * @param title 标题
     * @param msg   消息内容
     * @param option 显示选项{showTime:保留毫秒数，默认5000毫秒; url:点击内容后的跳转地址;hasClose:显示关闭按钮}
     */
    static Message(title, msg, option) {
        option = option || {};
        if (typeof (option.showTime) === "undefined") {
            option.showTime = 5000;
        }
        option.hasClose = option.hasClose || true;
        option.url = option.url || null;
        let bb = document.createElement("article");
        let html = '<div class="message-header"><p>' + title + '</p>';
        if (option.hasClose) {
            html += '<button class="delete" aria-label="delete"></button>';
        }
        html += "</div>";
        if (option.url) {
            html += '<a href="' + option.url + ">";
        }
        html += '<div class="message-body">' + msg + ' </div>';
        if (option.url) {
            html += "</a>";
        }
        bb.innerHTML = html;
        bb.setAttribute("class", "message com-topmsg  is-info");
        document.body.appendChild(bb);
        var hide;
        hide = function () {
            try {
                if (option.close) {
                    option.close();
                }
            }
            catch (e) { }
            try {
                document.body.removeChild(bb);
            }
            catch (e) { }
        };
        /*自动隐藏信息层*/
        if (option.showTime > 0) {
            setTimeout(hide, option.showTime);
        }
        if (option.hasClose) {
            bb.querySelector(".delete").addEventListener("click", function () {
                try {
                    clearTimeout(hide);
                }
                catch (e) { }
                hide();
            });
        }
        return bb;
    }
    /**
     * 模式框
     * @param title 标题
     * @param content 内容
     * @param buttons 按钮列表 [{text:"添加",class:"is-success",click:function(){}},{text:"确认",url:"http://www.baidu.com/"}]
     * @param option 选项  close:function{} 关闭事件
     */
    static Modal(title, content, buttons, option) {
        option = option || {};
        let html = '<div class="modal-background"></div><div class="modal-card com-modal-card"><header class="modal-card-head"><p class="modal-card-title">'
            + title + '</p><button class="delete close is-medium" aria-label="close"></button></header>'
            + '<section class="modal-card-body">' + content + '</section>'
            + '<footer class="modal-card-foot">';
        for (let i = 0; i < buttons.length; i++) {
            let btn = buttons[i];
            let txt = btn.text;
            html += '<button class="button ' + (btn.css || "") + '">' + txt + "</buttton>";
        }
        html += "</footer></div>";
        let el = document.createElement("div");
        el.setAttribute("class", "modal is-active");
        el.innerHTML = html;
        document.body.appendChild(el);
        //所有的关闭按钮
        el.querySelectorAll(".close").forEach(m => m.addEventListener("click", function () {
            if (option.close) {
                option.close();
            }
            el.parentNode.removeChild(el);
        }));
        //所有操作按钮
        var btns = el.querySelector("footer").querySelectorAll(".button");
        for (let i = 0; i < buttons.length; i++) {
            var btn = buttons[i];
            if (btn.click) {
                btns[i].addEventListener("click", btn.click);
                btns[i].addEventListener("click", function () {
                    el.parentNode.removeChild(el);
                });
            }
        }
    }
    /**
     * 显示或隐藏边栏
     */
    static SwitchSidebar(state) {
        var sidebar = document.querySelector("aside");
        if (sidebar) {
            let disp = (state == 1 ? "block" : (state == 0 ? "none" : (sidebar.style.display == "block" ? "none" : "block")));
            sidebar.style.display = disp;
        }
    }
    /**
     * 获取屏幕宽度和高度,元素左坐标，顶坐标，宽度，高度
     * */
    static GetElementPoint(el) {
        return [window.innerWidth, window.innerHeight, el.offsetLeft, el.offsetTop, el.clientWidth, el.clientHeight];
    }
    /**
     * 弹出一个层
     * @param selector
     * @param x
     * @param y
     */
    static Popup(selector, x, y) {
        var el = document.querySelector(selector);
        var click = function () {
            el.style.zIndex = "-1111";
        };
        if (el) {
            //el.style.left = "0px";
            //el.style.right = "0px";
            //el.style.width = el.clientWidth+"px";
            //el.style.height = el.clientHeight+"px";
            //修改显示的位置
            console.log(el.offsetWidth + "," + el.offsetHeight);
            var width = el.offsetWidth + 6;
            var height = el.offsetHeight + 6;
            var bodyWidth = window.innerWidth;
            var bodyHeight = window.innerHeight;
            var newX = x;
            var newY = y;
            if (x + width > bodyWidth) {
                newX = x - width;
            }
            if (y + height > bodyHeight) {
                newY = y - height;
            }
            el.style.left = newX + "px";
            el.style.top = newY + "px";
            el.style.zIndex = "999";
            try {
                el.removeEventListener("click", click);
            }
            catch (e) {
            }
            el.addEventListener("click", click);
        }
    }
}
//# sourceMappingURL=fair.js.map