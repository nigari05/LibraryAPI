# 📚 LibraryAPI

Bu layihə, müasir kitabxana idarəetmə sisteminin backend infrastrukturunu təmin etmək üçün ASP.NET Core (.NET 10), Entity Framework Core və SQL Server texnologiyaları istifadə edilərək yaradılmışdır. Layihə **Onion Architecture** prinsiplərinə uyğun dizayn edilmişdir: domain (Entities) və əsas kontraktlar (Core) mərkəzdə, DataAccess və Business ondan asılı, WebAPI isə ən xaricdə yerləşir.

## 🏗️ Arxitektura Quruluşu

1. **WebAPI (Controllers)** — HTTP sorğularını qəbul edir, validasiyadan keçirir və müvafiq servisə ötürür. JWT autentifikasiyası, Swagger sənədləşdirməsi və planlaşdırılmış (background) xidmətlər də bu qatda konfiqurasiya olunur.
2. **Business (Managers & Services)** — Layihənin bütün əsas biznes qaydaları və məntiqləri bu qatda icra olunur (validasiya, keşləmə, fayl saxlama, asinxron bildiriş və s.).
3. **DataAccess (Repositories)** — Entity Framework Core vasitəsilə SQL Server verilənlər bazası ilə birbaşa əlaqəni idarə edir (generic repository + Specification pattern).
4. **Entities (Concrete & DTOs)** — Domain modelləri və Data Transfer Object-ləri saxlayır.
5. **Core** — Ümumi utilit funksiyalar (keşləmə, fayl saxlama abstraksiyası, background task queue), nəticə (Result) strukturları və repository kontraktları üçün istifadə olunur.

💡 **Mühüm Qeyd:** Domain entity-lərinin birbaşa API cavabı kimi qaytarılmasının qarşısını almaq üçün tam DTO (Data Transfer Object) strukturu tətbiq edilmişdir.

## 🛠️ Texnologiya Steki

* Dil: C#
* Framework: ASP.NET Core (.NET 10)
* ORM: Entity Framework Core (SQL Server)
* Validasiya: FluentValidation
* Mapping: AutoMapper
* Autentifikasiya: JWT Bearer Authentication & Role-based Authorization
* Keşləmə: IMemoryCache əsaslı in-memory cache (ICacheService abstraksiyası ilə)
* Fayl saxlama: Yerli disk üzərində IFileStorageService abstraksiyası (şəkil yükləmə/endirmə)
* Planlaşdırılmış tapşırıqlar: ASP.NET Core BackgroundService (gündəlik təmizləmə)
* Asinxron emal: System.Threading.Channels əsaslı background task queue (email bildirişi simulyasiyası)
* Sənədləşmə: Swagger / OpenAPI (Swashbuckle.AspNetCore) — JWT Bearer dəstəyi ilə
* Test: xUnit, Moq, EF Core InMemory/SQLite

## 🚀 Quraşdırma və İşə Salma Bələdçisi

### 1. Layihəni Klonlayın

İlk öncə repozitoriyanı yerli mühitinizə yükləyin:

```
git clone https://github.com/nigari05/LibraryAPI.git
cd LibraryAPI
```

### 2. Konfiqurasiyanı Təyin Edin

Bağlantı sətri və digər parametrlər `appsettings.json`/`appsettings.Development.json`-dan (mühitə görə) oxunur — koda hardcode edilmir (bax: Checkpoint 6). Layihənin gözlədiyi konfiqurasiya açarlarının tam nümunəsi:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LibraryDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": null,
    "Issuer": "LibraryAPI",
    "Audience": "LibraryUsers",
    "ExpireMinutes": 60
  },
  "FileStorage": {
    "BasePath": "wwwroot/uploads"
  },
  "Scheduling": {
    "DailyCleanupIntervalHours": 24
  },
  "Notifications": {
    "SimulatedSendDelayMs": 2000
  }
}
```

⚠️ **`Jwt:Key` boş buraxılmamalıdır** — token imzalamaq üçün istifadə olunur və tətbiq bunsuz açılışda xəta verəcək. Bunu heç vaxt `appsettings.json`-da commit etməyin; əvəzinə **User Secrets** (lokal development) və ya **environment variable** (production) vasitəsilə təyin edin:

```
cd WebAPI
dotnet user-secrets set "Jwt:Key" "ən azı 32 simvoldan ibarət təhlükəsiz bir sirr açar"
```

Production mühitində isə `ASPNETCORE_ENVIRONMENT=Production` təyin edərək və `Jwt__Key` environment variable-ı ilə (`__` = `:` ekvivalenti) eyni məqsədə çatmaq olar. `appsettings.Development.json` isə ayrıca bir dev bazasını (`LibraryDb_Dev`) istifadə edir ki, dev/prod məlumatları qarışmasın.

### 3. Miqrasiyaları Tətbiq Edin

```
dotnet ef database update --project DataAccess --startup-project WebAPI
```

### 4. Layihəni İşə Salın

```
cd WebAPI
dotnet run
```

Layihə işə düşdükdən sonra Swagger sənədləşdirməsinə aşağıdakı ünvandan daxil ola bilərsiniz:

```
https://localhost:xxxx/swagger
```

Qorunan (JWT tələb edən) endpoint-ləri test etmək üçün: `POST /api/Auth/login` ilə token alın, sonra Swagger UI-nin sağ yuxarısındakı **"Authorize"** düyməsinə klikləyib tokeni daxil edin (yalnız tokenin özü, `Bearer` prefiksi olmadan).

## 📌 API Modulları

* **Autentifikasiya (Auth):** Qeydiyyat, giriş, JWT token (access + refresh) idarəetməsi
* **Kitablar (Books):** Əlavə etmə/yeniləmə/silmə/siyahılanma, üç fərqli filtrləmə yanaşması (dynamic LINQ, native SQL, Specification pattern), üz qabığı şəklinin yüklənməsi/endirilməsi (`POST` / `GET /api/Book/{id}/cover`)
* **Müəlliflər (Authors):** Müəllif məlumatlarının idarə edilməsi
* **Kateqoriyalar (Categories):** Kateqoriya idarəetməsi — siyahı nəticəsi keşlənir, data dəyişəndə keş avtomatik etibarsızlaşdırılır
* **Üzvlər (Members):** Kitabxana üzvlərinin qeydiyyatı və idarə edilməsi
* **Kitab İcarəsi (BookLoans):** `borrow`/`return` — tranzaksiya daxilində stok idarəetməsi, uğurlu əməliyyatdan sonra üzvə asinxron (bloklamadan) email bildirişi
* **Arxa Plan Xidmətləri:** Gündəlik təmizləmə (sahibsiz fayllar + gecikmiş icarələr) və email növbəsinin icrası tətbiq başlayanda avtomatik işə düşür, ayrıca çağırış tələb etmir

