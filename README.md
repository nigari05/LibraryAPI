# 📚 LibraryAPI

Bu layihə, müasir kitabxana idarəetmə sisteminin backend infrastrukturunu təmin etmək üçün ASP.NET Core (.NET 10), Entity Framework Core və SQL Server texnologiyaları istifadə edilərək yaradılmışdır. Layihə tam N-Layer Arxitektura (Qatlı Arxitektura) prinsiplərinə uyğun dizayn edilmişdir.

## 🏗️ Arxitektura Quruluşu

Layihə məlumatların təhlükəsizliyini, kodun oxunurluğunu və test edilə bilməsini təmin etmək üçün əsas qatlara bölünmüşdür:

1. **WebAPI (Controllers)** — HTTP sorğularını qəbul edir, validasiyadan keçirir və müvafiq servisə ötürür.
2. **Business (Managers & Services)** — Layihənin bütün əsas biznes qaydaları və məntiqləri bu qatda icra olunur.
3. **DataAccess (Repositories)** — Entity Framework Core vasitəsilə SQL Server verilənlər bazası ilə birbaşa əlaqəni idarə edir.
4. **Entities (Concrete & DTOs)** — Domain modelləri və Data Transfer Object-ləri saxlayır.
5. **Core** — Ümumi utilit funksiyalar, nəticə (Result) strukturları və extension metodlar üçün istifadə olunur.

💡 **Mühüm Qeyd:** Domain entity-lərinin birbaşa API cavabı kimi qaytarılmasının qarşısını almaq üçün tam DTO (Data Transfer Object) strukturu tətbiq edilmişdir.

## 🛠️ Texnologiya Steki

* Dil: C#
* Framework: ASP.NET Core (.NET 10)
* ORM: Entity Framework Core
* Verilənlər Bazası: SQL Server
* Validasiya: FluentValidation
* Autentifikasiya: JWT Authentication & Authorization
* Sənədləşmə: Swagger / OpenAPI

## 🚀 Quraşdırma və İşə Salma Bələdçisi

### 1. Layihəni Klonlayın

İlk öncə repozitoriyanı yerli mühitinizə yükləyin:

```
git clone https://github.com/USERNAME/LibraryAPI.git
cd LibraryAPI
```

### 2. Connection String-i Yeniləyin

`appsettings.json` faylını açın:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Miqrasiyaları Tətbiq Edin

```
dotnet ef database update
```

### 4. Layihəni İşə Salın

```
dotnet run
```

Layihə işə düşdükdən sonra Swagger sənədləşdirməsinə aşağıdakı ünvandan daxil ola bilərsiniz:

```
https://localhost:xxxx/swagger
```

## 📌 API Modulları

* **Kitablar (Books):** Kitabların əlavə edilməsi, yenilənməsi, silinməsi və siyahılanması
* **Müəlliflər (Authors):** Müəllif məlumatlarının idarə edilməsi
* **Üzvlər (Members):** Kitabxana üzvlərinin qeydiyyatı və idarə edilməsi

