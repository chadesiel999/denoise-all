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

//24/6/14
function detectZoom() {
    var ratio = 0,
        screen = window.screen,
        ua = navigator.userAgent.toLowerCase();
    if (window.devicePixelRatio) {
        ratio = window.devicePixelRatio;
    } else if (!isNaN(document.documentElement.style.zoom)) {
        ratio = document.documentElement.style.zoom || 1;
    } else {
        ratio = Math.round((window.outerWidth / window.innerWidth) * 100) / 100;
        if (ratio.toString().split('.')[1].length < 2) {
            ratio = Math.round(ratio);
        }

    }
    if (ratio) {
        ratio = Math.round(ratio * 100);
    }
    return ratio;
}

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
            isWorking = event.data.value.code == 200 && event.data.value.msg.length > 400;
            if (isWorking) {

                $("#floating").prop("hidden", true);
                image.src = "data:image/png;base64," + event.data.value.msg;
            }
            else {
         
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
    showTip();

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

}
function MaxFullScreen() {
    var fullscreenElement = document.fullscreenElement || document.mozFullScreenElement || document.webkitFullscreenElement;

    if (!fullscreenElement) {
        launchFullScreen(document.getElementById("desktopimg")); // 单独元素显示全屏
    }

}

function showTip() {
    toastr.warning(localizedStrings["PressExitTheFullScreen"]);
}

function PannelFullScreen() {
    var userAgent = navigator.userAgent;
    var isEdge = userAgent.indexOf("Edg") > -1; //判断是否IE的Edge浏览器
    //var isFF = userAgent.indexOf("Firefox") > -1; //判断是否Firefox浏览器
    var isChrome = userAgent.indexOf("Chrome") > -1
        && userAgent.indexOf("; Win") > -1
        && userAgent.indexOf("Safari") > -1; //判断Chrome浏览器
    if (!isChrome && !isEdge) {
        toastr.error(localizedStrings["NotChrome"]);
        return;
    }
    var zoom = detectZoom();

    if (zoom < 98 || zoom > 102) {
        toastr.error(localizedStrings["NotRightScale"]);
        return;
    }

    var pannelHeight = $(window).height();
    var pannelWidth = pannelHeight / 9 * 16;
    zoomFull = 1024 / pannelWidth;
    showTip();

    $("#webdesktop").css({
        "position": "absolute",
        "text-align": "center",
        "top": "0px",
        "left": "0px",
        "right": "0px",
        "bottom": "0px",
        "background": "black",
        "visibility": "visible",
        "filter": "Alpha(opacity=90)"
    });
    $("#desktopimg").css({
        width: pannelWidth,
        height: pannelHeight
    });

    $("nav").hide();
    $("footer").hide();
    isPannel = true;
}
function exitFullScreen() {
    isFull = document.fullscreenElement || document.mozFullScreenElement || document.webkitFullscreenElement;

    if (!isFull) {
        location.reload();
    }
}

$(document).ready(function () {
    if (!document.getElementById("webdesktop")) {
        return;
    }
    //toastr.options.positionClass = 'toast-center-center';

    $("#webdesktop").bind("contextmenu", function (e) {
        return false;
    });

    startWorker();
    $("#webdesktop").mousemove(function (event) {
        hasMouseMove = true;
        if (!isWorking) {
            return;
        }

        if (isFull) {
            var zoomDetect = detectZoom() / 100;/// 0.95;
            event.offsetX = Math.round(event.offsetX * zoomDetect);
            event.offsetY = Math.round(event.offsetY * zoomDetect);
        }

        else {
            //event.offsetX = Math.round(event.offsetX / zoomFull);
            //event.offsetY = Math.round(event.offsetY / zoomFull);
        }

        sendXY(event.offsetX, event.offsetY);
        xx = event.offsetX;
        yy = event.offsetY;
    });
    $("#webdesktop").mouseleave(function () {
        sendXY(0, 0);
        xx = 0;
        yy = 0;
     
    });
    $("#webdesktop").mousedown(function (event) {
        hasMouseMove = false;
        if (!isWorking) {
            return;
        }
        sendMousedown(event);
    });

    $("#webdesktop").mouseup(function (event) {

        if (!isWorking) {
            return;
        }
        sendMouseup(event);
        hasMouseMove = false;
    });

    $("#webdesktop").dblclick(function (e) {
        if (!isWorking) {
            return;
        }
        sendDoubleClick();
    });
    $(document).bind("keyup", function (event) {
        var key = event.which;
        //esc退出全屏 
        if (key == 27) {
            if (isFull) {
                location.reload();
            }
            else if (isPannel) {
                $("nav").show();
                $("footer").show();
                $("#webdesktop").css({
                    position: "relative",
                    width: "1024px",
                    height: "576px"
                });

                $("#desktopimg").css({

                    width: 1024,
                    height: 576
                });
                zoomFull = 0.64;
                isPannel = false;
            }
            event.returnValue = false;
        }
    });
    $("#desktopimg").bind("keyup", function (event) {
        var key = event.which;
        if (key == 8 || key == 13 || key == 16 || key == 20
            || key >= 37 || key <= 40 || key >= 48 || key <= 103
            || key == 122) {

            sendKeyUp(key);
        }
    });
    $("#desktopimg").bind("keydown", function (event) {
        var key = event.which;
        if (key == 122) {
            MaxFullScreen();
            event.returnValue = false;
        }
        else if (key == 8 || key == 13 || key == 16 || key == 20
            || key >= 37 || key <= 40 || key >= 48 || key <= 103) {

            sendKeyDown(key);
        }
    });
});


document.addEventListener("fullscreenchange", exitFullScreen);
document.addEventListener("mozfullscreenchange", exitFullScreen);
document.addEventListener("webkitfullscreenchange", exitFullScreen);
document.addEventListener("msfullscreenchange", exitFullScreen);