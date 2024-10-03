document.addEventListener('DOMContentLoaded', function () {
    const startTimeSelect = document.getElementById('StartTime');
    const endTimeSelect = document.getElementById('EndTime');
    const endTimeOptions = Array.from(endTimeSelect.options);

    startTimeSelect.addEventListener('change', function () {
        const selectedStartTime = parseInt(this.value);

        // Clear current end time options
        endTimeSelect.innerHTML = '<option value="">-- Select End Time --</option>';

        // Re-populate end time options with values greater than selected start time
        endTimeOptions.forEach(option => {
            const optionValue = parseInt(option.value);
            if (optionValue > selectedStartTime) {
                endTimeSelect.appendChild(option.cloneNode(true));
            }
        });
        endTimeSelect.disabled = false;
    });
});
