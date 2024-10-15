function getBrandsByCategory(categoryId) {
    if (categoryId) {
        $.ajax({
            url: '/Create/GetBrandsByCategory',
            type: 'GET',
            data: { categoryId: categoryId },
            success: function (data) {
                // Başarı durumunda markaları güncelle
                var brandSelect = $('#brandSelect'); // Markaların listeleneceği select elementinin ID'si
                brandSelect.empty(); // Mevcut markaları temizle
                $.each(data, function (index, brand) {
                    brandSelect.append($('<option>', {
                        value: brand.brandId,
                        text: brand.brandName
                    }));
                });
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error: " + status + error);
            }
        });
    } else {
        // Eğer kategori seçilmemişse marka listesini temizle
        $('#brandSelect').empty();
        $('#brandSelect').append('<option value="">Önce kategori seçin</option>');
    }
}
