function btnDescription(id, desc) {
    var localString = document.getElementById(id).innerHTML;
    var cutString = desc;
    cutString = cutString.substring(0, 30);
    if (localString.length > 30) {
        document.getElementById(id).innerHTML = cutString;
    }
    else {
        document.getElementById(id).innerHTML = desc;
    }
}


//const searchInput = document.getElementById('todosearch');
//const rows = document.querySelectorAll('tbody tr');

//searchInput.addEventListener('keyup', function (event) {
//    const q = event.target.value.toLowerCase();
//    rows.forEach(row => {
//        row.querySelector('td').textContent.toLowerCase().startsWith(q) ? row.style.display = '' : row.style.display = "none";
//    });
//});


function change( id) {

    document.getElementById(id).style.color = "#0db2f2";

}

function changeTo(id) {

    document.getElementById(id).style.color = "black";

}


$(() => {
    getUsers();


    $('#todoSearch').on('keyup', () => {
        getUsers();
    });

});


function getUsers()
{
    $.ajax({
        url: '/Home/SearchUsers?',
        dataType: 'html',
        method: 'GET',
        data: { searchText: $('#todoSearch').val(), dateSearch: $('#todoDateSearch').val(), dateSearchTo: $('#todoDateSearchTo').val(), StatusSearch: $('#todoStatusSearch').val() },
        success: function (res)
        {
            $('#text-Area').html('').html(res);
        },
        error: function (err)
        {
            console.log(err);
        }

    })
}