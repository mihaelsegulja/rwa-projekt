$('#confirmDeleteModal').on('show.bs.modal', function (e) {
    const button = $(e.relatedTarget);
    const projectId = button.data('project-id');
    const projectTitle = button.data('project-title');

    $('#projectTitle').text(projectTitle);
    $('#deleteForm').attr('action', `/Project/Delete/${projectId}`);
});
