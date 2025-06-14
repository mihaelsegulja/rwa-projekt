// For TempData messages
$(function () {
    const toastList = $('.toast');
    toastList.each(function () {
        const t = new bootstrap.Toast(this, { delay: 3000 });
        t.show();
    });
});

// For dynamic messages
function showToast (message, type = "info", delay = 3000) {
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

    const container = $("#dynamic-toast-container");
    if (!container.length) {
        console.error("Toast container not found.");
        return;
    }

    container.append(toastHtml);

    const $toast = $(`#${toastId}`);
    const toastInstance = new bootstrap.Toast($toast[0], { delay });
    toastInstance.show();

    $toast.on("hidden.bs.toast", () => $toast.remove());
};
