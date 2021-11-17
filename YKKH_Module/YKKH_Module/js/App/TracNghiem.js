$(document).ready(function () {
    // Biến toàn cục
    var object = {
        name: 'storeTracNghiem',
        data: []
    }
    var temp = []; // tạo mảng phụ
    var indexQuestion = 1; // lưu giá trị số thứ tự câu hỏi
    var holderQuestion = "Nhập nội dung câu hỏi tại đây";
    var holderAnswer = "Nội dung câu trả lời, click vào dấu tròn để lựa chọn đáp án đúng";

    $('.nextQuestion').click(function () {
        nextQuestion();
    });

    $('#' + object.name).on("click", ".deleteQuestion", function () { // delegation
        var idStoreQuestion = $(this).attr("data-id-store-question");
        var idValueQuestion = $(this).attr("data-id-value-question");
        var stt = $(this).attr("data-stt");

        deleteQuestion(idStoreQuestion, idValueQuestion, stt);
    });

    $('#' + object.name).on("click", ".deleteAnswer", function () { // delegation
        var idStoreAnswer = $(this).attr("data-id-content-answer");
        var idValueAnswer = $(this).attr("data-id-value-answer");

        deleteAnswer(idStoreAnswer, idValueAnswer);
    });

    $('#' + object.name).on("click", ".addAnswer", function () { // delegation
        var idValueQuestion = $(this).attr("data-id-value-question");
        var idStoreAnswer = $(this).attr("data-id-store-answer");
        var idGroupAnswer = $(this).attr("data-id-group-answer");

        createAnswer(idValueQuestion, idStoreAnswer, idGroupAnswer, null);
    });
    //---

    var constantJson = function () {
        var json = { // mẫu json lưu giá trị câu hỏi - trả lời
            id: null,
            value: null,
            parent: null,
            select: false,
            group: null,
            codgrp: null,
            stoques: null,
            stoans: null,
            contans: null,
            stt: -1
        };

        return json;
    }

    var uid = function () { // tạo unique id
        return Date.now().toString(36) + Math.random().toString(36).substr(2);
    }

    var createStore = function () { // Hàm tạo store trắc nghiệm
        var store = $('#' + object.name); // tên
        store.empty(); // clear content inside div
        
        if ($('#ArticleID').val() != "") { // đã có dữ liệu từ server
            if (temp.length == 0) { // người dùng chưa thao tác xóa
                getServerData().then(function (data) {
                    temp = data;
                    exportData(store, temp);
                })
            }
            else { // người dùng đã thao tác xóa
                temp = object.data;
                exportData(store, temp);
            }
        }
        else { // tạo mới
            temp = object.data; // gán dữ liệu qua mảng phụ
            exportData(store, temp);
        }
    }

    var exportData = function (store, temp) { // Hàm duyệt và tạo câu hỏi - trả lời
        object.data = []; // reset lại mảng chính

        for (i = 0; i < temp.length; i++) {
            var item = temp[i];

            if (item.parent == null) { // câu hỏi
                createQuestion(store, item);
            }
            else { // trả lời
                createAnswer(item.parent, item.stoans, item.group, item);
            }
        }
    }

    var createQuestion = function (storeName, item) { // Hàm tạo câu hỏi
        var idStoreQuestion = "";
        var idValueQuestion = "";
        var valueQuestion = "";
        var idStoreAnswer = "";
        var idGroupAnswer = "";
        var json = constantJson();

        if (item != null) { // Đã có dữ liệu
            idStoreQuestion = item.stoques;
            idValueQuestion = item.id;
            valueQuestion = item.value;
            idStoreAnswer = item.stoans;
            idGroupAnswer = item.group;

            json.value = item.value;
        }
        else { // Tạo mới
            var id = uid();
            idStoreQuestion = "storeQuestion_" + id;
            idValueQuestion = "valueQuestion_" + id;
            idStoreAnswer = "storeAnswer_" + id;
            idGroupAnswer = "groupAnswer_" + id;
        }

        json.id = idValueQuestion;
        json.group = idGroupAnswer;
        json.stoques = idStoreQuestion;
        json.stoans = idStoreAnswer;
        json.stt = indexQuestion;
        object.data.push(json); // thêm phần tử vào mảng chính

        storeName.append(markUpQuestion(idStoreQuestion, idValueQuestion, valueQuestion, idStoreAnswer, idGroupAnswer, indexQuestion)); // tạo giao diện câu hỏi

        if (item == null) { // nếu tạo mới hoàn toàn
            for (i = 0; i < 4; i++) { // Tạo sẵn 4 câu trả lời
                createAnswer(idValueQuestion, idStoreAnswer, idGroupAnswer, null);
            }
        }

        indexQuestion++;
    }

    var createAnswer = function (idValueQuestion, idStoreAnswer, idGroupAnswer, item) { // Hàm tạo trả lời
        var id = uid();
        var storeAnswerName = $('#' + idStoreAnswer);
        var idContentAnswer = "";
        var idValueAnswer = "";
        var valueAnswer = "";
        var selectAnswer = false;
        var codeGroupAnswer = "";
        var json = constantJson();

        if (item != null) { // Đã có dữ liệu
            idContentAnswer = item.contans;
            idValueAnswer = item.id;
            valueAnswer = item.value
            selectAnswer = item.select;
            codeGroupAnswer = item.codgrp;
            idGroupAnswer = item.group;

            json.value = item.value;
        }
        else { // Tạo mới
            idContentAnswer = "contentAnswer_" + id;
            idValueAnswer = "valueAnswer_" + id;
            codeGroupAnswer = "codeGroupAnswer_" + id;
        }

        json.id = idValueAnswer;
        json.parent = idValueQuestion;
        json.group = idGroupAnswer;
        json.codgrp = codeGroupAnswer;
        json.stoans = idStoreAnswer;
        json.contans = idContentAnswer;
        object.data.push(json); // thêm phần tử vào mảng chính

        storeAnswerName.append(markUpAnswer(idContentAnswer, idValueAnswer, valueAnswer, selectAnswer, codeGroupAnswer, idGroupAnswer)); // tạo giao diện trả lời
    }

    var deleteQuestion = function (idStoreQuestion, idValueQuestion, stt) { // Hàm xóa câu hỏi
        Swal.fire({
            title: 'Bạn muốn xóa câu hỏi số ' + stt + '?',
            showCancelButton: true,
            cancelButtonColor: '#aaa',
            confirmButtonText: `Xác nhận`,
            cancelButtonText: `Không`,
        }).then((result) => {
            /* Read more about isConfirmed, isDenied below */
            if (result.isConfirmed) {
                object.data = object.data.filter(function (item) { // remove trả lời khỏi mảng 
                    return item.parent !== idValueQuestion;
                });

                object.data = object.data.filter(function (item) { // remove câu hỏi khỏi mảng chính
                    return item.id !== idValueQuestion;
                });

                var storeQuestion = $('#' + idStoreQuestion); 
                storeQuestion.remove(); // xóa div chứa câu hỏi

                // restart
                indexQuestion = 1;
                reloadData();
                createStore();
            }
        })
    }

    var deleteAnswer = function (idStoreAnswer, idValueAnswer) { // Hàm xóa trả lời
        object.data = object.data.filter(function (item) { // remove trả lời khỏi mảng chính
            return item.id !== idValueAnswer;
        });

        var storeAnswer = $('#' + idStoreAnswer);
        storeAnswer.remove(); // xóa div chứa trả lời

        reloadData();
    }

    var nextQuestion = function () { // Hàm thêm câu hỏi
        var store = $('#' + object.name);
        createQuestion(store, null);
    }

    var reloadData = function () { // Hàm cập nhật dữ liệu
        var data = object.data;

        if (data.length > 0) {
            // Duyệt mảng data lấy các giá trị
            for (i = 0; i < data.length; i++) {
                var item = data[i];

                // cập nhật lại value
                var id = $('#' + item.id);
                var valueLatest = id.val();
                item.value = valueLatest;
                //---

                // cập nhật select
                if (item.codgrp != null) {
                    var group = $('#' + item.codgrp);

                    if (group.prop("checked") == true) {
                        item.select = true;
                    }
                }
                //---
            }
        }

        object.data = data;

        return data;
    }

    $.transferData = function (articleID) { // Hàm lưu dữ liệu
        var getData = reloadData();
        //debugger;
        //console.log(getData);
        var model = {
            data: getData,
            articleID: articleID,

        }
        
        $.ajax({
            url: "../Admin/LuuTracNghiem",
            dataType: "json",
            type: "POST",
            data: JSON.stringify(model),
            contentType: 'application/json; charset=utf-8',
            async: true,
            processData: false,
            cache: false,
            success: function (data) {
                if (data == "") {
                    Swal.fire(
                        'Lưu thành công',
                        '',
                        'success'
                    )
                }
                else {
                    Swal.fire(
                        data,
                        '',
                        'warning'
                    )
                }
            },
            error: function (xhr) {
                Swal.fire(
                    xhr,
                    '',
                    'danger'
                )
                console.log(xhr);
            }
        });
    }

    var getServerData = function () { // Hàm lấy dữ liệu từ server
        var model = {
            articleID: $('#ArticleID').val()
        }

        return $.ajax({
            url: "../CMS_Article/LayDuLieuTracNghiem",
            dataType: "json",
            type: "POST",
            data: JSON.stringify(model),
            contentType: 'application/json; charset=utf-8',
            async: true,
            processData: false,
            cache: false,
            success: function (data) {
                return data;
            },
            error: function (xhr) {
                console.log(xhr);
            }
        });
    }

    var markUpQuestion = function (idStoreQuestion, idValueQuestion, valueQuestion, idStoreAnswer, idGroupAnswer, stt) { // Template câu hỏi
        var question = '' +
            '<div id="' + idStoreQuestion + '" class="row marginBottom_10">' +
                '<div class="col-md-12">' +
                    '<div class="panel panel-default">' +
                        '<div class="panel-heading">' +
                            'Câu hỏi số ' + indexQuestion +
                            '<div style="float: right">' +
                                '<button style="border: none" class="deleteQuestion" data-id-store-question="' + idStoreQuestion + '" data-id-value-question="' + idValueQuestion + '" data-stt="' + stt + '">' +
                                    '<i class="fa fa-trash fa-lg" style="color: #d9534f" aria-hidden="true"></i>' +
                                '</button>' +
                            '</div>' +
                        '</div>' +
                        '<div class="panel-body">' +
                            '<div class="row marginBottom_10">' +
                                '<div class="col-md-12">' +
                                    '<input type="text" id="' + idValueQuestion + '" value="' + valueQuestion + '" placeholder="' + holderQuestion + '" class="form-control" />' +
                                '</div>' +
                            '</div>' +
                            '<div id="' + idStoreAnswer + '" class="row">' +
                            '</div>' +
                            '<div class="row marginBottom_10">' +
                                '<div class="col-md-12">' +
                                    '<button class="btn btn-primary addAnswer" data-id-value-question="' + idValueQuestion + '" data-id-store-answer="' + idStoreAnswer + '" data-id-group-answer="' + idGroupAnswer + '">Thêm câu trả lời</button>' +
                                '</div>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>';

        return question;
    }

    var markUpAnswer = function (idContentAnswer, idValueAnswer, valueAnswer, selectAnswer, codeGroupAnswer, idGroupAnswer) { // Template trả lời
        var radioAnswer = '';

        if (selectAnswer) {
            radioAnswer = '<input type="radio" id="' + codeGroupAnswer + '" name="' + idGroupAnswer + '" checked data-id-value-answer="' + idValueAnswer + '" />';
        }
        else {
            radioAnswer = '<input type="radio" id="' + codeGroupAnswer + '" name="' + idGroupAnswer + '" data-id-value-answer="' + idValueAnswer + '" />';
        }

        var answer = '' +
            '<div id="' + idContentAnswer + '" class="col-md-12 marginBottom_10">' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">' +
                        radioAnswer +
                    '</span>' +
                    '<input type="text" id="' + idValueAnswer + '" value="' + valueAnswer + '" placeholder="' + holderAnswer + '" class="form-control">' +
                    '<span class="input-group-addon">' +
                        '<button style="border: none" class="deleteAnswer" data-id-content-answer="' + idContentAnswer + '" data-id-value-answer="' + idValueAnswer + '">' +
                            '<i class="fa fa-minus-circle" style="color: #d9534f" aria-hidden="true"></i>' +
                        '</button>' +
                    '</span>' +
                '</div>' +
            '</div>';

        return answer;
    }

    // init
    createStore();
    //---
});s