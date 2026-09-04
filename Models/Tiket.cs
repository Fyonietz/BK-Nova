namespace BKNova.Models
{
    public class StatusTiketDTO{
      public int Id {get;set;}
      public string Nama {get;set;} = string.Empty;
    }

    public class Tiket{
      public int Id_Siswa {get;set;}
      public int Id_BK {get;set;}
      public string Judul {get;set;} = string.Empty;
      public string Isi {get;set;} = string.Empty;
      public int Id_Status {get;set;}
    }

    public class TiketDTO{
      public int Id {get;set;}
      public string NamaSiswa{get;set;} = string.Empty;
      public string NamaBK {get;set;} = string.Empty;
      public string Judul {get;set;} =string.Empty;
      public string Isi {get;set;} = string.Empty;
      public string Status {get;set;} = string.Empty;
      public string Tempat {get;set;} = string.Empty;
      public DateTime? TanggalPerjanjian {get;set;}
      public DateTime? TanggalPembuatan {get;set;}
    }

    public class TiketUpdate{
      public int Id {get;set;}
      public int Id_Status{get;set;}
      public string Tempat {get;set;} = string.Empty;
      public DateTime? TanggalPerjanjian {get;set;}
    }
    public class RiwayatTiket{
      public int Id {get;set;}
      public int Id_Tiket {get;set;}
    }
}
