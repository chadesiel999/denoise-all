
var host = window.location.hostname;

function ExportHistoryFile(filefullname) {
    // 创建一个单击事件
    var event = new MouseEvent('click');
    var a = document.createElement('a');
    a.download = filefullname;
    // 将生成的URL设置为a.href属性
    a.href = '/outfiles/' + filefullname;
    // 触发a的单击事件
    a.dispatchEvent(event);
}
function padding(num, length) {
    return (Array(length).join("0") + num).slice(-length);
}
function getResult(data) {
    //var data = result.data.value;
    if (data != null && data.code == 200) {
        toastr.info(localizedStrings["ExportSuccessfully"]);
        //生成文件全名
        var filefullname = $("#filename").val();
        var pattern = new RegExp("\\.[^)]+")
        var format = $("#format").find("option:selected").text().match(pattern);
        filefullname = filefullname + format;

        ExportHistoryFile(filefullname);
        CreatFileName();
    }
    else
    {
        toastr.error(localizedStrings["ExportFailure"]);
    }

}
function ExportFile() {

    var channel = $("#channel").val();
    var format = $("#format").val();
    $("#filename").val(Stripscript($("#filename").val()));
    var filename = $("#filename").val();
    if (filename == "") {
        CreatFileName();
    }
    var _url = self.origin + '/ToExportFile';

    fetch(_url,
        {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                'code': "200",
                'msg': '{\"ChannelID\":' + channel + ',\"Format\":' + format + ',\"FileName\":\"' + filename + '\"}'
            })

        })
        .then(response => response.json())
        .then(data => getResult(data))
        .catch(error => console.error('Export Error.', error));
}

function Openwin() {
    var width = 550;
    var height = 500;
    window.open("/outfiles", "newwindow", "top=" + (window.screen.height - 30 - height) / 2 + ",left= " + (window.screen.width - 10 - width) / 2 + ",height=" + height + ", width=" + width + ", toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, status=no");
}

function Message(messageEvent) {
    var data = JSON.parse(messageEvent.data);
    var a = document.createElement('a');
    if (data.code != 200) {
        toastr.error(localizedStrings["ExportFailure"]);
        return;
    }
    //生成文件全名
    var filefullname = $("#filename").val();
    var pattern = new RegExp("\\.[^)]+")
    var format = $("#format").find("option:selected").text().match(pattern);
    filefullname = filefullname + format;

    // 创建一个单击事件
    var event = new MouseEvent('click');

    a.download = filefullname;
    // 将生成的URL设置为a.href属性
    a.href = '/outfiles/' + filefullname;
    // 触发a的单击事件
    a.dispatchEvent(event);
    CreatFileName();
}

function Stripscript(s) {
    var pattern = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）——|{}【】‘；：”“'。，、？]")
    var rs = "";
    for (var i = 0; i < s.length; i++) {
        rs = rs + s.substr(i, 1).replace(pattern, '');
    }
    return rs;
}

function CreatFileName() {
    var now = new Date();
    var name = 'Data_'
        + now.getFullYear() + '_' + (now.getMonth() + 1) + '_' + padding(now.getDate(), 2) + '_'
        + padding(now.getHours(), 2) + '_' + padding(now.getMinutes(), 2) + '_' + padding(now.getSeconds(), 2) + '_'
        + padding(now.getMilliseconds(), 3);
    $("#filename").val(name);
}
function Init() {
    toastr.options.positionClass = 'toast-center-center';
    var fileObj = document.getElementById("filename");
    if (fileObj) {
        CreatFileName();
    }
}
Init();


