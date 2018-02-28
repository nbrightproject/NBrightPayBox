

$(document).ready(function () {
    // start load all ajax data, continued by js in product.js file

    $('#cmdSave').unbind("click");
    $('#cmdSave').click(function () {
        $('.processing').show();
        nbxget('nbrightpayboxajax_savesettings', '.payboxdata');
    });


    $(document).on("nbxgetcompleted", NBS_PayBox_nbxgetCompleted); // assign a completed event for the ajax calls

    // function to do actions after an ajax call has been made.
    function NBS_PayBox_nbxgetCompleted(e) {

        $('.processing').hide();

    };

});

