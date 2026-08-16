namespace BKNova.Models
{
 
    public class BKDTO
    {
        public int Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
    public class TugasBK
    {
        public int Id_User_BK { get; set; }
        public int Id_Kelas { get; set; }
        public int Id_Tahun_Ajaran { get; set; }
        public bool Is_Active { get; set; } = true;
    }

    public class UpdateTugasBK
    {
        public int Id_Kelas { get; set; }
        public int Id_Tahun_Ajaran { get; set; }
        public bool Is_Active { get; set; }
    }

    public class TugasBKDTO
    {
        public int Id { get; set; }
        public int Id_User_BK { get; set; }
        public string Nama_BK { get; set; } = string.Empty;
        public int Id_Kelas { get; set; }
        public string Nama_Kelas { get; set; } = string.Empty;
        public string Tingkat { get; set; } = string.Empty;
        public int Id_Tahun_Ajaran { get; set; }
        public string TahunAjaran { get; set; } = string.Empty;
        public bool Is_Active { get; set; }
        public DateTime Assigned_At { get; set; }
    }
}
