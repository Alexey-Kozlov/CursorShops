function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function getStringFromDate(dt) {
    var year = dt.getFullYear();
    var month = dt.getMonth() < 9 ? '0' + (dt.getMonth() + 1) : (dt.getMonth() + 1);
    var day = dt.getDate() < 10 ? '0' + dt.getDate() : dt.getDate();
    var hour = dt.getHours() < 10 ? '0' + dt.getHours() : dt.getHours();
    var min = dt.getMinutes() < 10 ? '0' + dt.getMinutes() : dt.getMinutes();
    return day + '.' + month + '.' + year + ' ' + hour + ':' + min;
}

