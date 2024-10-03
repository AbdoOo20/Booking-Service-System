$(document).ready(function () {
    var serviceIdToProcess; // Store the service ID temporarily
    var currentRow; // Store the current row to update later

    // Open the modal on click
    $(document).on('click', '.make-service-online', function () {
        serviceIdToProcess = $(this).data('service-id'); // Get the service ID
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

