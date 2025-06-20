$('#confirmDeleteModal').on('show.bs.modal', function (e) {
    const button = $(e.relatedTarget);
    const itemId = button.data('item-id');
    const itemType = button.data('item-type');
    const itemName = button.data('item-name');

    $('#deleteItemLabel').text(`${itemType.toLowerCase()} "${itemName}"`);
    $('#deleteForm').attr('action', `/${itemType}/Delete/${itemId}`);
});
