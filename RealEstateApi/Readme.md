# Real Estate API

Bu proje, emlak ilanları için kapsamlı bir backend API'sidir. Kullanıcılar kayıt olabilir, giriş yapabilir ve kendi ilanlarını yönetebilirler.

## Özellikler

### Kullanıcı Yönetimi
- Kullanıcı kaydı
- Kullanıcı girişi
- JWT tabanlı kimlik doğrulama

### İlan Yönetimi
- İlan oluşturma (sadece giriş yapmış kullanıcılar)
- İlan düzenleme (sadece ilan sahibi)
- İlan silme (sadece ilan sahibi)
- İlan listeleme (herkes)
- Detaylı ilan görüntüleme

### Arama ve Filtreleme
- **Metin tabanlı arama**: İlan başlığı ve açıklamasında arama
- **Konum filtreleri**: Şehir, ilçe ve sokak bazında filtreleme
- **Fiyat filtreleri**: Minimum ve maksimum fiyat aralığı
- **Oda tipi filtresi**: 1+1, 2+1, 3+1, 4+1, Dublex, Villa vb.
- **Sıralama**: Fiyat, tarih veya başlık göre artan/azalan sıralama
- **Sayfalama**: Büyük veri setleri için sayfa bazlı görüntüleme

### İstatistikler
- Toplam ilan sayısı
- Ortalama fiyat
- Şehir bazında istatistikler

## Kurulum

### 1. Projeyi İndirin
```bash
git clone <repository-url>
cd RealEstateApi
```

### 2. Bağımlılıkları Yükleyin
```bash
dotnet restore
```

### 3. Veritabanını Oluşturun
```bash
# Migration oluştur
dotnet ef migrations add InitialCreate

# Veritabanını güncelle
dotnet ef database update
```

### 4. Projeyi Çalıştırın
```bash
dotnet run
```

Uygulama şu adreste çalışacaktır: `https://localhost:7039`

## API Endpoints

### Kimlik Doğrulama
- `POST /api/Auth/register` - Kullanıcı kaydı
- `POST /api/Auth/login` - Kullanıcı girişi

### İlan Yönetimi
- `GET /api/Listings` - Tüm ilanları listele (filtreleme destekli)
- `GET /api/Listings/{id}` - Belirli bir ilanı getir
- `GET /api/Listings/my-listings` - Kullanıcının kendi ilanları (🔒 Giriş gerekli)
- `POST /api/Listings` - Yeni ilan oluştur (🔒 Giriş gerekli)
- `PUT /api/Listings/{id}` - İlan güncelle (🔒 Sadece ilan sahibi)
- `DELETE /api/Listings/{id}` - İlan sil (🔒 Sadece ilan sahibi)
- `GET /api/Listings/stats` - İlan istatistikleri

## Örnek Kullanım

### Kullanıcı Kaydı
```json
POST /api/Auth/register
{
  "fullName": "Ahmet Yılmaz",
  "email": "ahmet@example.com",
  "password": "123456"
}
```

### Kullanıcı Girişi
```json
POST /api/Auth/login
{
  "email": "ahmet@example.com",
  "password": "123456"
}
```

### Yeni İlan Oluşturma
```json
POST /api/Listings
Authorization: Bearer <token>
{
  "title": "Merkezi Konumda 3+1 Daire",
  "description": "Şehir merkezinde, ulaşım imkanları mükemmel, yeni yapı 3+1 daire...",
  "city": "İstanbul",
  "district": "Kadıköy",
  "street": "Atatürk Caddesi",
  "apartmentNumber": "12",
  "roomType": "3+1",
  "price": 2500000
}
```

### İlan Arama ve Filtreleme
```
GET /api/Listings?searchTerm=merkezi&city=İstanbul&district=Kadıköy&roomType=3+1&minPrice=1000000&maxPrice=3000000&sortBy=price&sortOrder=asc&page=1&pageSize=10
```

### Filtreleme Parametreleri

| Parametre | Açıklama | Örnek |
|-----------|----------|--------|
| `searchTerm` | İlan başlığı ve açıklamasında arama | "merkezi", "yeni yapı" |
| `city` | Şehir filtresi | "İstanbul", "Ankara" |
| `district` | İlçe filtresi | "Kadıköy", "Çankaya" |
| `street` | Sokak filtresi | "Atatürk Caddesi" |
| `minPrice` | Minimum fiyat | 1000000 |
| `maxPrice` | Maksimum fiyat | 5000000 |
| `roomType` | Oda tipi | "1+1", "2+1", "3+1", "4+1", "Dublex", "Villa" |
| `sortBy` | Sıralama kriteri | "price", "date", "title" |
| `sortOrder` | Sıralama yönü | "asc", "desc" |
| `page` | Sayfa numarası | 1, 2, 3... |
| `pageSize` | Sayfa başına kayıt | 10, 20, 50 |

## Güvenlik

- Tüm şifreler hash'lenerek saklanır
- JWT token'lar 7 gün geçerlidir
- İlan işlemleri için kimlik doğrulama zorunludur
- Kullanıcılar sadece kendi ilanlarını düzenleyebilir/silebilir

## Veritabanı Yapısı

### Users Tablosu
- Id (Primary Key)
- FullName
- Email (Unique)
- PasswordHash

### Listings Tablosu
- Id (Primary Key)
- Title
- Description
- City
- District
- Street
- ApartmentNumber (Nullable)
- RoomType ("1+1", "2+1", "3+1", "4+1", "Dublex", vb.)
- Price
- CreatedAt
- UpdatedAt
- UserId (Foreign Key)

## Teknolojiler

- **Framework**: ASP.NET Core 9.0
- **Veritabanı**: SQL Server / LocalDB
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Token
- **Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI

## Geliştirme Notları

### Migration Komutları
```bash
# Yeni migration oluştur
dotnet ef migrations add <MigrationName>

# Veritabanını güncelle
dotnet ef database update

# Migration'ı geri al
dotnet ef database update <PreviousMigrationName>

# Migration'ı kaldır
dotnet ef migrations remove
```

### Test Etme
Swagger UI kullanarak API'yi test edebilirsiniz:
`https://localhost:7039/swagger`

### CORS Ayarları
Geliştirme ortamında tüm origin'lere izin verilmiştir. Production'da bu ayar güncellenmeli.

## Örnek Response'lar

### Başarılı İlan Listeleme
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "title": "Merkezi Konumda 3+1 Daire",
      "description": "Şehir merkezinde...",
      "city": "İstanbul",
      "district": "Kadıköy",
      "street": "Atatürk Caddesi",
      "apartmentNumber": "12",
      "roomType": "3+1",
      "price": 2500000,
      "createdAt": "2025-01-15T10:30:00Z",
      "updatedAt": "2025-01-15T10:30:00Z",
      "userId": 1,
      "user": {
        "id": 1,
        "fullName": "Ahmet Yılmaz",
        "email": "ahmet@example.com"
      }
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 1,
    "totalCount": 1
  }
}
```

### Hata Response'u
```json
{
  "success": false,
  "message": "İlan bulunamadı"
}
```

### Validation Hatası
```json
{
  "success": false,
  "message": "Geçersiz veriler",
  "errors": [
    "Başlık gereklidir",
    "Fiyat 0'dan büyük olmalıdır"
  ]
}
```
