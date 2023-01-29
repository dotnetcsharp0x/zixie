console.log('table')

cnt = 1;

function load_crypto() {
    $('#table-crypto tbody tr').remove();
    $.ajax({
        url: "/Home/Load_Crypto/"+cnt,
        context: document.body
    }).done(function (resp) {
        console.log(resp);
        resp.forEach(element => {
            $('#table-crypto tbody').append('<tr role="button" class="align-middle" onclick=\"window.location.href = \'/Home/Crypto/' + element['symbol'] + '\';\"><td scope="row" class="first_row" onclick=\"window.location.href = \'/Home/Crypto/' + element['symbol'] + '\';\"><a href="/Home/Crypto/' + element['symbol'] + '"><span class="d-inline-block star-black" tabindex="0" data-bs-toggle="popover" data-bs-trigger="hover" data-bs-placement="top" data-bs-content="Add to watchlist"><i class="fa fa-star-o"></i></span></a></td><td scope="row" class="first_row"  onclick=\"window.location.href = \'/Home/Crypto/' + element['symbol'] + '\';\" ><a href="/Home/Crypto/' + element['symbol'] + '"><img title="' + element['name'] + '" src="/img/crypto/' + element['symbol'] + '.png" onerror="this.onerror=null;this.src=\'/img/stocks/question.jpg\';" height="50" width="50" /></a></td><td scope="row"><span class="name"  onclick=\"window.location.href = \'/Home/Crypto/' + element['symbol'] + '\';\" ><a href="/Home/Crypto/' + element['symbol'] + '">' + element['name'] + '</span><span class="ticker">' + element['symbol'] + '</span></a></td><td scope="row"  onclick=\"window.location.href = \'/Home/Crypto/' + element['symbol'] + '\';\" ><a href="/Home/Crypto/' + element['symbol'] + '">$' + element['price'].toFixed(3) + '</a></td></tr>');
        });
    });
}

