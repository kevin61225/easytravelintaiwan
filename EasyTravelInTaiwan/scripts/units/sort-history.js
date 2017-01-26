$("#SaveSortedPlaceBtn").click(function () {
    $.ajax({
        type: "POST",
        url: "@Url.Action("PostDirectionHistory", "Map")",
        data: JSON.stringify({ tid: $waypointId, sortedList: SortedNumber }),
    contentType: 'application/json; charset=utf-8',
    success: function (data) {
        console.log(data);
        if (data.Status == "1") {
            $.growlUI(data.Messages);
        }
        else {
            $("#SortHistory").html(data);
            $("#HistorylListSelectPicker option").last().attr("selected", "selected");
            $('#HistorylListSelectPicker').selectpicker('refresh');
            $.growlUI('路徑已儲存 !');
        }
    }
});
});

$("#DeleteSortedPlaceBtn").click(function () {
    $.ajax({
        type: "POST",
        url: "@Url.Action("DeleteDirectionHistory", "Map")",
        data: JSON.stringify({ tid: $waypointId, sortedList: SortedNumber }),
    contentType: 'application/json; charset=utf-8',
    success: function (data) {
        if (data.Status == "1") {
            $.growlUI(data.Messages);
        }
        else {
            $("#SortHistory").html(data);
            $("#HistorylListSelectPicker option").last().attr("selected", "selected");
            $('#HistorylListSelectPicker').selectpicker('refresh');
            $('.dropdown-menu .inner >li:last').click();
            $.growlUI('已刪除規劃紀錄 !');
        }
    }
});
});