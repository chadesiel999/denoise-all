function getResult(data) {
    var image = document.getElementById("desktopimg");
    isWorking = data.value.code == 200;
    if (isWorking) {

        $("#floating").prop("hidden", true);
        image.src = "data:image/png;base64," + data.value.msg;
    }
    else {
        //最小化
        $("#floating").prop("hidden", false);
        refreshDate();
    }
}

function GetImg() {
    var _url = self.origin + '/Remote/GetScreenByJson';

    fetch(_url)
        .then(response => response.json())
        .then(data => getResult(data))
        //.then(data => postMessage(data))
        .catch(error => console.error('Unable to get Imgs.', error));
}
