// wwwroot/js/withdraw.js
$(document).ready(function () {
    $('#withdrawButton').click(function (e) {
        e.preventDefault();
        if ($(this).is(':disabled')) {
            return;
        }
        $('#loadingOverlay').show();
        var data = {
            serviceProviderEmail: $('#serviceProviderEmail').val(),
            totalAmount: $('#totalAmount').val(),
            platformPercentage: 0
        };
        $.ajax({
            url: 'http://localhost:18105/api/PayPal/payout',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                $.ajax({
                    url: '/ProfileProvider/Payout',
                    type: 'POST',
                    success: function (result) {
                        if (result.success) {
                            $('#balanceDisplay').text(result.newBalance);
                            $('#responseMessage').html('<div class="alert alert-success">Withdrawal Successful</div>');
                        } else {
                            $('#responseMessage').html('<div class="alert alert-danger">Error: Unable to update balance.</div>');
                        }
                    },
                    error: function (xhr, status, error) {
                        $('#responseMessage').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
                    },
                    complete: function () {
                        $('#loadingOverlay').hide();
                        setTimeout(function () {
                            $('#responseMessage').fadeOut();
                        }, 3000);
                    }
                });
            },
            error: function (xhr, status, error) {
                $('#responseMessage').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            },
            complete: function () {
                $('#loadingOverlay').hide();
                setTimeout(function () {
                    $('#responseMessage').fadeOut();
                }, 3000);
            }
        });
    });
});
