﻿function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}
function focusInput(id) {
    document.getElementById(id).focus();
}