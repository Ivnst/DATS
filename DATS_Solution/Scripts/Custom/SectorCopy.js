function copyBtn(id) {
    $.post("/SectorSetting/Copy", { id: id },
    function (data) {
        location.reload();
    });
}