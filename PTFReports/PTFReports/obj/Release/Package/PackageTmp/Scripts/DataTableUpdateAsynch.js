$(function () {
    $.getJSON("/Administration/Branches", null, function (d) {
        $("body").append(d.Data);

        var footer = createFooter(d.Count);

        $("#DataTable tfoot a").live("click", function (e) {
            e.preventDefault();
            var data = {
                page: $(this).text()
            };

            $.getJSON("/Administration/Branches", data, function (html) {
                $("#DataTable").remove();
                $("body").append(html.Data);
                $("#DataTable thead").after(footer);
            });
        });
    });
});

function createFooter(d) {
    var rowsPerPage = 5;
    var footer = "<tfoot>";

    for (var i = 1; i < (d + 1); i++) {
        footer = footer + "<a href=#" + i + "</a>&nbsp;";
    }

    footer = footer + "</tfoot>";
    $("#DataTable thead").after(footer);
    return footer;
}