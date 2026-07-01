var screenWorker;
var isWorking = false;

var host = window.location.hostname;
var now;
var lastMinutes;
var lastDate;
var hasMouseMove = false;
var zoomFull = 0.64;
var isFull = false;
var isPannel = false;

var xx = 0;
var yy = 0;

function detectZoom() {
    var ratio = 0,
        screen = window.screen,
        ua = navigator.userAgent.toLowerCase();
    if (window.outerWidth !== undefined && window.innerWidth !== undefined) {
        ratio = window.outerWidth / window.innerWidth;
    }
    else if (window.devicePixelRatio !== undefined) {
        ratio = window.devicePixelRatio;
    }
    else if (~ua.indexOf('msie')) {
        if (screen.deviceXDPI && screen.logicalXDPI) {
            ratio = screen.deviceXDPI / screen.logicalXDPI;
        }
    }

    if (ratio) {
        ratio = Math.round(ratio * 100);
    }
    return ratio;
};

function SaveScreen() {
    var now = new Date();
    var image = new Image();
    image.setAttribute('crossOrigin', 'anonymous');
    image.onload = function () {
        var canvas = document.createElement('canvas');
        canvas.width = image.width;
        canvas.height = image.height;

        var context = canvas.getContext('2d');
        context.drawImage(image, 0, 0, image.width, image.height);
        var url = canvas.toDataURL('image/png');

        var a = document.createElement('a');
        // 创建一个单击事件
        var event = new MouseEvent('click');

        a.download = 'Img_'
            + now.getFullYear() + '_' + now.getMonth() + 1 + '_' + now.getDate() + '_'
            + now.getHours() + '_' + now.getMinutes() + '_' + now.getSeconds() + '_'
            + now.getMilliseconds();
        // 将生成的URL设置为a.href属性
        a.href = url;
        // 触发a的单击事件
        a.dispatchEvent(event);
    }

    image.src = document.querySelector("#desktopimg").src;
}
function padding(num, length) {
    return (Array(length).join("0") + num).slice(-length);
}
function getWeekDate() {
    var day = now.getDay();
    var weeks = new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");
    var week = weeks[day];
    return week;
}
function refreshDate() {
    now = new Date();
    if (lastMinutes != now.getMinutes()) {
        $("#time").html(padding(now.getHours(), 2) + ":" + padding(now.getMinutes(), 2));
        lastMinutes = now.getMinutes();
    }
    if (lastDate != now.getDate()) {
        //format - 10月21日, 星期一
        $("#day").html(now.getMonth() + 1 + "Months" + now.getDate() + "Days," + getWeekDate());
        lastDate = now.getDate();
    }
}
function startWorker() {
    if (typeof (Worker) !== "undefined") {
        if (typeof (screenWorker) == "undefined") {
            screenWorker = new Worker("./js/remotework.js", { name: window.location.hostname });
        }
        screenWorker.onmessage = function (event) {
            var image = document.getElementById("desktopimg");
            isWorking = event.data.value.code == 200;
            if (isWorking) {

                $("#floating").prop("hidden", true);
                image.src = "data:image/png;base64," + event.data.value.msg;
            }
            else {
                //最小化
                $("#floating").prop("hidden", false);
                refreshDate();
            }
        };
    }
    else {
        document.getElementById("result").innerHTML = "Sorry, your browser does not support Web Workers...";
    }
}
function launchFullScreen(element) {

    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen();
    } else if (element.webkitRequestFullScreen) {
        element.webkitRequestFullScreen();
    } else if (element.msRequestFullscreen) {
        element.msRequestFullscreen();
    }
    else {
        toastr.error("The current browser does not support partial full screen!");
        return;
    }
    isFull = true;
    showTip();
}
function MaxFullScreen() {
    var fullscreenElement = document.fullscreenElement || document.mozFullScreenElement || document.webkitFullscreenElement;

    if (!fullscreenElement) {
        launchFullScreen(document.getElementById("webdesktop")); // 单独元素显示全屏
        //launchFullScreen(document.getElementById("desktopimg")); // 单独元素显示全屏
    }
    var i = 0;
    $('#desktopimg').on('click', function () {
        i++;
        setTimeout(function () {
            i = 0;
        }, 500);
        if (i > 1) {
            location.reload();
            i = 0;
        }
    })

}

function showTip() {
    toastr.options.containerId = 'webdesktop';
    toastr.options.timeOut = 500000;
    toastr.options.extendedTimeOut = 500000;
    toastr.warning('Double-click to exit the full screen');
}

$(document).ready(function () {

    if (!document.getElementById("webdesktop")) {
        return;
    }
    //toastr.options.positionClass = 'toast-center-center';
    $("#webdesktop").bind("contextmenu", function (e) {
        return false;
    });

    if (document.getElementById("get-img")) {
        GetImg();
    }
    else
    {
        startWorker();
    }
   
});

