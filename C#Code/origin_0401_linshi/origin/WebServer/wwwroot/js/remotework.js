var timeInter;

function Close() {
    clearInterval(timeInter);

}
function getResult(data) {
    postMessage(data);
}

function GetImg() {
    var _url = self.origin + '/Remote/GetScreenByJson';

    fetch(_url)
        .then(response => response.json())
        .then(data => getResult(data))
        //.then(data => postMessage(data))
        .catch(error => console.error('Unable to get Imgs.', error));
        //.catch(error =>
        //{
        //    var data = {
        //        'value': { 'code': '0' }
        //    };       
        //    getResult(data);
        //    console.error('Unable to get Imgs.', error);
        //});
}
function Start() {
        timeInter = setInterval("GetImg()", 200);
}

Start();