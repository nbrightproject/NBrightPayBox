

$(document).ready(function () {
    // start load all ajax data, continued by js in product.js file

    $('#paybox_cmdSave').unbind("click");
    $('#paybox_cmdSave').click(function () {
        $('.processing').show();
        $(this).hide();
        nbxget('nbrightpayboxajax_savesettings', '.payboxdata', '.payboxreturnmsg');
    });


    $(document).on("nbxgetcompleted", NBS_PayBox_nbxgetCompleted); // assign a completed event for the ajax calls

    // function to do actions after an ajax call has been made.
    function NBS_PayBox_nbxgetCompleted(e) {

        $('.processing').hide();

        $('#paybox_cmdSave').show();

    };

});

