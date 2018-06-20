function TaskAction(_action) {    
    $("#TaskAction").val(_action);
    switch (_action) {
        case "Save":
            $("#Waiting").show();
            $("#RespName").val(SelUsers.options[SelUsers.selectedIndex].text);
            if (AddTask) {
                //новое задание в Горячей Линии
                NewTaskAddFiles();
            } else {
                //редактирование задания
                SendTaskUploadFiles('SubmitForm();');
            }
            break;
        case "Completed":
            $("#Waiting").show();
            SendTaskUploadFiles('SubmitForm();');
            break;
        case "Cancel":
        case "Return":
            $("#Waiting").show();
            SendTaskUploadFiles('SubmitForm();');
            break;
        case "Print":
            window.onbeforeunload = null;
            window.open('/HotLine/PrintTask?ID=' + $("#ID").val() +
                '&SiteID=' + $("#SiteID").val() +
                '&WebID=' + $("#WebID").val() +
                '&ListID=' + $("#ListID").val(), '_blank', 'scrollbars= yes, resizable = yes, width = 800, height = 800');
            break;
        case "AttachFile":
            ShowAttachmentDialog();
            break;
    }
}
function SubmitForm() {
    if ($("#TaskAction").val() == "Cancel" || $("#TaskAction").val() == "Return") {
        if ($('#EditForm').jqxValidator('validate')) {
            document.forms["EditForm"].submit();
        } else {
            $("#Waiting").hide();
            return;
        }
    } else {
        document.forms["EditForm"].submit();
    }
}
function SubmitAddTaskForm(ReturnFiles) {    
    $("#JsonAttachedFiles").val(JSON.stringify(ReturnFiles));
    $("#RespName").val($("#SelUsers option:selected").text());
    if ($("#AddForm").valid()) {
        document.forms["AddForm"].submit();
    } else {
        $("#Waiting").hide();
    }
}