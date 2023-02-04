var count_stocks = 0;
var ticker_find = null;
$('#PortfolioItems').hide();
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

var percentage = $("#percentage").text();
//console.log("percentage: " + parseFloat(percentage.replace(",",".")).toFixed(2));
$("#percentage").text(parseFloat(percentage.replace(",", ".")).toFixed(2));
var price = $("#price").text();
//console.log("percentage: " + parseFloat(percentage.replace(",",".")).toFixed(2));
if (parseFloat(price.replace(",", ".")) > 1) {
    $("#price").text(parseFloat(price.replace(",", ".")).toFixed(2));
}

function show_modal() {
    $('#CreateModal').fadeIn(300);
    $('#CreateModal').modal('show');
}
function hide_modal() {
    $('#CreateModal').fadeOut(300);
    $('#CreateModalItem').fadeOut(300);
    setTimeout(close, 300);
}

function close() {
    $('#CreateModal').modal('hide');
    $('#CreateModalItem').modal('hide');
    console.log("log");
}

function ShowPortfolio(pId) {
    console.log(pId);
    var cnt = 1;
    $('#Id_Portfolio').val(pId);
    $('#PortfolioItems tbody tr').remove();
    $.ajax({
        url: "/Home/PortfolioItems/" + pId,
        context: document.body
    }).done(function (resp) {
        console.log(resp);
        if (resp.length > 0) {
            console.log("#Id_Portfolio:" + $("#Id_Portfolio").text());
            $('#PortfolioItems').show(100);
        }
        else {
            $('#PortfolioItems').hide(100);
        }
        
        resp.forEach(element => {
            if (element['currency'] == "rub") {
                result_currency = "₽";

            }
            if (element['currency'] == "usd") {
                result_currency = "$";

            }
            if (element['currency'] == "eur") {
                result_currency = "€";
            }
            var percent = roundNumber((((element['cuurentPrice'] / element['price']) * 100) - 100), 2);
            var img = '';
            if (percent < 0) {
                img = '<img title="' + element['nameInstrument'] + '" src="/img/other/icon-down.jpg" height="10" width="10" />';
            }
            else {
                img = '<img title="' + element['nameInstrument'] + '" src="/img/other/icon-up.jpg" height="10" width="10" />';
            }
            $('#PortfolioItems tbody').append('<tr><td class="first_column_portfolioitems"><a title="' + element['nameInstrument'] + '" href="/Home/Detail/' + element['ticker'] + '/">' + cnt + '</a></td><td class="img-portfolioitems"><a title="' + element['nameInstrument'] + '" href="/Home/Detail/' + element['ticker'] + '/"><img title="' + element['nameInstrument'] + '" src="/img/stocks/' + element['ticker'] + '.png" onerror="this.onerror=null;this.src=\'/img/stocks/question.jpg\';" height="50" width="50"/></a></td><td><a title="' + element['nameInstrument'] + '" href="/Home/Detail/' + element['ticker'] + '/">' + element['nameInstrument'] + '</a></td><td><a title="' + element['nameInstrument'] + '" href="/Home/Detail/' + element['ticker'] + '/">' + element['count'] + '</a></td><td><a title="' + element['nameInstrument'] + '" href="/Home/Detail/' + element['ticker'] + '/">' + roundNumber(element['price'], 3) + ' ' + result_currency + '</a></td><td><a title="' + element['nameInstrument'] + '" href="/Home/Detail/' + element['ticker'] + '/">' + element['cuurentPrice'] + ' ' + result_currency + '</a></td><td>' + percent + ' % '+img+'</td></tr>');
            cnt++;
            
        });
        //$("#Id_Portfolio").text(pId);
        
    });
    
}
function roundNumber(number, digits) {
    var multiple = Math.pow(10, digits);
    var rndedNum = Math.round(number * multiple) / multiple;
    return rndedNum;
}
function show_modalItem() {
    $('#CreateModalItem').fadeIn(300);
    $('#CreateModalItem').modal('show');
}
$('#form1Items').keyup(function (e) {
    //console.log(e.which);
    var ticker = $(this).val();
    $.ajax({
        url: "/api/values/Search/" + ticker,
        context: document.body
    }).done(function (result) {
        $("#searched-elemetsItems").empty();
        count_stocks = result["sharesTable"].length;
        ticker_find = result["sharesTable"][0]["ticker"];

        if (result["sharesTable"].length > 0) {
            $("#searched-elemetsItems").append('<h6 class="dropdown-header">Stocks</h6>');
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
                //console.log(result_currency);
                $("#searched-elemetsItems").append('<li class="d-inline-flex w-100"><a class="dropdown-item pt-2 pb-2" href="#"  onclick="load_information(' + result["sharesTable"][index]["id"] + ',' + result["sharesTable"][index]["instrumentType"] + ',\'' + result["sharesTable"][index]["name"] + '\',\'' + result["sharesTable"][index]["ticker"] + '\',\'' + result["sharesTable"][index]["currency"] + '\',\'' + result["sharesTable"][index]["figi"] + '\')"><img class="rounded-circle" title="' + result["sharesTable"][index]["name"] + '" src="/img/stocks/' + result["sharesTable"][index]["ticker"] + '.png" onerror="this.onerror=null;this.src=\'/img/stocks/question.jpg\';" height="25" width="25"/> ' + result["sharesTable"][index]["name"] + ' (' + result["sharesTable"][index]["ticker"] + ') | ' + result_currency + ' ' + result["sharesTable"][index]["price"] + ' </a></li>');
                console.log(result["sharesTable"][index]["id"] + " | " + result["sharesTable"][index]["name"] + " | " + result["sharesTable"][index]["ticker"] + " | " + result["sharesTable"][index]["currency"] + " | " + result["sharesTable"][index]["price"] + " | " + result["sharesTable"][index]["instrumentType"]);
                //Id_Instrument = result["sharesTable"][index]["id"];
                //Instrument_Type = result["sharesTable"][index]["instrumentType"];
                //nameInstrument = result["sharesTable"][index]["name"];
                
            }
        }
    });
});
function load_information(Id_Instrument, Instrument_Type, nameInstrument,ticker,currency,figi) {
    console.log(nameInstrument, Instrument_Type, Id_Instrument)
    $('#form1Items').val(nameInstrument);
    $('#Instrument_Type').val(Instrument_Type);
    $('#Id_Instrument').val(Id_Instrument);
    $('#NameInstrument').val(nameInstrument);
    $('#Ticker').val(ticker);
    $('#Currency').val(currency);
    $('#Figi').val(figi);
}
$('#search-autocompleteItems').keypress(
    function (event) {
        if (event.which == '13') {
            event.preventDefault();
            if (count_stocks == 1) {
                /*location.href = "/Home/Detail/" + ticker_find;*/
                $('#form1Items').val(nameInstrument);
                $('#Instrument_Type').val(Instrument_Type);
                $('#Id_Instrument').val(Id_Instrument);
            }
        }
    });
//$("#search-autocompleteItems").append('<h6 class="dropdown-header">Type to find</h6>');
