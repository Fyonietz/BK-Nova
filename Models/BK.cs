namespace BKNova.Models
{
    public class TugasBK
    {
        public int Id_BK { get; set; }
        public int Id_Kelas { get; set; }
        public int Id_TahunAjaran { get; set; }
        public bool Is_Active { get; set; }
    }
    public class BKDTO{
      public int Id {get;set;}
      public string Nama {get;set;} = string.Empty;
      public string Role {get;set;}  = string.Empty;
    }
}
