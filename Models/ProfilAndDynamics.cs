namespace BKNova.Models
{
    public enum JenisKelamin
    {
        Laki,
        Perempuan
    }


    public class Siswa
    {
        public string NISN { get; set; } = string.Empty;
        public string NIS { get; set; } = string.Empty;
        public int Id_Kelas { get; set; }
        public JenisKelamin Kelamin { get; set; }
        public string Tempat_Tanggal_Lahir { get; set; } = string.Empty;
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
    }
    public class RegisterSiswa
    {
        public User user { get; set; } = new();
        public Siswa siswa { get; set; } = new();
    }
    public class UpdateSiswa
    {
        public int Id { get; set; }
        public int Id_Kelas { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NISN { get; set; } = string.Empty;
        public string NIS { get; set; } = string.Empty;
        public JenisKelamin Kelamin { get; set; }
        public string Tempat_Tanggal_Lahir { get; set; } = string.Empty;
    }

    public class SiswaDTO
    {
        public int Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string NISN { get; set; } = string.Empty;
        public string NIS { get; set; } = string.Empty;
        public string Kelamin { get; set; } = string.Empty;
        public string Tempat_Tanggal_Lahir { get; set; } = string.Empty;
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public string Refresh_Token { get; set; } = string.Empty;
        public DateTime? Refresh_Token_Expired { get; set; }
    }
    public class WaliKelas
    {
        public int Id_Kelas { get; set; }
        public int Id_Tahun_Ajaran { get; set; }
    }

    public class RegisterWaliKelas
    {
        public User user { get; set; } = new();
        public WaliKelas wali_kelas { get; set; } = new();
    }
    public class UpdateWaliKelas
    {
        public string Nama { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Id_Kelas { get; set; }
        public int Id_Tahun_Ajaran { get; set; }
    }
    public class WaliKelasDTO
    {
        public int Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Kelas { get; set; } = string.Empty;
        public string Tahun_Ajaran { get; set; } = string.Empty;
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public string Refresh_Token { get; set; } = string.Empty;
        public DateTime? Refresh_Token_Expired { get; set; }
    }


    public class RiwayatKelasSiswa
    {
        public int Id_User { get; set; }
        public int Id_Kelas { get; set; }
        public int Id_Tahun_Ajaran { get; set; }
        public bool Is_Active { get; set; } = true;
    }

    public class UpdateRiwayatKelasSiswa
    {
        public int Id_Kelas { get; set; }
        public int Id_Tahun_Ajaran { get; set; }
        public bool Is_Active { get; set; }
    }

    public class RiwayatKelasSiswaDTO
    {
        public int Id { get; set; }
        public int Id_User { get; set; }
        public string Nama_Siswa { get; set; } = string.Empty;
        public int Id_Kelas { get; set; }
        public string Nama_Kelas { get; set; } = string.Empty;
        public int Id_Tahun_Ajaran { get; set; }
        public bool Is_Active { get; set; }
    }

    public class PromoteKelas
    {
        public int Id_Kelas_Lama { get; set; }
        public int Id_Tahun_Ajaran_Baru { get; set; }
    }
}//Namespace
