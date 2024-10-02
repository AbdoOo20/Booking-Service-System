$(document).ready(function () {
    // Attach event handlers to trigger filtering dynamically
    $("#SearchText").on("keyup", function () {
        filterData();
    });

    $("#FromDate, #ToDate").on("change", function () {
        filterData();
    });

    // Function to handle the AJAX call and update results
    function filterData() {
        var searchText = $("#SearchText").val();
        var fromDate = $("#FromDate").val();
        var toDate = $("#ToDate").val();


        // Validate dates
        if (fromDate && toDate) {
            if (new Date(fromDate) > new Date(toDate)) {
                $("#dateError").text("To date cannot be earlier than From date.").show();
                return;
            } else {
                $("#dateError").hide();
            }
        }
        $.ajax({
            url: ajaxUrl,
            type: 'GET',
            data: {
                searchText: searchText,
                fromDate: fromDate,
                toDate: toDate
            },
            success: function (data) {
                updateResults(data);
            },
            error: function () {
                alert("Error retrieving data.");
            }
        });
    }

    // Function to update the results table dynamically
    function updateResults(data) {
        var resultsTable = $("#resultsTable tbody");
        resultsTable.empty();

        if (Array.isArray(data) && data.length > 0) {
            data.forEach(function (item) {
                var row = "<tr>" +
                    "<td>" + item.name + "</td>" +
                    "<td>" + item.paymentCount + "</td>" +
                    "<td>" + item.totalBenefit + "</td>" +
                    "<td>" +
                    "<a href='/Controller/Details?paymentIncomeId=" + item.paymentIncomeId + "'>" +
                    "<i class='fa-solid fa-angles-right' style='color:green;font-size:30px'></i>" +
                    "</a>" +
                    "</td>" +
                    "</tr>";
                resultsTable.append(row);
            });
        } else {
            resultsTable.append("<tr><td colspan='4'>No results found</td></tr>");
        }
    }
});
