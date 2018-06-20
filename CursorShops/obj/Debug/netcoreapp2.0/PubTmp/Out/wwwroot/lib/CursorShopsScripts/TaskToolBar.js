function TaskAction(_action) {    
    $("#TaskAction").val(_action);
    switch (_action) {
        case "Save":
        case "Completed":
        case "Question":
            $("#Waiting").show();
            SendTaskUploadFiles('SubmitForm();');
            break;
        case "Print":
            window.onbeforeunload = null;
            getBinaryFile('', 'задание.pdf', 'GetWordContext.asmx/GetWordBinary', $("#SiteID").val(), $("#WebID").val(), $("#ID").val(), $("#ListID").val(), $("#CurrentUserName").val());
            break;
        case "AttachFile":
            ShowAttachmentDialog();
            break;
    }   
}
function SubmitForm() {
    if ($("#TaskAction").val() == "Question" || JSON.parse($("#CommentMustBe").val().toLowerCase()) == true) {
        if ($('#EditForm').jqxValidator('validate')) {
            document.forms["EditForm"].submit();
        } else {
            $("#Waiting").hide();
            return;
        }
    } else 
        document.forms["EditForm"].submit();    
}