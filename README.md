markdown
Kodu kopyala
# Bitirme Projesi

## Proje Hakkında
Bitirme Projesi, [Umut Yükse](https://github.com/UmutYuksel) tarafından geliştirilen bir otomobil ilanı yönetim sistemidir. Kullanıcıların otomobil ilanlarını görüntülemesine, eklemesine, düzenlemesine ve silmesine olanak tanır. Proje, ASP.NET Core MVC ve Entity Framework Core kullanılarak geliştirilmiştir.

## Özellikler
- Otomobil ilanı ekleme, düzenleme ve silme
- Kullanıcı arayüzü için Bootstrap kullanımı
- Resim galerisi ile ilan görselleştirme
- Otomobil detayları ile kullanıcı dostu deneyim
- SQLite veritabanı ile basit veri yönetimi

## Teknolojiler
- **ASP.NET Core MVC**
- **Entity Framework Core**
- **SQLite**
- **Bootstrap**

## Kurulum
1. **Proje Dosyalarını Klonlayın**
   ```bash
   git clone https://github.com/UmutYuksel/Bitirme-Projesi.git
Proje Dizini İçine Girin
bash
Kodu kopyala
cd Bitirme-Projesi
Gerekli Paketleri Yükleyin
bash
Kodu kopyala
dotnet restore
Veritabanını Oluşturun Projeyi ilk kez çalıştırıyorsanız, veritabanını oluşturmak için aşağıdaki komutu çalıştırın:
bash
Kodu kopyala
dotnet ef database update
Uygulamayı Çalıştırın
bash
Kodu kopyala
dotnet run
Tarayıcıda http://localhost:5000 adresine gidin.
Katkıda Bulunanlar
Umut Yüksel

İletişim
Sorular veya öneriler için umtyuksell@icloud.com ile iletişime geçebilirsiniz.
