namespace BKNova.Models
{
    public class BidangMasalahDTO
    {
        public int Id { get; set; }
        public string Kode { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
    }

    public class SoalMasalahDTO
    {
        public int Id { get; set; }
        public string Kode { get; set; } = string.Empty;
        public string BidangMasalah { get; set; } = string.Empty;
        public string Pertanyaan { get; set; } = string.Empty;
    }

    public class SubmitAUM
    {
        public int Id_User { get; set; }
        public int Id_Tahun_Ajaran { get; set; }
        public List<int> Id_Soal_Masalah_Terpilih { get; set; } = new();
    }
    public class HasilAUMFlat
    {
        public int Id { get; set; }
        public int IdSiswa { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string NIS {get;set;} = string.Empty;
        public string NISN {get;set;} = string.Empty;
        public string WaktuMengisi{get;set;} = string.Empty;
        public string Tingkat { get; set; } = string.Empty;
        public string Kode_Bidang { get; set; } = string.Empty;
        public string Nama_Bidang { get; set; } = string.Empty;
        public string Pilihan { get; set; } = string.Empty;
    }


    // Hasil setelah dikelompokkan
    public class BidangGrouped
    {
        public string Kode_Bidang { get; set; } = string.Empty;
        public string Nama_Bidang { get; set; } = string.Empty;
        public List<string> Pilihan { get; set; } = new();
    }

    public class HasilAUMGrouped
    {
        public int IdSiswa { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string Tingkat { get; set; } = string.Empty;
        public string NIS {get;set;} = string.Empty;
        public string NISN {get;set;} = string.Empty;
        public string WaktuMengisi{get;set;} = string.Empty;
        public List<BidangGrouped> Bidang { get; set; } = new();
    }


}
