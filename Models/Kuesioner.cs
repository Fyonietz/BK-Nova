namespace BKNova.Models
{
    // Request
    public class Kuesioner
    {
        public string Judul { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public int Id_Kelas { get; set; }
        public int Id_Tahun_Ajaran { get; set; }
        public List<SoalKuesioner> Soal { get; set; } = new();
    }

    public class SoalKuesioner
    {
        public string Pertanyaan { get; set; } = string.Empty;
        public string Tipe { get; set; } = string.Empty; // "Pilihan Ganda" atau "Esai"
        public int Urutan { get; set; }
        public List<OpsiJawaban> Opsi { get; set; } = new();
    }

    public class OpsiJawaban
    {
        public string Teks { get; set; } = string.Empty;
        public int Urutan { get; set; }
    }

    public class JawabanKuesioner
    {
        public int Id_Soal { get; set; }
        public int? Id_Opsi { get; set; }        // PG
        public string? Teks_Jawaban { get; set; } // Esai
    }

    // DTO Response
    public class KuesionerDTO
    {
        public int Id { get; set; }
        public string Judul { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string Tahun_Ajaran { get; set; } = string.Empty;
        public string Created_At { get; set; } = string.Empty;
        public bool Sudah_Submit { get; set; }
    }

    public class KuesionerDetailDTO
    {
        public int Id { get; set; }
        public string Judul { get; set; } = string.Empty;
        public string Deskripsi { get; set; } = string.Empty;
        public List<SoalDetailDTO> Soal { get; set; } = new();
    }

    public class SoalDetailDTO
    {
        public int Id { get; set; }
        public string Pertanyaan { get; set; } = string.Empty;
        public string Tipe { get; set; } = string.Empty;
        public int Urutan { get; set; }
        public List<OpsiDetailDTO> Opsi { get; set; } = new();
    }

    public class OpsiDetailDTO
    {
        public int Id { get; set; }
        public string Teks { get; set; } = string.Empty;
        public int Urutan { get; set; }
    }
    public class JawabanSiswaDTO
    {
        public string Pertanyaan { get; set; } = string.Empty;
        public string Tipe { get; set; } = string.Empty;
        public string? Jawaban_PG { get; set; }
        public string? Jawaban_Esai { get; set; }
    }
}
