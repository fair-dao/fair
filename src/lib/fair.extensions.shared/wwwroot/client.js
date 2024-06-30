import * as fair from '/_content/fair.ui/js/fair.ui.min.js';
window.fair = fair;
window.blazorCulture = {
    get: function () {
        try {
            return window.localStorage['BlazorCulture'];
        } catch (e) {
            console.log(e);
        }
    },
    set: function (value) {
        window.localStorage['BlazorCulture'] = value;
    }
};


/**
 * 调用Maui插件
 * @param {any} cmd 命令
 * @param {any} params  参数列表
 */

window.MauiCall = function (cmd, params) {

    if (cmd === "check") { //检测是否支持MauiPlug
        if (window.mauiplug) return true;
        else return false;
    }
    if (window.mauiplug) {
        let data = JSON.stringify(params);
        return window.mauiplug.CallMauiPlug(cmd, data);
    }
    return null;
};

/**
 * 返回否浏览器是否在后台运行
 * */
window.RunInBackgroud = function () {
    return document.hidden;
}


window.ClientEnv = function () {
    var env = {};
    if (window.mauiplug) {
        env.IsApp = true;
        var data = window.mauiplug.CallMauiPlug("device", "");
        if (data) {
            let arr = data.split(',');
            if (arr.length >= 4) {
                env.Manufacturer = arr[0];
                env.Model = arr[1];
                env.DeviceId = arr[2];
                env.ClientVer = arr[3];
            }
        }

    } else env.IsApp = false;
    env.DeviceWidth = window.screen.width;
    env.DeviceHeight = window.screen.height;
    env.IsAndroid = fair.env.IsAndroid();
    env.IsIOS = fair.env.IsIOS();
    return env;
}

