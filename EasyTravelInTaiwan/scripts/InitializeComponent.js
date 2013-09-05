/* 
* Initialize component for all page
* @Scripts.Render("~/js")
* @RenderSection("Scripts", required: false)
*/
$(function () {
    $('.selectpicker').selectpicker();
    $("#box").niceScroll({ autohidemode: true })
});