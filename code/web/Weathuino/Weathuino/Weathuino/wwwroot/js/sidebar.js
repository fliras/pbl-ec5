const sidebar = document.getElementById("sidebar");
const main = document.getElementById("mainContent");

window.addEventListener("DOMContentLoaded", () => {
    const isClosed = localStorage.getItem("sidebarState") === "closed";
    if (isClosed) {
        sidebar.classList.add("closed");
        main.classList.add("shifted");
    }

    // setup dark mode
    const isDarkMode = localStorage.getItem("darkMode") === "true";
    if (isDarkMode) {
        document.body.classList.add("dark-mode");
    }

    updateDarkModeButtonText();
});

function toggleSidebar() {
    sidebar.classList.toggle("closed");
    main.classList.toggle("shifted");

    const state = sidebar.classList.contains("closed") ? "closed" : "open";
    localStorage.setItem("sidebarState", state);
}

function toggleDarkMode() {
    document.body.classList.toggle("dark-mode");

    const isDarkMode = document.body.classList.contains("dark-mode");
    localStorage.setItem("darkMode", isDarkMode);

    updateDarkModeButtonText();
}

function updateDarkModeButtonText() {
    const buttonSpan = document.querySelector('.dark-mode-toggle button span');
    const isDarkMode = document.body.classList.contains('dark-mode');
    if (buttonSpan) {
        buttonSpan.textContent = isDarkMode ? 'Modo Claro' : 'Modo Escuro';
    }
}
