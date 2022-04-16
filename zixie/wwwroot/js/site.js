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
            $('#toggle_star_' + i).attr("onclick", "Toggle_Watchlist('" + 0 + "','" + ticker + "','" + i +"')");
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
//document.getElementById('#add_star').style.display = 'block';
//document.getElementById('#delete_star').style.display = 'block';
