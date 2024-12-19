$(function () {
    $('.addToBasket').on("click", function () {

        $.ajax({
            method: "POST",
            url: "/basket/addproduct",
            data: {
                productId: $(this).data("id")
            },
            success: function (response) {
                alert(response)
            },
            error: function (response) {
                alert(response.responseText)
            }

        })
    })

    $('.increaseBtn').on("click", function () {
        var btn =$(this)
        $.ajax({
            method: "POST",
            url: "/basket/IncreaseCount",
            data: {
                basketProductId: $(this).data("id")
            },
            success: function (response) {
                btn.parent().siblings(".quantity")[0].innerText = response.quantity
                btn.parent().siblings(".productTotalPrice")[0].innerText = response.productTotalPrice.toFixed(2)+"  AZN"
                   $ ('#totalPrice')[0].innerText = response.totalPrice.toFixed(2)+"  AZN"
            },
            error: function (response) {
                alert(response.responseText)
            }

        })
    })
    $('.decreaseBtn').on("click", function () {
        var btn =$(this)
        $.ajax({
            method: "POST",
            url: "/basket/DecreaseCount",
            data: {
                basketProductId: $(this).data("id")
            },
            success: function (response) {
                btn.parent().siblings(".quantity")[0].innerText=response.quantity
                btn.parent().siblings(".productTotalPrice")[0].innerText = response.productTotalPrice.toFixed() + "   AZN"
               
                $('#totalPrice')[0].innerText = response.totalPrice.toFixed(2) + "  AZN"

            },
            error: function (response) {
                alert(response.responseText)
            }

        })
    })

    $('.deleteBtn').on("click", function () {
        var btn =$(this)
        $.ajax({
            method: "POST",
            url: "/basket/DeleteProduct",
            data: {
                basketProductId: $(this).data("id")
            },
            success: function (response) {
                $('#totalPrice')[0].innerText = response.totalPrice.toFixed(2) + "  AZN"
               btn.parent().parent().remove() 
            },
            error: function (response) {
                alert(response.responseText)
            }

        })
    })
})