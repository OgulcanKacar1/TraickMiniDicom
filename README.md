# Mini DICOM API

Bu proje, medikal görüntüleme (`.dcm`) dosyalarını işlemek ve içerisindeki metadataları yönetmek için geliştirilmiş, .NET tabanlı bir RESTful API servisidir.

## 🚀 Özellikler

* **DICOM Yükleme:** `.dcm` uzantılı medikal dosyaları uç nokta üzerinden kabul eder.
* **Veri Ayıklama:** `fo-dicom` kütüphanesini kullanarak dosya içerisinden *PatientName, StudyInstanceUID, Modality, Series* ve *Image Resolution* (Görüntü Çözünürlüğü) bilgilerini okur.
* **Veritabanı Kaydı:** Ayıklanan medikal verileri PostgreSQL veritabanındaki `Studies` tablosuna ilişkisel olarak kaydeder.
* **Listeleme:** Kaydedilen tüm çalışmaları (study) en güncelden eskiye doğru sıralayarak listeler.

## 🛠️ Kullanılan Teknolojiler

* **Backend:** .NET Web API (C#)
* **Veritabanı:** PostgreSQL
* **ORM:** Entity Framework Core (Code-First)
* **DICOM İşleme:** fo-dicom
* **Dokümantasyon & Test:** Swagger UI





