$(document).ready(function () {
    ManualReplaceImages();
});

function ManualReplaceImages() {
 //подключаем динамические изображения - для каждого получаем байтовый массив через веб-сервис
    $(".img_replace").each(function (key, value) {
        var _url = '';
        if ($(value).attr('source')) {
            _url = $(value).attr('source');
        }
        $.ajax({
            dataType: "json",
            type: "POST",
            data: { url: _url, FileName: $(value).attr('filename'), IsSystemIcon: $(value).attr('issystemicon'), IsPreview: $(value).attr('IsPreview')},
            url: 'EsMVCWebUtils.asmx/GetBytesFromUrl',
            success: function (data, status, xhr) {
                var doc = document.getElementById(value.id);
                doc.src = 'data:image/png;base64,' + data.Content;
            },
            error: function (error) {
                alert(error.responseText);
            }
        })
    });
}