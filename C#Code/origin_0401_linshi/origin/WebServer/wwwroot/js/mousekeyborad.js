
function sendMousedown(event) {
    if (event.button == 2) {
        sendMsg("0x0008", "");
    }
    else if (event.button == 0) {
        sendMsg("0x0002", "");
    }
}

function sendMouseup(event) {
    if (event.button == 2) {
        sendMsg("0x0010", "");
    }
    else if (event.button == 0) {
        sendMsg("0x0004", "");
    }
}

//function sendClickLeft() {
//    sendMsg("0x0002", "");
//    sendMsg("0x0004", "");
//}

//function sendClickRight() {
//    sendMsg("0x0008", "");
//    sendMsg("0x0010", "");
//}

function sendKeyDown(key) {
    sendMsg("0xFFFF", key + "");
}
function sendKeyUp(key) {
    sendMsg("0xFFFE", key + "");
}

function sendXY(offsetX, offsety) {
    var image = document.getElementById("desktopimg");
    if (image == null) {
        sendMsg("0x0001", '"' + offsetX + ',' + offsety + '"');
    }
    else {
        webWidth = image.width;
        webHeight = image.height;
        imgWidth = image.naturalWidth;
        imgHeight = image.naturalHeight;
        sendMsg("0x0001", '"' + offsetX + ',' + offsety + ',' + webWidth + ',' + webHeight + ',' + imgWidth + ',' + imgHeight + '"');
    }

}

function sendMsg(code, msg) {
    var _url = self.origin + '/Remote/SendMouseKeyboard';

    fetch(_url,
        {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                'code': code,
                'msg': msg
            })

        });
}