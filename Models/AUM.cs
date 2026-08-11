namespace BKNova.Models
{
    public class BidangMasalahDTO{
      public int Id {get;set;}
      public string Kode {get;set;} = string.Empty; 
      public string Nama {get;set;} = string.Empty; 
    }

    public class SoalMasalahDTO{
      public int Id {get;set;}
      public string Kode {get;set;} = string.Empty;
      public string BidangMasalah {get;set;} = string.Empty;
      public string Pertanyaan {get;set;} = string.Empty;
    }
      
}
