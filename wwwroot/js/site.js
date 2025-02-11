// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    let scrollBtn = document.getElementById("scrollBtn");
    let icon = scrollBtn.querySelector("i");

    window.addEventListener("scroll", function () {
        if (window.scrollY > 300) {
            icon.classList.remove("fa-arrow-down");
            icon.classList.add("fa-arrow-up");
        } else {
            icon.classList.remove("fa-arrow-up");
            icon.classList.add("fa-arrow-down");
        }
    });

    scrollBtn.addEventListener("click", function () {
        if (icon.classList.contains("fa-arrow-up")) {
            window.scrollTo({ top: 0, behavior: "smooth" });
        } else {
            window.scrollTo({ top: document.body.scrollHeight, behavior: "smooth" });
        }
    });
});
