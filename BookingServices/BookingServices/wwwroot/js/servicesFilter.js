document.getElementById("searchInput").addEventListener("keyup", filterTable);
document.getElementById("locationSelect").addEventListener("change", filterTable);
document.getElementById("categorySelect").addEventListener("change", filterTable);
document.getElementById("minQuantityRange").addEventListener("input", filterTable); // Min quantity filter
document.getElementById("maxQuantityRange").addEventListener("input", filterTable); // Max quantity filter

function updateQuantityLabel() {
    var minQuantityLabel = document.getElementById("minQuantityLabel");
    var maxQuantityLabel = document.getElementById("maxQuantityLabel");
    minQuantityLabel.textContent = document.getElementById("minQuantityRange").value;
    maxQuantityLabel.textContent = document.getElementById("maxQuantityRange").value;
}

function filterTable() {
    var nameInput = document.getElementById("searchInput").value.toLowerCase();
    var locationSelect = document.getElementById("locationSelect").value.toLowerCase().trim();
    var categorySelect = document.getElementById("categorySelect").value.toLowerCase().trim();
    var minQuantity = parseInt(document.getElementById("minQuantityRange").value) || 0; // Get min quantity
    var maxQuantity = parseInt(document.getElementById("maxQuantityRange").value) || 100; // Get max quantity
    var table = document.querySelector("table tbody");
    var rows = table.getElementsByTagName("tr");

    for (var i = 0; i < rows.length; i++) {
        var cells = rows[i].getElementsByTagName("td");
        var serviceName = cells[0].textContent.toLowerCase().trim(); // Service Name column
        var locationName = cells[1].textContent.toLowerCase().trim(); // Location column
        var categoryName = cells[7].textContent.toLowerCase().trim(); // Category column
        var quantity = parseInt(cells[4].textContent) || 0; // Quantity column

        var nameMatch = serviceName.indexOf(nameInput) > -1;
        var locationMatch = locationSelect === "" || locationName === locationSelect;
        var categoryMatch = categorySelect === "" || categoryName === categorySelect;
        var quantityMatch = quantity >= minQuantity && quantity <= maxQuantity; // Check quantity range

        if (nameMatch && locationMatch && categoryMatch && quantityMatch) {
            rows[i].style.display = "";
        } else {
            rows[i].style.display = "none";
        }
    }
}