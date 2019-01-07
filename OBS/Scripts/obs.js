

//  Şimdiki tarihi yazdırma

var date = new Date();

var day = date.getDate();
var month = date.getMonth() + 1;
var year = date.getFullYear();

if (month < 10) month = "0" + month;
if (day < 10) day = "0" + day;

var todayView = day + "." + month + "." + year;

///////// FONKSİYON TANIMLAMALARI //////////

// Select2 Ajax ile listeleme

$.select2Ajax = {
    VeriCek: function(id, dataUrl) {


        $(id).select2({
            ajax: {
                url: dataUrl,
                data: function(params) {
                    return {
                        q: params.term // arama terimi
                    };
                },
                processResults: function(data) {
                    return {
                        results: data.items
                    };
                }
            },
            placeholder: "Seçiniz"
        });

    }
};

$.imageUpload = {
    Post: function(id) {
        if ($(id).val().length > 0) {
            var fileUpload = $(id).get(0);
            var files = fileUpload.files;
            var fileData = new FormData();
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }
            // Adding one more key to FormData object
            fileData.append('username', 'Manas');
            $.ajax({
                url: '/upload/image',
                type: "POST",
                contentType: false, // Not to set any content header
                processData: false, // Not to process data
                data: fileData,
                success: function (result) {
                },
                error: function (err) {
                    alert(err.statusText);
                }
            });
        }
    }
};

$.iziToast = {
    Success: function() {
        iziToast.success({
            title: 'Başarılı',
            message: 'İşleminiz gerçekleştirilmiştir.'
        });
    },
    Info: function(message) {
        iziToast.info({
            title: 'Bilgi',
            message: message
        });
    },
    Warning: function() {
        iziToast.warning({
            title: 'Hata',
            message: 'Lütfen tekrar deneyiniz.'
        });
    }
};




