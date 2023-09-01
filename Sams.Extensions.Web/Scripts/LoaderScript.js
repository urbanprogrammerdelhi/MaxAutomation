var showLoader = function (form) {
    $("<div id='DvLoader' />").css({
        'position': 'fixed',
        'left': 0,
        'right': 0,
        'bottom': 0,
        'top': 0,
        'background': '#0020ff36',
        'z-index': '99',
        'text-align': 'center'
    }).appendTo($("body"))
        .append(
            $("<img />")
            .attr("src", "https://ifm360.in/SAMSExtension/Images/Loader.gif")
        );
}
function HideLoader() {
    debugger;
    var loader = document.getElementById("DvLoader");
    loader.style.display = 'none';
}