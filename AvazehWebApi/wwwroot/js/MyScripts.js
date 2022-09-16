function ToggleConfirmDelete(elementID, IsDeleteClicked) {
    deleteID = 'delete_' + elementID;
    ConfirmID = 'confirmDelete_' + elementID;
    if (IsDeleteClicked) {
        $('#' + deleteID).hide();
        $('#' + ConfirmID).show();
    }
    else {
        $('#' + deleteID).show();
        $('#' + ConfirmID).hide();
    }
}