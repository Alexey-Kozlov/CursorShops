var _attachmentsRemove;
var attachmentsRemoveId;
var result;
var uploadControlPlaceholder;
var attachedFilesList;
var AllSendBytes = 0;
var CurrentSendBytes = 0;
var files;
var filesCounter = 0;
var _waitScreen = null;
var CtrlFolderUrlID;
var progressbarCtrl;
var FileuploadString = "fileupload";
var FileUploadIndex = 0;
var FileUploadLocalFileCount = 0;
var immediatlyFileSend;

$(document).ready(function () {
    immediatlyFileSend = typeof SetImmediatlyFileSend !== 'undefined' ? SetImmediatlyFileSend : false;
    //подгружаем дополнительный скрипт
    //var script = document.createElement("script"); 
    //script.src = "/lib/CursorShopsScripts/GetFileContent.js"
    //document.head.appendChild(script); 

    attachedFilesList = [];
    _urlPath = "";
    attachmentsRemoveId = 'attachmentsToBeRemovedFromServer';
    result = '';

    result += '<table border="0" cellpadding="0" cellspacing="0" id="GrididAttachmentsTable2">';
    var allAttachments = 0;
    result += '</table>';
    //сообщения для ошибок прикрепленных файлов
    result += "<div id='es_task_html_strerror1' title='Ошибка' style='display:none'><p style='text-align:center;'>Такой файл уже прикреплен! Выберите файл с другим наименованием.</p></div>";
    result += "<div id='es_task_html_strerror2' title='Ошибка' style='display:none'><p style='text-align:center;'>Невозможно прикрепление к заданию файла формата XLSX размером больше 10 Мб! Сохраните файл в формате XLS и снова прикрепите к заданию.</p></div>";
    result += "<div id='es_task_html_strerror3' title='Предупреждение' style='display:none'><p style='text-align:center;'>За один сеанс редактирования задания невозможно прикрепить к заданию файлы, общим объемом превышающих 100 Мб.<br>Щелкните кнопку 'Сохранить' или 'Задание выполнено', затем снова откройте задание на редактирование и прикрепите оставшиеся файлы.</p></div>";
    uploadControlPlaceholder = document.getElementById('AttachmentFileUploadDiv');

    uploadControlPlaceholder.innerHTML = ES_TaskBuildAttachmentsUploadControl();
    var dropZone = $("#ES_DropZone");
    dropZone.bind('dragover', handleDragOver);
    dropZone.bind('drop', handleDragFileSelect);
    dropZone.bind('dragenter', handleDragEnter);
    dropZone.bind('dragleave', handleDragLeave);

    document.getElementById('FileUploadAttachmentPlace').innerHTML = result;
    $("#FileUploadAttachmentPlace").hide();
    progressbarCtrl = $("#progressbar");
    progressLabel = $(".progress-label");
    progressbarCtrl.progressbar({
        value: false,
        change: function () {
            progressLabel.text(progressbarCtrl.progressbar("value") + "%");
        }
    });
    //отображаем уже прикрепленные файлы
    if (!immediatlyFileSend) {
        $.ajax({
            type: "POST",
            dataType: "json",
            data: { TaskID: $('#ID').val(), UserName: $('#CurrentUserName').val(), WebID: $('#WebID').val(), ListID: $('#ListID').val(), SiteID: $('#SiteID').val() },
            url: 'EsMVCWebUtils.asmx/GetAttachedFiles',
            success: function (data, status, xhr) {
                for (var i = 0; i < data.length; i++) {
                    var f = {};
                    f.name = data[i].FileName;
                    f.url = data[i].Url;
                    f.size = 0;
                    f.allowEdit = data[i].AllowEdit;
                    attachedFilesList.push(f);
                    ES_ShowFilesTable(f);
                }
                if (!immediatlyFileSend && data.length == 0) {
                    $('#AttachedFilesList').hide();
                }
            },
            error: function (error) {
                alert(error.responseText);
            }
        });
    }
});

function ShowAttachmentDialog(urlctrl_id) {
    CtrlFolderUrlID = urlctrl_id;
    (document.getElementById("Part1")).style.display = "none";
    (document.getElementById("GridpartAttachment")).style.display = "block";
    if (window.frameElement != null && typeof window.frameElement.autoSize == "function" && TypeofFullName("window.frameElement.autoSize") == "function")
        window.frameElement.autoSize();
    (GetAttachElement(FileuploadString + String(FileUploadIndex))).focus();
}

function ES_TaskBuildAttachmentsUploadControl() {
    var upload = '';
    upload += '<input type="hidden" name="' + attachmentsRemoveId + '"/>';
    upload += '<span id="GridpartAttachment" style="display:none">';
    upload += '<table cellspacing="0" cellpadding="0" border="0" width="100%">';
    upload += '<tbody><tr>';
    upload += '<td style="font-size:18px;width:160px;padding:10px;">Выберите файл:</td>';
    upload += '<td id="attachmentsOnClient" style="width:300px;"><input type="file" name="fileupload0" id="onetidIOFile" multiple style="width:300px;height:40px;" class="form-control" /></td>';
    upload += '<td style="text-align:left;padding-left:10px;"><button id="attachOKbutton" class="btn ControlBg" type="button" onclick="InputSendFile()">Ок</button><span>&nbsp;&nbsp;</span>';
    upload += '<button id="attachCancelButton" class="btn ControlBg" type="button" onclick="GridCancelAttach()">Отмена</button></td>';
    upload += '</tr><tr>';  
    upload += '<td style="padding:10px;" colspan="3"><div id="ES_DropZone" class="ES_dropDef">Или перетащите файлы сюда:</div></td>';
    upload += '</tr></tbody></table></span>';
    return upload;

}

function GridShowPart1() {
    if (document.getElementById("GridpartAttachment")) {
        (document.getElementById("GridpartAttachment")).style.display = "none";
        (document.getElementById("Part1")).style.display = "block";
        if (window.frameElement != null && typeof window.frameElement.autoSize == "function")
            window.frameElement.autoSize();
        if (window.frameElement != null && typeof window.frameElement.SetFirstFocus == "function")
            window.frameElement.SetFirstFocus(true);
    }
}

function GridCancelAttach() {
    var fileID = FileuploadString + String(FileUploadIndex);
    var fileInput = document.getElementById(fileID);
    if (fileInput == null)
        fileInput = (document.getElementsByName(fileID))[0];
    if (fileInput == null)
        return;
    var filename = fileInput.value;

    if (Boolean(filename)) {
        fileInput.outerHTML = "<input type='file' name=" + fileID + ">";
    }
    GridShowPart1();
    return false;
}

function ES_TaskHasAttachmentsValueChanged() {
    if (typeof FileUploadLocalFileCount != "undefined" && FileUploadLocalFileCount > 0)
        return true;
    if (_attachmentsRemove == null)
        _attachmentsRemove = (document.getElementsByName(attachmentsRemoveId))[0];
    if (_attachmentsRemove != null && _attachmentsRemove.value != '')
        return true;
    return false;
}

function handleDragFileSelect(evt) {
    evt.stopPropagation();
    if (evt.preventDefault) {
        evt.preventDefault();
    }
    var files = evt.originalEvent.dataTransfer.files;
    for (var i = 0, f; f = files[i]; i++) {
        if (attachedFilesList.length == 0) {
            attachedFilesList.push(f);
            ES_ShowFilesTable(f);
        } else {
            var IsFilePresent = false;
            for (var j = 0; j < attachedFilesList.length; j++) {
                if (attachedFilesList[j].name == f.name) {
                    IsFilePresent = true;
                    break;
                }
            }
            if (!IsFilePresent) {
                attachedFilesList.push(f);
                ES_ShowFilesTable(f);
            }
        }
    }
    ES_removeClass(document.getElementById('ES_DropZone'), 'ES_over');
    ES_TaskOkAttach();
    if (immediatlyFileSend) {
        //немедленная передача выбранных файлов
        SendTaskUploadFiles('FilesAdded();');
    }
}

function handleDragOver(evt) {
    if (evt.preventDefault) {
        evt.preventDefault();
    }
    evt.originalEvent.dataTransfer.dropEffect = 'copy';
}

function handleDragEnter(evt) {
    ES_addClass(this, 'ES_over');
}
function handleDragLeave(evt) {
    ES_removeClass(this, 'ES_over');
}
function ES_addClass(o, c) {
    var re = new RegExp("(^|\\s)" + c + "(\\s|$)", "g")
    if (re.test(o.className)) return
    o.className = (o.className + " " + c).replace(/\s+/g, " ").replace(/(^ | $)/g, "")
}
function ES_removeClass(o, c) {
    var re = new RegExp("(^|\\s)" + c + "(\\s|$)", "g")
    o.className = o.className.replace(re, "$1").replace(/\s+/g, " ").replace(/(^ | $)/g, "")
} 
function InputSendFile() {
    var files = document.getElementsByName(FileuploadString + String(FileUploadIndex))[0].files;
    for (var i = 0, f; f = files[i]; i++) {
        if (attachedFilesList.length == 0) {
            attachedFilesList.push(f);
            ES_ShowFilesTable(f);
        } else {
            var IsFilePresent = false;
            for (var j = 0; j < attachedFilesList.length; j++) {
                if (attachedFilesList[j].name == f.name) {
                    IsFilePresent = true;
                    break;
                }
            }
            if (!IsFilePresent) {
                attachedFilesList.push(f);
                ES_ShowFilesTable(f);
            }
        }
    }

    ES_TaskOkAttach();
    if (immediatlyFileSend) {
        //немедленная передача выбранных файлов
        SendTaskUploadFiles('FilesAdded();');
    }
}

function ES_ShowFilesTable(f) {
    if (immediatlyFileSend)
        return;
    if (!f.name)
        return;
    $('#AttachedFilesList').show();
    var tableFiles = document.getElementById('GrididAttachmentsTable2');
    var newRow = tableFiles.insertRow(0);
    var newCell = newRow.insertCell(0);
    newCell.setAttribute('style', 'padding:2px 5px 2px 5px');
    var _name = f.name;
    var _serviceUrl = 'EsMVCWebUtils.asmx/GetBytesFromUrl';
    if (_name.length > 30)
        _name = _name.substring(0, 29) + '...';
    if (f.size == 0) {
        var newLink = document.createElement('a');
        var linkText = document.createTextNode(f.name);
        newLink.appendChild(linkText);
        newLink.name = 'a_' + f.name;
        newLink.text = f.name;
        newLink.href = "#";
        newLink.setAttribute('onclick', 'getBinaryFile("' + f.url + '","' + f.name + '","' + _serviceUrl + '");');
        newCell.appendChild(newLink);
    } else {
        var newText = document.createTextNode(_name);
        newCell.appendChild(newText);
    }    
    var newCell2 = newRow.insertCell(1);
    newCell2.setAttribute('style', 'padding:2px 5px 2px 5px');
    var newText2;
    if (f.size == -1) {
        var newText2 = document.createTextNode('Ссылка');
    } else {
        var fileSize = Math.round(f.size / 1000);
        if (fileSize == 0)
            fileSize = 1;
        if(f.size != 0)
            newText2 = document.createTextNode(String(fileSize) + ' Kb');
        else
            newText2 = document.createTextNode('');
    }
    newCell2.appendChild(newText2);
    var newCell3 = newRow.insertCell(2);
    newCell3.setAttribute('style', 'padding:2px 5px 2px 5px');
    if (f.allowEdit) {
        var newImg = document.createElement('IMG');
        newImg.src = '/lib/images/rect.gif';
        newCell3.appendChild(newImg);
    }
    var newCell4 = newRow.insertCell(3);
    newCell4.setAttribute('style', 'padding:2px 5px 2px 5px');
    if (f.allowEdit) {
        var newElem = document.createElement('A');
        newElem.name = 'DelFile_' + f.name;
        newElem.text = 'Удалить';
        newElem.href = '#';
        newElem.setAttribute('onclick', 'ES_DeleteFile("' + f.name + '")');
        newCell4.appendChild(newElem);
    }
    //отобразить таблицу с выбранными файлами
    $("#FileUploadAttachmentPlace").show();
}


function ES_DeleteFile(fileName) {
    for (var i = 0; i < attachedFilesList.length; i++) {
        if (attachedFilesList[i].name == fileName) {
            attachedFilesList.splice(i, 1);
            break;
        }
    }
    var tableFiles = document.getElementById('GrididAttachmentsTable2');
    for (var i = 0; i < tableFiles.rows.length; i++) {
        if (tableFiles.rows[i].cells[0].textContent == fileName) {
            tableFiles.deleteRow(i);
        }
    }
    alert('Для подтверждения удаления файлов, щелкните кнопку "Сохранить" в панели инструментов.');
}



function ShowAllert(Message) {
    $(Message).dialog({ modal: true, buttons: { Ok: function () { $(this).dialog('close'); } } });
}


function ES_TaskOkAttach() {
    var fileID = FileuploadString + String(FileUploadIndex);
    var fileInput = GetAttachElement(fileID);
    var filename = TrimWhiteSpaces(fileInput.value);
    /*
    //проверка - если это XLSX файл и его размер > 10 Мб - запретить загрузку, т.к. такие файлы не поддерживаются WebApp.
    for (var i = 0; i < attachedFilesList.length; i++) {
        if (attachedFilesList[i].size > 10000000 && attachedFilesList[i].name.substring(attachedFilesList[i].name.lastIndexOf('.') + 1).toUpperCase() == 'XLSX') {
            ShowAllert('#es_task_html_strerror2');
            CancelAttach();
            return;
        }
    }
    //проверка - если общий размер прикрепленных за данный сеанс файлов превышает 100 Мб - выдать сообщение.    
    var AttachedFilesSizeCounter = 0;
    for (var i = 0; i < attachedFilesList.length; i++) {
        AttachedFilesSizeCounter = AttachedFilesSizeCounter + attachedFilesList[i].size;
    }

    if (AttachedFilesSizeCounter > 100000000) {
        ShowAllert('#es_task_html_strerror3');
        CancelAttach();
        return;
    }
    */
    ++FileUploadIndex;
    ++FileUploadLocalFileCount;
    var oAttachments = document.getElementById("attachmentsOnClient");
    var inputNode = document.createElement("input");
    LastFileUploadID = FileuploadString + String(FileUploadIndex);
    inputNode.tabIndex = 1;
    inputNode.type = "File";
    inputNode.className = "form-control";
    inputNode.title = 'Выбрать файл';
    inputNode.name = FileuploadString + String(FileUploadIndex);
    inputNode.id = FileuploadString + String(FileUploadIndex);
    inputNode.setAttribute('style','width:300px;height:40px;');
    inputNode.setAttribute('multiple', '');    
    oAttachments.appendChild(inputNode);
    var aForm = fileInput.form;
    if (!aForm)
        aForm = document.createElement("form");
    aForm.encoding = 'multipart/form-data';
    fileInput.style.display = "none";
    GridShowPart1();
    if (document.getElementById("Status")) {
        if ($("#Status").val() == "В работе") {
            alert('Для прикрепления файлов, не забудьте щелкнуть кнопку "Сохранить" в панели инструментов!');
        }
    }
    
}

function SendTaskUploadFilesR(f, ReturnFunction) {
    if (_waitScreen != null) {
        _waitScreen.close();
    }

    if (f && f.size != 0) {

        $('#FileUploadDialog').dialog({ modal: true, height: 250, width: 550 });

        if (filesCounter == 0) {
            $('#UploadTaskFileName')[0].innerHTML = '<b>Сохраняем файл - ' + f.name + ' ...</b>';
        } else {
            $('#UploadTaskFileName')[0].innerHTML = '<b>Сохранили файл - ' + attachedFilesList[filesCounter - 1].name + '. Сохраняем файл - ' + f.name + ' ...</b>';
        }

        var r = new FileReader();
        r.onload = (function (theFile) {
            return function (e) {
                try {
                    $.ajax({
                        type: "POST",
                        data: {
                            FileName: theFile.name, Bytes: e.target.result.split("base64,")[1], TaskID: $('#ID').val(), Author: $('#CurrentUserName').val(),
                            WebID: $('#WebID').val(), ListID: $('#ListID').val(), SiteID: $('#SiteID').val(), NewAttachedFiles: $("#NewAttachedFiles").val()
                        },
                        url: 'EsMVCWebUtils.asmx/SendFile',
                        success: function (data, status, xhr) {
                            //в последующих передачах не обрабатываем удаление файлов
                            $("#NewAttachedFiles").val('');
                            CurrentSendBytes += theFile.size;
                            progress();
                            filesCounter++;
                            if (filesCounter < attachedFilesList.length) {
                                SendTaskUploadFilesR(attachedFilesList[filesCounter], ReturnFunction);
                            } else {
                                ClearResources();
                                eval(ReturnFunction);
                            }
                        },
                        error: function (error) {
                            ClearResources();
                            alert(error.responseText);
                        }
                    });
                }
                catch (err) {
                    alert('Возникла ошибка при передаче файлов - ' + err.name + ',' + err.message + '. Обновите экран клавишей F5, проверьте загруженные файлы и загрузите оставшиеся.')
                }
            };
        })(f);
        r.readAsDataURL(f);
    } else {
        //обработка записи в таблице файлов, где это уже имеющийся в БД файл
        try {
            $.ajax({
                type: "POST",
                data: {
                    FileName: '', Bytes: '', TaskID: $('#ID').val(), Author: $('#CurrentUserName').val(),
                    WebID: $('#WebID').val(), ListID: $('#ListID').val(), SiteID: $('#SiteID').val(), NewAttachedFiles: $("#NewAttachedFiles").val()
                },
                url: 'EsMVCWebUtils.asmx/SendFile',
                success: function (data, status, xhr) {
                    //в последующих передачах не обрабатываем удаление файлов
                    $("#NewAttachedFiles").val('');
                    progress();
                    filesCounter++;
                    if (filesCounter < attachedFilesList.length) {
                        SendTaskUploadFilesR(attachedFilesList[filesCounter], ReturnFunction);
                    } else {
                        ClearResources();
                        eval(ReturnFunction);
                    }
                },
                error: function (error) {
                    ClearResources();
                    alert(error.responseText);
                }
            });
        }
        catch (err) {
            alert('Возникла ошибка при передаче файлов - ' + err.name + ',' + err.message + '. Обновите экран клавишей F5, проверьте загруженные файлы и загрузите оставшиеся.')
        }
    }

}

function SendTaskUploadFiles(ReturnFunction) {
    for (var i = 0; i < attachedFilesList.length; i++) {
        if (attachedFilesList[i].size == -1) {
            continue;
        }
        AllSendBytes += attachedFilesList[i].size;
    }
    if (document.getElementById("NewAttachedFiles") != null) {
        var FilesNames = '';
        var tableFiles = document.getElementById('GrididAttachmentsTable2');
        for (var i = 0; i < tableFiles.rows.length; i++) {
            FilesNames += tableFiles.rows[i].cells[0].textContent + '#';
        }
        if (FilesNames == '')
            FilesNames = 'empty';
        $("#NewAttachedFiles").val(FilesNames);
    }       
    //переписываем attachedFilesList наоборот - чтобы передача шла сначала, а не с конца.
    attachedFilesList.reverse();
    SendTaskUploadFilesR(attachedFilesList[0], ReturnFunction);
}



function progress() {
    progressbarCtrl.progressbar("value", parseInt(CurrentSendBytes / AllSendBytes * 100));
}

function GetAttachElement(elem) {
    var ret = document.getElementById(elem);
    if (ret == null)
        ret = (document.getElementsByName(elem))[0];
    return ret;
}
function TrimWhiteSpaces(str) {
    var st;
    var end;

    str = str.toString();
    var len = str.length;
    var ch;

    for (st = 0; st < len; st++) {
        ch = str.charAt(st);
        if (ch != ' ' && ch != '\t' && ch != '\n' && ch != '\r' && ch != '\f')
            break;
    }
    if (st == len)
        return "";
    for (end = len - 1; end > st; end--) {
        ch = str.charAt(end);
        if (ch != ' ' && ch != '\t' && ch != '\n' && ch != '\r' && ch != '\f')
            break;
    }
    end++;
    return str.substring(st, end);
}


function NewTaskAddFiles() {
    //прикрепление файлов к новому заданию делаем без графического интерфейса отображения передачи файлов, т.к.
    //самого задания еще нет и все данные вместе с файлами передаются разом в общей форме.
    var ReturnFiles = new Array();
    var tableFiles = document.getElementById('GrididAttachmentsTable2');
    if (tableFiles.rows.length > 0) {
        for (var i = 0; i < attachedFilesList.length; i++) {
            if (attachedFilesList[i].size == 0) {
                continue;
            } else {
                NewTaskUploadFiles(attachedFilesList[i], ReturnFiles);
                break;
            }
        }        
    } else {
        return eval('SubmitAddTaskForm(new Array())');
    }
}

function NewTaskUploadFiles(f, ReturnFiles) {
    var r = new FileReader();
    r.onload = (function (theFile) {
        return function (e) {
            filesCounter++;
            var tmpFile = {};
            tmpFile.FileName = theFile.name;
            tmpFile.Content = e.target.result.split("base64,")[1];
            ReturnFiles.push(tmpFile);
            if (filesCounter < attachedFilesList.length) {
                NewTaskUploadFiles(attachedFilesList[filesCounter], ReturnFiles);
            } else {
                return eval('SubmitAddTaskForm(ReturnFiles)');
            }
        };
    })(f);
    r.readAsDataURL(f);
}

function ClearResources() {
    attachedFilesList = [];
    AllSendBytes = 0;
    filesCounter = 0;
}