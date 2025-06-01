document.addEventListener("DOMContentLoaded", function() {
    loadDashboard("#dashboard1", "/Dashboards/GetDashboard1");
    loadDashboard("#dashboard2", "/Dashboards/GetDashboard2");
});

function loadDashboard(containerSelector, url) {
    const container = document.querySelector(containerSelector);
    if (!container) return;
    
    fetch(url)
    .then(response => {
      if (!response.ok) {
        throw new Error("Network error");
      }
    return response.text();
    })
    .then(html => {
        container.innerHTML = html;
    })
    .catch(err => {
        console.error(err);
    container.innerHTML = "<p class='fallback-text text-danger'>Erro ao carregar o dashboard.</p>";
    });
}