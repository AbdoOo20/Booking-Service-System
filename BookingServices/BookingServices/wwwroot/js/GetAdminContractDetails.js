document.addEventListener('DOMContentLoaded', function () {
    const contractSelect = document.getElementById('AdminContractId');
    const viewContractBtn = document.getElementById('viewContractBtn');

    // Enable or disable the view button based on the selection
    contractSelect.addEventListener('change', function () {
        viewContractBtn.disabled = this.value === "" ? true : false;
    });

    // Handle click event on the "View Contract" button
    viewContractBtn.addEventListener('click', function () {
        const selectedContractId = contractSelect.value;
        if (selectedContractId) {
            // Show the modal
            $('#adminContractModal').modal('show');

            // Fetch contract details via AJAX
            fetch(`/AdminContract/GetAdminContractDetails/${selectedContractId}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Failed to load contract details.');
                    }
                    return response.text();
                })
                .then(data => {
                    // Inject the contract details into the modal body
                    document.getElementById('adminContractDetailsContent').innerHTML = data;
                })
                .catch(error => {
                    // Handle errors and display a message in the modal body
                    document.getElementById('adminContractDetailsContent').innerHTML = 'Error loading contract details.';
                    console.error(error);
                });
        }
    });
});
