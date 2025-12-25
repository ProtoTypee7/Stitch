// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.





    document.addEventListener('DOMContentLoaded', function () {
        const dropdown = document.querySelector('.dropdown');
        const menu = dropdown.querySelector('.dropdown-menu');

        dropdown.addEventListener('mouseenter', () => {
            menu.style.display = 'block';
        });

        dropdown.addEventListener('mouseleave', () => {
            menu.style.display = 'none';
        });
    });



    // numver animate hereee
document.addEventListener("DOMContentLoaded", () => {
    const counters = document.querySelectorAll(".stagger-counter");

    const animateCounter = (el) => {
        const target = +el.getAttribute("data-count");
        let current = 0;

        const duration = 2000; // total animation time (ms)
        const steps = 100; // number of updates
        const increment = Math.ceil(target / steps);
        const interval = duration / steps;

        const update = () => {
            current += increment;
            if (current < target) {
                el.textContent = current;
                setTimeout(update, interval);
            } else {
                el.textContent = target.toLocaleString();
            }
        };

        update();
    };

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && !entry.target.classList.contains("animated")) {
    entry.target.classList.add("animated");
    animateCounter(entry.target);
    observer.unobserve(entry.target);
}

        });
    }, {
        threshold: 0.6
    });

    counters.forEach(counter => observer.observe(counter));
});



// VALIDATE EMAIL 
   const emailInput = document.getElementById('email');
        emailInput.addEventListener('input', () => {
            const val = emailInput.value;
            const isValid = val.includes('@') && val.includes('.com');
            emailInput.classList.toggle('is-valid', isValid);
            emailInput.classList.toggle('is-invalid', !isValid);
        });