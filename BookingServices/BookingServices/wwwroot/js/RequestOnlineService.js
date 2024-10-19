$(document).ready(function () {
    var serviceIdToProcess; // Store the service ID temporarily
    var currentRow; // Store the current row to update later
    var connection = new signalR.HubConnectionBuilder().withUrl("/AdminNotification").build();

    connection.start().then(function () {
        console.log("SignalR connected successfully!");
    }).catch(function (err) {
        return console.error(err.toString());
    });

    // Open the modal on click
    $(document).on('click', '.make-service-online', function () {
        serviceIdToProcess = $(this).data('service-id'); // Get the service ID
        serviceName = $(this).data('service-name'); // Get the service ID
        currentRow = $(this).closest('tr'); // Get the current table row
        $('#confirmModal').modal('show'); // Show the modal
    });

    // Handle the confirm button click
    $('#confirmAction').on('click', function () {
        $.ajax({
            url: '/Services/RequestToMakeServiceOnline', // Adjust the URL if needed
            type: 'POST',
            data: { id: serviceIdToProcess }, // Pass the service ID to the action
            success: function (response) {
                $('#confirmModal').modal('hide'); // Hide the modal on success
                if (response.success) {
                    // Update the status column in the current row dynamically
                    currentRow.find('.status-column').html('<div class="alert alert-warning text-center m-auto pt-1 pb-1">Pending</div>');
                    // Add Notification to DB and get updated notifications
                    $.ajax({
                        url: '/AdminNotification/AddNotification', // URL for adding notification
                        type: 'POST',
                        data: {
                            NotificationTitle: `requested to make the service ${serviceName} available online on the website`,
                            Time: new Date()
                        },

                        success: function (notificationResponse) {
                            if (notificationResponse.success) {
                                // Get all notifications and update the count
                                $.ajax({
                                    url: '/AdminNotification/GetAllNotifications',
                                    type: 'GET',
                                    success: function (data) {
                                        // Clear the dropdown list and append new notifications
                                        $('#dropDownCustomer').empty();
                                        $.each(data.notifications, function (index, notification) {
                                            $('#dropDownCustomer').append(`
                                            <div class="notification-item">
                                                <div class="notification-title">${notification.notificationTitle}</div>
                                                <div class="notification-time">${new Date(notification.time).toLocaleString()}</div>
                                            </div>
                                        `);
                                        });

                                        // Update the notification count
                                        $('.badge-number').text(data.length);

                                        // Send notification to SignalR Hub
                                        var notificationMessage = `requested to make the service ${serviceName} available online on the website`;
                                        connection.invoke("SendMessage", notificationMessage, new Date(), data.length).catch(function (err) {
                                            return console.error(err.toString());
                                        });
                                    },
                                    error: function () {
                                        console.error('Error fetching notifications.');
                                    }
                                });
                            }
                        },
                        error: function () {
                            console.error('Error adding notification.');
                        }
                    });
                } else {
                    // Handle error response
                    console.error(response.message); // Log error message
                }
            },
            error: function () {
                $('#confirmModal').modal('hide'); // Hide the modal on error
                console.error('An error occurred. Please try again.'); // Log error message
            }
        });
    });
});

