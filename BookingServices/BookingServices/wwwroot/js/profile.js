
// wwwroot/js/withdraw.js
$(document).ready(function () {
    $("form").on("submit", function (event) {
        if ($(this).valid()) { } else { event.preventDefault(); }
    });
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
                            $('#message').html('<div class="alert alert-success">Withdrawal Successful</div>');
                            setTimeout(function () {
                                window.location.reload();
                            }, 2000);
                        } else {
                            $('#message').html('<div class="alert alert-danger">Error: Unable to update balance.</div>');
                        }
                    },
                    error: function (xhr, status, error) {
                        $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
                    },
                    complete: function () {
                        $('#loadingOverlay').hide();
                        setTimeout(function () {
                            $('#message').fadeOut();
                        }, 3000);
                    }
                });
            },
            error: function (xhr, status, error) {
                $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            },
            complete: function () {
                $('#loadingOverlay').hide();
                setTimeout(function () {
                    $('#message').fadeOut();
                }, 3000);
            }
        });
    });
});
