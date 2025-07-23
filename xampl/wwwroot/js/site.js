$(document).ready(function () {
    $('#pageHintsModal').on('show.bs.modal', function (event) {
        var modalBody = $('#pageHintsModalBody');
        var hintsContent = $('#currentPageHints').html();

        if (hintsContent.trim() === '') {
            modalBody.html('<p>No specific hints available for this page. Please refer to general help.</p>');
        } else {
            modalBody.html(hintsContent);
        }
    });
});