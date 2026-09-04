CREATE DATABASE IF NOT EXISTS `bk_nova` CHARACTER SET utf8mb4 COLLATE  utf8mb4_unicode_ci;

USE bk_booster;
---Accountibilty---
CREATE TABLE IF NOT EXISTS Roles(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Nama VARCHAR(255) NOT NULL UNIQUE
);

INSERT INTO Roles(Nama) VALUES('Admin'),('Guru BK'),('Wali Kelas'),('Siswa');

CREATE TABLE IF NOT EXISTS User(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Nama VARCHAR(255) NOT NULL,
  Id_Role INT,
  Password VARCHAR(255) NOT NULL,
  Refresh_Token VARCHAR(255) DEFAULT NULL,
  Refresh_Token_Expired TIMESTAMP NULL DEFAULT NULL,
  Is_Active tinyint(1) DEFAULT 1,
  Created_At TIMESTAMP NULL DEFAULT current_timestamp(),
  Updated_At TIMESTAMP NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),

  CONSTRAINT `fk_user_role` FOREIGN KEY(Id_Role) REFERENCES Roles(Id)
);

---Class Master---
CREATE TABLE IF NOT EXISTS Tahun_Ajaran(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Nama VARCHAR(255) NOT NULL UNIQUE,
  Semester enum('Ganjil','Genap') NOT NULL,
  Is_Active tinyint(1) DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Jurusan(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Nama VARCHAR(255) NOT NULL UNIQUE,
  Kode VARCHAR(255) NOT NULL UNIQUE
);
 
CREATE TABLE IF NOT EXISTS Kelas(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Nama VARCHAR(255) NOT NULL,
  Id_Jurusan INT DEFAULT NULL,
  CONSTRAINT fk_Kelas_Jurusan FOREIGN KEY(Id_Jurusan) REFERENCES Jurusan(Id) ON DELETE SET NULL
);


--Dynamics And Profil--
CREATE TABLE IF NOT EXISTS Siswa(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_User INT,
  NISN VARCHAR(255) DEFAULT NULL,
  NIS VARCHAR(255) DEFAULT NULL,
  Jenis_Kelamin enum("Laki-Laki","Perempuan") NOT NULL,
  Tempat_Tanggal_Lahir VARCHAR(255) NOT NULL
  CONSTRAINT fk_Siswa_User FOREIGN KEY (ID_User)
  REFERENCES User(Id) ON DELETE SET NULL
) 

CREATE TABLE IF NOT EXISTS Wali_Kelas(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_User INT,
  Id_Kelas INT,
  Id_Tahun_Ajaran INT,

  CONSTRAINT fk_Wali_Kelas_User FOREIGN KEY(Id_User)
  REFERENCES User(Id) ON DELETE SET NULL,

  CONSTRAINT fk_Wali_Kelas_Kelas FOREIGN KEY(Id_Kelas)
  REFERENCES Kelas(Id) ON DELETE SET NULL,
  
  CONSTRAINT fk_Wali_Kelas_Tahun_Ajaran FOREIGN KEY(Id_Tahun_Ajaran)
  REFERENCES Tahun_Ajaran(Id) ON DELETE SET NULL
)

 
CREATE TABLE IF NOT EXISTS Riwayat_Kelas_Siswa(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_Siswa INT,
  Id_Kelas INT,
  Id_Tahun_Ajaran INT,
  Is_Active tinyint(1) DEFAULT 1

  CONSTRAINT fk_Riwayat_Kelas_Siswa FOREIGN KEY(Id_Siswa)
  REFERENCES Siswa(Id) ON DELETE SET NULL,

  CONSTRAINT fk_Riwayat_Kelas_Kelas FOREIGN KEY(Id_Kelas)
  REFERENCES Kelas(Id) ON DELETE SET NULL,

  CONSTRAINT fk_Riwayat_Kelas_Kelas FOREIGN KEY(Id_Tahun_Ajaran)
  REFERENCES Tahun_Ajaran(Id) ON DELETE SET NULL,

)




//AUM
CREATE TABLE IF NOT EXISTS Bidang_Masalah(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Kode VARCHAR(255) NOT NULL,
  Nama VARCHAR(255) NOT NULL 
)
 
CREATE TABLE IF NOT EXISTS Soal_Masalah(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_Bidang_Masalah INT,
  Pertanyaan TEXT NOT NULL,
  
  CONSTRAINT fk_Soal_Bidang FOREIGN KEY(Id_Bidang_Masalah)
  REFERENCES Bidang_Masalah(Id)
)

CREATE TABLE IF NOT EXISTS Status_Submit_AUM(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_Siswa INT,
  Id_Tahun_Ajaran INT,
  Submitted_At TIMESTAMP NULL DEFAULT current_timestamp(),
                              
  CONSTRAINT fk_Status_Siswa FOREIGN KEY(Id_Siswa)
  REFERENCES Siswa(Id),

  CONSTRAINT fk_Status_TahunAjaran FOREIGN KEY(Id_Tahun_Ajaran)
  REFERENCES Tahun_Ajaran(Id)
)


CREATE TABLE IF NOT EXISTS Hasil_AUM(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_Siswa INT,
  Id_Soal_Masalah INT,
  Id_Tahun_Ajaran INT,
  Creted_At TIMESTAMP NULL DEFAULT current_timestamp(),
                              
  CONSTRAINT fk_Hasil_Siswa FOREIGN KEY(Id_Siswa)
  REFERENCES Siswa(Id),

  CONSTRAINT fk_Hasil_SoalMasalah FOREIGN KEY(Id_Soal_Masalah)
  REFERENCES Soal_Masalah(Id),

  CONSTRAINT fk_Hasil_TahunAjaran FOREIGN KEY(Id_Tahun_Ajaran)
  REFERENCES Tahun_Ajaran(Id)
)

CREATE TABLE IF NOT EXISTS Tugas_BK(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_User_BK INT,
  Id_Kelas INT,
  Id_Tahun_Ajaran INT,
  Assigned_At TIMESTAMP NULL DEFAULT current_timestamp(),
  Is_Active tinyint(1) DEFAULT 1,


  CONSTRAINT fk_Tugas_User FOREIGN KEY(Id_User_BK)
  REFERENCES User(Id),
  
  CONSTRAINT fk_Tugas_Kelas FOREIGN KEY(Id_Kelas)
  REFERENCES Kelas(Id),

  CONSTRAINT fk_Tugas_TahunAjaran FOREIGN KEY(Id_Tahun_Ajaran)
  REFERENCES Tahun_Ajaran(Id),
)


//Tiket 
CREATE TABLE IF NOT EXISTS Status_Tiket(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Nama VARCHAR(255) NOT NULL
)
INSERT INTO Status_Tiket(Nama) VALUES ('Dikirim'),('Disetujui'),('Ditunda'),('Dibatalkan'),('Selesai');
CREATE TABLE IF NOT EXISTS Tiket(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_Siswa INT,
  Id_BK INT,
  Judul VARCHAR(255) NOT NULL,
  Isi TEXT  NOT NULL,
  Tanggal_Pembuatan TIMESTAMP NULL DEFAULT current_timestamp(),
  Tanggal_Perjanjian DATETIME NULL DEFAULT NULL,
  Id_Status INT,
  Tempat VARCHAR(255),

  CONSTRAINT fk_Tiket_Siswa FOREIGN KEY(Id_Siswa)
  REFERENCES Siswa(Id),

  
  CONSTRAINT fk_Tiket_BK FOREIGN KEY(Id_BK)
  REFERENCES User(Id),

  CONSTRAINT fk_Tiket_Status FOREIGN KEY(Id_Status)
  REFERENCES Status_Tiket(Id)
)

CREATE TABLE IF NOT EXISTS Riwayat_Tiket(
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Id_Tiket INT,

  CONSTRAINT fk_Riwayat_Tiket FOREIGN KEY(Id_Tiket)
  REFERENCES Tiket(Id)
)
