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

    public class TiketSiswaDTO{
      public int Id {get;set;}
      public string BK {get;set;} = string.Empty;
      public string Judul {get;set;} =string.Empty;
      public string Isi {get;set;} = string.Empty;
      public string Status {get;set;} = string.Empty;
      public string Tempat {get;set;} = string.Empty;
      public string Tanggal_Perjanjian {get;set;}
      public string Tanggal_Pembuatan {get;set;}
    }
            
    public class TiketBKDTO{
      public int Id {get;set;}
      public string Siswa {get;set;} = string.Empty;
      public string Tingkat {get;set;} = string.Empty;
      public string Kelas {get;set;} = string.Empty;
      public string Jurusan{get;set;} = string.Empty;
      public string Judul {get;set;} =string.Empty;
      public string Isi {get;set;} = string.Empty;
      public string Status {get;set;} = string.Empty;
      public string Tempat {get;set;} = string.Empty;
      public string Tanggal_Perjanjian {get;set;}
      public string Tanggal_Pembuatan {get;set;}
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
