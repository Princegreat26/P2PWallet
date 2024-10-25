// Get the form element
const form = document.querySelector(".send-money-form");

// Add an event listener to the form submit event
form.addEventListener("submit", async (event) => {
  // Prevent the default form submission behavior
  event.preventDefault();

  // Get the input values
  const amount = document.getElementById("amount").value;
  const destinationAccountNumber = document.getElementById(
    "destination-account-number"
  ).value;
  //const pin = document.getElementById("pin").value;

  // Create the request body
  const requestBody = {
    amount: parseFloat(amount),
    destinationAccountNumber: destinationAccountNumber,
    //pin: pin,
  };

  // Send the request to the backend API
  const response = await fetch(
    "https://localhost:7016/api/Transaction/transfer",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(requestBody),
    }
  );

  // Handle the response
  if (response.ok) {
    // Money transfer successful
    alert("Money transfer successful");
  } else {
    // Money transfer failed
    const error = await response.json();
    alert(`Money transfer failed: ${error.statusMessage}`);
  }
});
