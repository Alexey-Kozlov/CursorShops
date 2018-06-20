function getBinaryFile(_url, _fileName, webService, SiteID, WebID, TaskID, ListID, CurrentUserID, IsMultimedia) {
    SiteID = typeof SiteID !== 'undefined' ? SiteID : '';
    WebID = typeof WebID !== 'undefined' ? WebID : '';
    TaskID = typeof TaskID !== 'undefined' ? TaskID : '';
    ListID = typeof ListID !== 'undefined' ? ListID : '';
    CurrentUserID = typeof CurrentUserID !== 'undefined' ? CurrentUserID : '';
    var _data = {};
    _data.FileName = _fileName;
    if (SiteID != '') {
        _data.SiteID = SiteID;
        _data.WebPath = WebID;
        _data.WebID = WebID;
        _data.TaskID = TaskID;
        _data.ListID = ListID;
        _data.CurrentUserID = CurrentUserID;
    } else {
        _data.url = _url;
    }
    if (IsMultimedia && IsMultimedia == true) {
        $('#player1-container').on('dialogclose', function (event) {
            audio_player.pause();
        });
        $('#player2-container').on('dialogclose', function (event) {
            audio_player.pause();
        });
        var media_id;
        if (_url.indexOf('.mp3') != -1) {
            $('#player2-container').dialog({ modal: true, height: 100, width: 600 });
            $('#player2-container').dialog('option', 'title', _fileName);
            media_id = document.getElementById('player2-container').querySelector('.mejs__container').id;  

        } else {
            $('#player1-container').dialog({ modal: true, height: 480, width: 700 });
            $('#player1-container').dialog('option', 'title', _fileName);
            media_id = document.getElementById('player1-container').querySelector('.mejs__container').id;  

        }
        mejs.i18n.language('ru');                 
        var audio_player = mejs.players[media_id];
        audio_player.setSrc(_url);
        audio_player.load();
        audio_player.play();
    } else {
        $("#Waiting").show();
        $.ajax({
            url: webService,
            data: _data,
            type: "POST",
            dataType: 'json',
            success: function (data, status, xhr) {                
                    if (undefined === window.navigator.msSaveOrOpenBlob) {                                  
                        var link = document.createElement("a");
                        link.download = data.FileName;
                        link.href = 'data:text/csv;charset=utf-8;base64,' + data.Content;
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                        delete link;
                    } else {
                        var blob = new Blob([convertDataURIToBinary(data.Content)]);
                        window.navigator.msSaveOrOpenBlob(blob, data.FileName);
                    }                
                $("#Waiting").hide();
            },
            error: function (error) {
                $("#Waiting").hide();
                alert(error.responseText);
            }
        });
    }
}

function convertDataURIToBinary(dataURI) {
    var raw = window.atob(dataURI);
    var rawLength = raw.length;
    var array = new Uint8Array(new ArrayBuffer(rawLength));
    for (i = 0; i < rawLength; i++) {
        array[i] = raw.charCodeAt(i);
    }
    return array;
}