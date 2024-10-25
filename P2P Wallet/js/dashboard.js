import jwt_decode from "https://cdn.jsdelivr.net/npm/jwt-decode@3.1.2/build/jwt-decode.esm.js";

document.addEventListener("DOMContentLoaded", () => {
  // Retrieve the token from local storage or wherever it is stored
  const token = localStorage.getItem("localToken");
  console.log("Token:", token);

  if (token) {
    //const jwt_decode = require("jwt-decode");
    // Decode the token to retrieve the user ID
    const decodedToken = jwt_decode(token);
    console.log("Decoded token:", decodedToken);

    const userId = decodedToken.UserId;
    console.log("User ID:", userId);

    fetch("https://localhost:7016/api/User/GetUserData", {
      method: "GET",
      headers: {
        Authorization: `bearer ${token}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Failed to fetch user data.");
        }
        return response.json();
      })
      .then((data) => {
        console.log("User data:", data);

        // Assuming the response contains the user data
        document.getElementById("user-name").textContent = data.username;
        document.getElementById(
          "greeting"
        ).textContent = `Good Morning, ${data.username}`;
        document.getElementById(
          "card-holder"
        ).textContent = `${data.firstName} ${data.lastName}`;
        document.getElementById(
          "total-balance"
        ).textContent = `₦${data.account[0].balance}`;
        document.getElementById(
          "account-number"
        ).textContent = `${data.accounts[0].accountNumber.slice(-4)}`;
      })
      .catch((error) => {
        console.error("Error fetching user data:", error);
      });
  } else {
    // Redirect the user to the login page if they are not authenticated
    window.location.href = "login.html"; // Change this to your login page
  }
});
