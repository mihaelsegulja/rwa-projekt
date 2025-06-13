$(function () {
    const toastList = $('.toast');
    toastList.each(function () {
        const t = new bootstrap.Toast(this, { delay: 3000 });
        t.show();
    });
});

function showToast(message, type = "info", delay = 3000) {
    const bgClass = {
        success: "bg-success text-white",
        error: "bg-danger text-white",
        warning: "bg-warning text-dark",
        info: "bg-info text-dark"
    }[type] || "bg-secondary text-white";

    const toastId = `toast-${Date.now()}`;

    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    const container = document.getElementById("dynamic-toast-container");
    if (!container) {
        console.error("Toast container not found.");
        return;
    }

    container.insertAdjacentHTML("beforeend", toastHtml);

    const toastEl = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastEl, { delay });
    toast.show();

    toastEl.addEventListener("hidden.bs.toast", () => toastEl.remove());
}