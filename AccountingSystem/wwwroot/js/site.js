const DetailRequest = function (requestId) {

    let url = "/Requests/Details?Id=" + requestId;

    $("#requestModalBodyDiv").load(url, function () {
        $("#requestModal").modal("show");

    })
    $("#modal-title").text('Подробности');
}

const CancelRequest = function (requestId) {
    let url = "/Requests/Cancel?Id=" + requestId;

    $("#requestModalBodyDiv").load(url, function () {
        $("#requestModal").modal("show");

    })
    $("#modal-title").text('Отменить заявку');
}
const CancelSendRequest = function (requestId) {
    let url = "/Requests/CancelSend?Id=" + requestId;

    $("#requestModalBodyDiv").load(url, function () {
        $("#requestModal").modal("show");

    })
    $("#modal-title").text('Отменить отправку');
}

const ReviewRequest = function (requestId) {

    let url = "/Requests/Review?id=" + requestId;

    $("#requestsModalBodyDiv").load(url, function () {
        $("#requestsModal").modal("show");

    })
    $("#modal-title").text('Обзор заявки');
}

const EditFromReview = function (requestId) {

    let url = "/Requests/EditFromReview?id=" + requestId;

    $("#requestsModalBodyDiv").load(url, function () {
        $("#requestsModal").modal("show");

    })
    $("#modal-title").text('Редактирование');
}

const EditRequest = function (requestId) {

    let url = "/Requests/Edit?id=" + requestId;

    $("#requestModalBodyDiv").load(url, function () {
        $("#requestModal").modal("show");

    })
    $("#modal-title").text('Редактирование');
}

const DeleteUser = function (userId) {

    let url = "/Users/Delete?id=" + userId;

    $("#userModalBodyDivs").load(url, function () {
        
        $("#userModals").modal("show");
        
    })
    $("#modal-title").text('Удалить сотрудника');

}

const CloseModal = function () {
    $('body').trigger('click');
}

const EditUser = function (userId) {

    let url = "/Users/Edit?id=" + userId;

    $("#userModalBodyDivs").load(url, function () {
       
        $("#userModals").modal("show");
    })
    $("#modal-title").text("Редактирование");
}