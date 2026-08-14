namespace BKNova.Models
{
    public enum Semester{
      Ganjil,
      Genap
    }
    public class Tahun_Ajaran{
      public string Nama {get;set;} = string.Empty;
      public Semester Semester{get;set;}
      public bool Is_Active {get;set;}
    }
    public class Tahun_AjaranDTO{
      public int Id{get;set;}
      public string Nama {get;set;} = string.Empty;
      public Semester Semester {get;set;}
      public bool Is_Active {get;set;}
    }
    public class Jurusan{
      public string Nama {get;set;} = string.Empty;
      public string Kode{get;set;} = string.Empty;
    }
    public class JurusanDTO{
      public int Id {get;set;}
      public string Nama {get;set;} = string.Empty;
      public string Kode{get;set;} = string.Empty;
    }
    public class Kelas{
      public string Nama {get;set;} = string.Empty;
      public int Id_Jurusan {get;set;}
    }
    public class KelasDTO{
      public int Id {get;set;}
      public string Nama {get;set;} = string.Empty;
      public string Jurusan {get;set;} = string.Empty;
      public string Tingkat {get;set;} = string.Empty;
    }

}
