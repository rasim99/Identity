$(function () {
    let list = $("#collapseTwo");
    let btns = list.find("li");
    let productRow = $(".productRow");
    btns.each(function () {
        let categoryId = $(this).find("a").attr("categoryId");
        $(this).on("click", function () {

            $.ajax({
                url: "/Shop/GetProducts",
                method: "GET",
                data: {
                    categoryId: categoryId
                },
                contentType: "application/json",
                success: function (resp) {
                     productRow.html(resp)
                }
            });
            
        });

    });
});