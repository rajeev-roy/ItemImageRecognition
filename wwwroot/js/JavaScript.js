function validate(pics) {
    document.getElementById("p1").innerHTML = '';
    var files = document.getElementById("pics").files;
    if (files.length == 0) {
        alert('please select files');
    }
    if (files.length > 5) {
        alert('you can select upto 5 files');
        document.getElementById("pics").value = '';
        return;
    }
    document.getElementById("p1").innerHTML = files.length + 'files selected';
    ValidateEx();
    return;
}
var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
function ValidateEx() {
    var arrInputs = document.getElementsByTagName("input");
    for (var i = 0; i < arrInputs.length; i++) {
        var oInput = arrInputs[i];
        if (oInput.type == "file") {
            var sFileName = oInput.value;
            if (sFileName.length > 0) {
                var blnValid = false;
                for (var j = 0; j < _validFileExtensions.length; j++) {
                    var sCurExtension = _validFileExtensions[j];
                    if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                        blnValid = true;
                        break;
                    }
                }

                if (!blnValid) {
                    alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
                    document.getElementById("pics").value = '';
                    return false;
                }
            }
        }
    }

    return true;
}