function Message(messageEvent) {
    var data = JSON.parse(messageEvent.data);
    if (data.code != 200) {
        return;
    }
    var msg = data.msg;
    if (msg.sn !== undefined) {
        $("#sn").html(msg.sn);
    }
    if (msg.hdver !== undefined) {
        $("#sw").html(msg.hdver);
    }

    if (msg.ip !== undefined) {
        $("#ip").html(msg.ip);
    }
    if (msg.mac !== undefined) {
        $("#mac").html(msg.mac);
    }

    if (msg.lxiaddr  !== undefined) {
        $("#lxiaddr").html(msg.lxiaddr );
    }
    if (msg.lxiver  !== undefined) {
        $("#lxiver").html(msg.lxiver );
    }
    if (msg.time  !== undefined) {
        $("#time").html(msg.time );
    }
}