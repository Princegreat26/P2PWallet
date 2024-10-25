const container = document.querySelector(".container");
const LoginLink = document.querySelector(".loginlink");
const SignUpLink = document.querySelector(".signuplink");

SignUpLink.addEventListener("click", () => {
  container.classList.add("active");
});

LoginLink.addEventListener("click", () => {
  container.classList.remove("active");
});

document.addEventListener("DOMContentLoaded", function () {
  const container = document.querySelector(".container");
  const loginForm = document.querySelector(".form-box.login form");
  const signupForm = document.querySelector(".form-box.signup form");

  loginForm.addEventListener("submit", function (event) {
    event.preventDefault();

    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    // Validate input fields
    if (!username || !password) {
      alert("Please enter a username and password.");
      return;
    }

    const userlogsData = {
      username: username,
      password: password,
    };

    fetch("https://localhost:7016/api/Authentication/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(userlogsData),
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Login failed.");
        }
        return response.json();
      })
      .then((data) => {
        const token = data.data; // Assuming the token is returned in the data property
        localStorage.setItem("localToken", token);
        alert("Login successful!");
        // Redirect to a protected page or dashboard
        window.location.href = "dashboard.html"; // Change this to your protected page
      })
      .catch((error) => {
        console.error("Error:", error);
        alert("An error occurred during login.");
      });
  });

  // loginForm.addEventListener("submit", function (event) {
  //   event.preventDefault();

  //   const username = document.getElementById("username").value;
  //   const password = document.getElementById("password").value;

  //   const userlogsData = {
  //     username: username,
  //     password: password,
  //   };

  //   fetch("https://localhost:7016/api/Authentication/login", {
  //     method: "POST",
  //     headers: {
  //       "Content-Type": "application/json",
  //     },
  //     body: JSON.stringify(userlogsData),
  //   })
  //     .then((response) => response.text())
  //     .then((token) => {
  //       localStorage.setItem("localToken", token);
  //       if (token) {
  //         alert("Login successful!");
  //         // Redirect to a protected page or dashboard
  //         window.location.href = "dashboard.html"; // Change this to your protected page
  //       } else {
  //         alert("Login failed. Wrong Username or Password.");
  //       }
  //     })
  //     .catch((error) => {
  //       console.error("Error:", error);
  //       alert("An error occurred during login.");
  //     });
  // });

  //new ldloader({ root: ".ldld.full" }).on();
  // var ldld = new ldloader({ root: "#my-loader" });
  // ldld.on();

  signupForm.addEventListener("submit", function (event) {
    event.preventDefault();

    const firstName = document.getElementById("firstName").value;
    const lastName = document.getElementById("lastName").value;
    const phoneNumber = document.getElementById("phoneNumber").value;
    const email = document.getElementById("email").value;
    const username = document.getElementById("signupusername").value;
    const password = document.getElementById("signuppassword").value;
    const confirmPassword = document.getElementById("confirmPassword").value;
    const address = document.getElementById("address").value;

    if (password !== confirmPassword) {
      alert("Passwords do not match!");
      return;
    }

    const userData = {
      firstName: firstName,
      lastName: lastName,
      username: username,
      email: email,
      password: password,
      phoneNumber: phoneNumber,
      address: address,
    };

    // Create and display the spinner
    const spinner = document.createElement("div");
    spinner.className = "spinner";
    document.body.appendChild(spinner);

    fetch("https://localhost:7016/api/Authentication/signup", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(userData),
    })
      .then((response) => response.json())
      .then((data) => {
        // Remove the spinner
        document.body.removeChild(spinner);
        if (data) {
          alert("Sign up successful! Please log in.");
          container.classList.remove("active");
        } else {
          alert("Sign up failed. Please try again.");
        }
      })
      .catch((error) => {
        // Remove the spinner
        document.body.removeChild(spinner);
        console.error("Error:", error);
        alert("An error occurred during sign up.");
      });
  });
});
