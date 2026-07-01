/**
 * Created by ljw on 2023/3/1.
 */
var inputAutoComplete = (function () {
    var getInputVal, saveData, init, clean, dataMap = {
        size: 10
    };
    getInputVal = function ($input) {
        if ($input.length > 0 && $input[0].tagName === 'INPUT') {
            return $.trim($input.val());
        }
    };
    saveData = function (newValue, storeId, size) {
        var localDataStr = localStorage.getItem(storeId);
        var dataArray = localDataStr ? localDataStr.split(',') : [];
        var newDataIndex = $.inArray(newValue, dataArray);
        if (newDataIndex !== -1) {
            dataArray.splice(newDataIndex, 1);
        }
        dataArray.unshift(newValue);
        if (dataArray.length > size) {
            dataArray.pop();
        }
        localStorage.setItem(storeId, dataArray);
    };

    init = function ($input, storeId, size) {
        var newValue = getInputVal($input);
        var storeId = storeId || $input.attr('id');
        if (!newValue || !storeId) return;
        saveData(newValue, storeId, size || dataMap.size);
        var source = localStorage.getItem(storeId);      
        $input.autocomplete({
            source: source.split(','),
            autoFocus: true,
            minLength:0
        }).focus(function(){
            if (this.value == "")
                $(this).trigger('keydown.autocomplete');
        });
    };

    clean = function ($input, storeId) {
        storeId = storeId || $input.attr('id');
        if (!storeId) return;
        localStorage.removeItem(storeId);
        $input.autocomplete({
            source: []
        });
    };

    return {
        init: init,
        clean: clean
    };
})();