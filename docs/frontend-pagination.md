# Frontend Pagination Guide

This document describes the paginated API endpoints added for the high-priority modules in the project.

Base URL:

```text
/api/v1
```

## Common response format

All paginated endpoints return the same JSON structure:

```json
{
  "page": 1,
  "pageSize": 10,
  "totalItems": 120,
  "totalPages": 12,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "data": [
    {
      "id": 1,
      "nama": "Example"
    }
  ]
}
```

Query parameters:

- `page` : page number, minimum 1
- `pageSize` : row count per page, default 10, max 100

---

## 1) Siswa endpoints

### Get all students with pagination

```http
GET /api/v1/siswa/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

Required role:

- Admin

### Get students by class with pagination

```http
GET /api/v1/siswa/kelas/{id}/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

Example:

```http
GET /api/v1/siswa/kelas/5/paged?page=1&pageSize=20
```

Response example:

```json
{
  "page": 1,
  "pageSize": 20,
  "totalItems": 45,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "data": [
    {
      "idSiswa": 1,
      "nis": "2024001",
      "nisn": "0001234567",
      "kelamin": "Laki-Laki",
      "tempat_Tanggal_Lahir": "Jakarta, 2008-05-10",
      "nama": "Andi",
      "kelas": "XII",
      "tingkat": "12"
    }
  ]
}
```

---

## 2) BK endpoints

### Get all BK users with pagination

```http
GET /api/v1/bk/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

Required role:

- Admin

### Get all BK assignment tasks with pagination

```http
GET /api/v1/bk/tugas/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

### Get current BK assignment tasks for logged-in BK

```http
GET /api/v1/bk/tugas/me/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

Required role:

- BK

Response example:

```json
{
  "page": 1,
  "pageSize": 10,
  "totalItems": 25,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "data": [
    {
      "id": 2,
      "id_User_BK": 12,
      "nama_BK": "Rina BK",
      "id_Kelas": 5,
      "nama_Kelas": "XI-A",
      "tingkat": "11",
      "id_Tahun_Ajaran": 1,
      "tahunAjaran": "2024/2025",
      "is_Active": true,
      "assigned_At": "2026-09-01T08:30:00Z"
    }
  ]
}
```

---

## 3) Tiket endpoints

### Get student ticket list with pagination

```http
GET /api/v1/tiket/{idUser}/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

Required role:

- Siswa

### Get BK ticket list with pagination

```http
GET /api/v1/tiket/bk/{idUser}/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

Required role:

- BK

Response example:

```json
{
  "page": 1,
  "pageSize": 10,
  "totalItems": 52,
  "totalPages": 6,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "data": [
    {
      "id": 11,
      "siswa": "Andi",
      "tingkat": "12",
      "kelas": "XII-A",
      "jurusan": "TKJ",
      "judul": "Permintaan konsultasi",
      "isi": "Saya ingin berkonsultasi mengenai jadwal pelajaran.",
      "status": "Dikirim",
      "tempat": "Ruang BK",
      "tanggal_Perjanjian": "2026-09-20T10:00:00",
      "tanggal_Pembuatan": "2026-09-16T08:12:00"
    }
  ]
}
```

---

## 4) Kuesioner endpoints

### Get BK questionnaire list with pagination

```http
GET /api/v1/kuesioner/bk/{idUser}/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

Required role:

- BK

### Get student questionnaire list with pagination

```http
GET /api/v1/kuesioner/siswa/{idUser}/paged?page=1&pageSize=10
Authorization: Bearer <token>
```

Required role:

- Siswa

Response example:

```json
{
  "page": 1,
  "pageSize": 10,
  "totalItems": 30,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "data": [
    {
      "id": 2,
      "judul": "Kuesioner Motivasi Belajar",
      "deskripsi": "Survey untuk mengukur motivasi siswa",
      "kelas": "XI-A",
      "tahun_Ajaran": "2024/2025",
      "created_At": "2026-09-10T15:00:00",
      "sudah_Submit": false
    }
  ]
}
```

---

## Frontend usage pattern

```ts
const res = await fetch(
  `/api/v1/siswa/paged?page=1&pageSize=10`,
  {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  }
);

const payload = await res.json();
console.log(payload.data);
console.log(payload.totalPages);
console.log(payload.hasNextPage);
```

## Notes

- Pagination is zero-safe: invalid page or pageSize values are normalized automatically.
- `page` starts at 1, not 0.
- `pageSize` defaults to 10 and maxes out at 100.
- Use `totalPages` and `hasNextPage` to drive your frontend pager UI.
