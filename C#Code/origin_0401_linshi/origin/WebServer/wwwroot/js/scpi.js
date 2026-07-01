function Message(messageEvent) {
    var data = JSON.parse(messageEvent.data);

    if (data.code == 200) {
        $("#result").val(data.msg);
    }
}
 
function ClearBox() {
    $("#result").val("");
}

function ClearHistory() {
    inputAutoComplete.clean($('#CMD'));
}
function Init() {
    $('#commandsDropDown a').on('click', function () {
        var commandTextBox = $('#CMD');
        var predefinedCommand = $(this).text();
        commandTextBox.val(predefinedCommand);
    });
    inputAutoComplete.init($('#CMD'));
    
}
Init();

