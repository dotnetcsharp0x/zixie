var count_stocks = 0;
var ticker_find = null;
function AddToWatchlist(ticker) {
    $.ajax({
        url: "/Home/AddToWatchlist/" + ticker,
        context: document.body
    }).done(function () {
        console.log('OK');
        $('#add_star').addClass('d-none');
        $('#delete_star').removeClass('d-none');
    });
}

function DeleteFromWatchlist(ticker) {
    $.ajax({
        url: "/Home/DeleteFromWatchlist/" + ticker,
        context: document.body
    }).done(function () {
        console.log('OK');
        $('#delete_star').addClass('d-none');
        $('#add_star').removeClass('d-none');
    });
}
function Toggle_Watchlist(cl, ticker, i) {
    console.log(cl + ' - ' + ticker + ' - ' + i);
    if (cl > 0) {
        $.ajax({
            url: "/Home/DeleteFromWatchlist/" + ticker,
            context: document.body
        }).done(function () {
            console.log('OK delete');
            $('#toggle_star_' + i).find('.star-yellow').addClass('d-none');
            $('#toggle_star_' + i).find('.star-black').removeClass('d-none');
            $('#toggle_star_' + i).off('click');
            $('#toggle_star_' + i).attr("onclick", "Toggle_Watchlist('" + 0 + "','" + ticker + "','" + i + "')");
        });
    }
    else {
        $.ajax({
            url: "/Home/AddToWatchlist/" + ticker,
            context: document.body
        }).done(function () {
            console.log('OK add');
            $('#toggle_star_' + i).find('.star-yellow').removeClass('d-none');
            $('#toggle_star_' + i).find('.star-black').addClass('d-none');
            $('#toggle_star_' + i).off('click');
            $('#toggle_star_' + i).attr("onclick", "Toggle_Watchlist('" + 1 + "','" + ticker + "','" + i + "')");
        });
    }
}
function imgError(image) {
    image.onerror = "";
    image.src = "/img/stocks/ddd.jpg";
    return true;
}

$('#form1').keyup(function (e) {
    console.log(e.which);
    var ticker = $(this).val();
    $.ajax({
        url: "/api/values/Search/" + ticker,
        context: document.body
    }).done(function (result) {
        $("#searched-elemets").empty();
        count_stocks = result["sharesTable"].length;
        ticker_find = result["sharesTable"][0]["ticker"];

        if (result["sharesTable"].length > 0) {
            $("#searched-elemets").append('<h6 class="dropdown-header">Stocks</h6>');
        }
        if (result["sharesTable"].length > 0) {
            for (let index = 0; index < result["sharesTable"].length; ++index) {
                var result_currency = "";

                if (result["sharesTable"][index]["currency"] == "rub") {
                    result_currency = "₽";

                }
                if (result["sharesTable"][index]["currency"] == "usd") {
                    result_currency = "$";

                }
                if (result["sharesTable"][index]["currency"] == "eur") {
                    result_currency = "€";
                }
                console.log(result_currency);
                $("#searched-elemets").append('<li class="d-inline-flex w-100"><a class="dropdown-item pt-2 pb-2" href="/Home/Detail/' + result["sharesTable"][index]["ticker"] + '"><img class="rounded-circle" title="' + result["sharesTable"][index]["name"] + '" src="/img/stocks/' + result["sharesTable"][index]["ticker"] + '.png" onerror="this.onerror=null;this.src=\'/img/stocks/question.jpg\';" height="25" width="25"/> ' + result["sharesTable"][index]["name"] + ' (' + result["sharesTable"][index]["ticker"] + ') | ' + result_currency + ' ' + result["sharesTable"][index]["price"] + '</a></li>');
                console.log(result["sharesTable"][index]["name"] + " | " + result["sharesTable"][index]["ticker"] + " | " + result["sharesTable"][index]["currency"] + " | " + result["sharesTable"][index]["price"]);
            }
        }
    });
});
$('#search-autocomplete').keypress(
    function (event) {
        if (event.which == '13') {
            event.preventDefault();
            if (count_stocks == 1) {
                location.href = "/Home/Detail/" + ticker_find;
            }
        }
    });
$("#searched-elemets").append('<h6 class="dropdown-header">Type to find</h6>');
