function TaskAction(_action, _modelName) {
    $("#TaskAction").val(_action);
    switch (_action) {
        case "Save":
            $("#Waiting").show();
            if ($('#EditForm').jqxValidator('validate')) {
                document.forms["EditForm"].submit();
            } else {
                $("#Waiting").hide();
                return;
            }
            break;
    }   
}
