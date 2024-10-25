document.addEventListener("DOMContentLoaded", function () {
  const createPinForm = document.querySelector(".create-pin-form");

  createPinForm.addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent the default form submission
    const newPin = document.getElementById("new-pin").value;
    const confirmPin = document.getElementById("confirm-pin").value;

    if (newPin === confirmPin) {
      alert("PIN created successfully!");
      // Here you can add code to save the PIN, e.g., using localStorage or sending it to a server
    } else {
      alert("PINs do not match. Please try again.");
    }
  });
});
