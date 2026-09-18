/*M!999999\- enable the sandbox mode */ 
-- MariaDB dump 10.20-12.3.3-MariaDB, for Linux (x86_64)
--
-- Host: localhost    Database: bk_nova
-- ------------------------------------------------------
-- Server version	12.3.3-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*M!100616 SET @OLD_NOTE_VERBOSITY=@@NOTE_VERBOSITY, NOTE_VERBOSITY=0 */;

DROP TABLE IF EXISTS `Riwayat_Tiket`;
DROP TABLE IF EXISTS `Jawaban_Kuesioner`;
DROP TABLE IF EXISTS `Status_Submit_Kuesioner`;
DROP TABLE IF EXISTS `Status_Submit_AUM`;
DROP TABLE IF EXISTS `Hasil_AUM`;
DROP TABLE IF EXISTS `Soal_Masalah`;
DROP TABLE IF EXISTS `Bidang_Masalah`;
DROP TABLE IF EXISTS `Opsi_Jawaban`;
DROP TABLE IF EXISTS `Soal_Kuesioner`;
DROP TABLE IF EXISTS `Kuesioner_Kelas`;
DROP TABLE IF EXISTS `Kuesioner`;
DROP TABLE IF EXISTS `Tiket`;
DROP TABLE IF EXISTS `Status_Tiket`;
DROP TABLE IF EXISTS `Tugas_BK`;
DROP TABLE IF EXISTS `Wali_Kelas`;
DROP TABLE IF EXISTS `Riwayat_Kelas_Siswa`;
DROP TABLE IF EXISTS `Siswa`;
DROP TABLE IF EXISTS `Kelas`;
DROP TABLE IF EXISTS `Jurusan`;
DROP TABLE IF EXISTS `Tahun_Ajaran`;
DROP TABLE IF EXISTS `User`;
DROP TABLE IF EXISTS `Roles`;

-- Roles
CREATE TABLE `Roles` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nama` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nama` (`Nama`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- User
CREATE TABLE `User` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nama` varchar(255) NOT NULL,
  `Id_Role` int(11) DEFAULT NULL,
  `Password` varchar(255) NOT NULL,
  `Refresh_Token` varchar(255) DEFAULT NULL,
  `Refresh_Token_Expired` timestamp NULL DEFAULT NULL,
  `Is_Active` tinyint(1) DEFAULT 1,
  `Created_At` timestamp NULL DEFAULT current_timestamp(),
  `Updated_At` timestamp NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `FCM_Token` text DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_user_role` (`Id_Role`),
  CONSTRAINT `fk_user_role` FOREIGN KEY (`Id_Role`) REFERENCES `Roles` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tahun_Ajaran
CREATE TABLE `Tahun_Ajaran` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nama` varchar(255) NOT NULL,
  `Semester` enum('Ganjil','Genap') NOT NULL,
  `Is_Active` tinyint(1) DEFAULT 0,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uq_tahun_semester` (`Nama`,`Semester`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Jurusan
CREATE TABLE `Jurusan` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nama` varchar(255) NOT NULL,
  `Kode` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nama` (`Nama`),
  UNIQUE KEY `Kode` (`Kode`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Kelas
CREATE TABLE `Kelas` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nama` varchar(255) NOT NULL,
  `Tingkat` enum('X','XI','XII') NOT NULL,
  `Id_Jurusan` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_Kelas_Jurusan` (`Id_Jurusan`),
  CONSTRAINT `fk_Kelas_Jurusan` FOREIGN KEY (`Id_Jurusan`) REFERENCES `Jurusan` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Siswa
CREATE TABLE `Siswa` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_User` int(11) DEFAULT NULL,
  `NISN` varchar(255) DEFAULT NULL,
  `NIS` varchar(255) DEFAULT NULL,
  `Jenis_Kelamin` enum('Laki-Laki','Perempuan') NOT NULL,
  `Tempat_Tanggal_Lahir` varchar(255) NOT NULL,
  `Id_Kelas` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_Siswa_User` (`Id_User`),
  KEY `fk_Siswa_Kelas` (`Id_Kelas`),
  CONSTRAINT `fk_Siswa_Kelas` FOREIGN KEY (`Id_Kelas`) REFERENCES `Kelas` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Siswa_User` FOREIGN KEY (`Id_User`) REFERENCES `User` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Riwayat_Kelas_Siswa
CREATE TABLE `Riwayat_Kelas_Siswa` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Siswa` int(11) DEFAULT NULL,
  `Id_Kelas` int(11) DEFAULT NULL,
  `Id_Tahun_Ajaran` int(11) DEFAULT NULL,
  `Is_Active` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `fk_Riwayat_Kelas_Siswa` (`Id_Siswa`),
  KEY `fk_Riwayat_Kelas_Kelas` (`Id_Kelas`),
  KEY `fk_Riwayat_Kelas_TA` (`Id_Tahun_Ajaran`),
  CONSTRAINT `fk_Riwayat_Kelas_Siswa` FOREIGN KEY (`Id_Siswa`) REFERENCES `Siswa` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Riwayat_Kelas_Kelas` FOREIGN KEY (`Id_Kelas`) REFERENCES `Kelas` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Riwayat_Kelas_TA` FOREIGN KEY (`Id_Tahun_Ajaran`) REFERENCES `Tahun_Ajaran` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Wali_Kelas
CREATE TABLE `Wali_Kelas` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_User` int(11) DEFAULT NULL,
  `Id_Kelas` int(11) DEFAULT NULL,
  `Id_Tahun_Ajaran` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_Wali_Kelas_User` (`Id_User`),
  KEY `fk_Wali_Kelas_Kelas` (`Id_Kelas`),
  KEY `fk_Wali_Kelas_Tahun_Ajaran` (`Id_Tahun_Ajaran`),
  CONSTRAINT `fk_Wali_Kelas_User` FOREIGN KEY (`Id_User`) REFERENCES `User` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Wali_Kelas_Kelas` FOREIGN KEY (`Id_Kelas`) REFERENCES `Kelas` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Wali_Kelas_Tahun_Ajaran` FOREIGN KEY (`Id_Tahun_Ajaran`) REFERENCES `Tahun_Ajaran` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tugas_BK
CREATE TABLE `Tugas_BK` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_User_BK` int(11) DEFAULT NULL,
  `Id_Kelas` int(11) DEFAULT NULL,
  `Id_Tahun_Ajaran` int(11) DEFAULT NULL,
  `Assigned_At` timestamp NULL DEFAULT current_timestamp(),
  `Is_Active` tinyint(1) DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `fk_Tugas_Kelas` (`Id_Kelas`),
  KEY `fk_Tugas_TahunAjaran` (`Id_Tahun_Ajaran`),
  KEY `fk_Tugas_User` (`Id_User_BK`),
  CONSTRAINT `fk_Tugas_Kelas` FOREIGN KEY (`Id_Kelas`) REFERENCES `Kelas` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Tugas_TahunAjaran` FOREIGN KEY (`Id_Tahun_Ajaran`) REFERENCES `Tahun_Ajaran` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Tugas_User` FOREIGN KEY (`Id_User_BK`) REFERENCES `User` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Status_Tiket
CREATE TABLE `Status_Tiket` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nama` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tiket
CREATE TABLE `Tiket` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Siswa` int(11) DEFAULT NULL,
  `Id_BK` int(11) DEFAULT NULL,
  `Judul` varchar(255) NOT NULL,
  `Isi` text NOT NULL,
  `Tanggal_Pembuatan` timestamp NULL DEFAULT current_timestamp(),
  `Tanggal_Perjanjian` datetime DEFAULT NULL,
  `Id_Status` int(11) DEFAULT NULL,
  `Tempat` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_Tiket_BK` (`Id_BK`),
  KEY `fk_Tiket_Siswa` (`Id_Siswa`),
  KEY `fk_Tiket_Status` (`Id_Status`),
  CONSTRAINT `fk_Tiket_BK` FOREIGN KEY (`Id_BK`) REFERENCES `User` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Tiket_Siswa` FOREIGN KEY (`Id_Siswa`) REFERENCES `Siswa` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Tiket_Status` FOREIGN KEY (`Id_Status`) REFERENCES `Status_Tiket` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Riwayat_Tiket
CREATE TABLE `Riwayat_Tiket` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Tiket` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_Riwayat_Tiket` (`Id_Tiket`),
  CONSTRAINT `fk_Riwayat_Tiket` FOREIGN KEY (`Id_Tiket`) REFERENCES `Tiket` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bidang_Masalah
CREATE TABLE `Bidang_Masalah` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Kode` varchar(255) NOT NULL,
  `Nama` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Soal_Masalah
CREATE TABLE `Soal_Masalah` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Bidang_Masalah` int(11) DEFAULT NULL,
  `Pertanyaan` text NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_Soal_Bidang` (`Id_Bidang_Masalah`),
  CONSTRAINT `fk_Soal_Bidang` FOREIGN KEY (`Id_Bidang_Masalah`) REFERENCES `Bidang_Masalah` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Hasil_AUM
CREATE TABLE `Hasil_AUM` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Siswa` int(11) DEFAULT NULL,
  `Id_Soal_Masalah` int(11) DEFAULT NULL,
  `Id_Tahun_Ajaran` int(11) DEFAULT NULL,
  `Creted_At` timestamp NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`),
  KEY `fk_Hasil_Siswa` (`Id_Siswa`),
  KEY `fk_Hasil_SoalMasalah` (`Id_Soal_Masalah`),
  KEY `fk_Hasil_TahunAjaran` (`Id_Tahun_Ajaran`),
  CONSTRAINT `fk_Hasil_Siswa` FOREIGN KEY (`Id_Siswa`) REFERENCES `Siswa` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Hasil_SoalMasalah` FOREIGN KEY (`Id_Soal_Masalah`) REFERENCES `Soal_Masalah` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Hasil_TahunAjaran` FOREIGN KEY (`Id_Tahun_Ajaran`) REFERENCES `Tahun_Ajaran` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Status_Submit_AUM
CREATE TABLE `Status_Submit_AUM` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Siswa` int(11) DEFAULT NULL,
  `Id_Tahun_Ajaran` int(11) DEFAULT NULL,
  `Submitted_At` timestamp NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`),
  KEY `fk_Status_Siswa` (`Id_Siswa`),
  KEY `fk_Status_TahunAjaran` (`Id_Tahun_Ajaran`),
  CONSTRAINT `fk_Status_Siswa` FOREIGN KEY (`Id_Siswa`) REFERENCES `Siswa` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Status_TahunAjaran` FOREIGN KEY (`Id_Tahun_Ajaran`) REFERENCES `Tahun_Ajaran` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Kuesioner
CREATE TABLE `Kuesioner` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_User_BK` int(11) DEFAULT NULL,
  `Id_Kelas` int(11) DEFAULT NULL,
  `Id_Tahun_Ajaran` int(11) DEFAULT NULL,
  `Judul` varchar(255) NOT NULL,
  `Deskripsi` text DEFAULT NULL,
  `Created_At` timestamp NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`),
  KEY `fk_Kuesioner_BK` (`Id_User_BK`),
  KEY `fk_Kuesioner_Kelas` (`Id_Kelas`),
  KEY `fk_Kuesioner_TahunAjaran` (`Id_Tahun_Ajaran`),
  CONSTRAINT `fk_Kuesioner_BK` FOREIGN KEY (`Id_User_BK`) REFERENCES `User` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Kuesioner_Kelas` FOREIGN KEY (`Id_Kelas`) REFERENCES `Kelas` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Kuesioner_TahunAjaran` FOREIGN KEY (`Id_Tahun_Ajaran`) REFERENCES `Tahun_Ajaran` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Kuesioner_Kelas
CREATE TABLE `Kuesioner_Kelas` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Kuesioner` int(11) DEFAULT NULL,
  `Id_Kelas` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_KuesionerKuesioner` (`Id_Kuesioner`),
  KEY `fk_KuesionerKelas_Kelas` (`Id_Kelas`),
  CONSTRAINT `fk_KuesionerKuesioner` FOREIGN KEY (`Id_Kuesioner`) REFERENCES `Kuesioner` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_KuesionerKelas_Kelas` FOREIGN KEY (`Id_Kelas`) REFERENCES `Kelas` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Soal_Kuesioner
CREATE TABLE `Soal_Kuesioner` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Kuesioner` int(11) DEFAULT NULL,
  `Pertanyaan` text NOT NULL,
  `Tipe` enum('Pilihan Ganda','Esai') NOT NULL,
  `Urutan` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_Soal_Kuesioner` (`Id_Kuesioner`),
  CONSTRAINT `fk_Soal_Kuesioner` FOREIGN KEY (`Id_Kuesioner`) REFERENCES `Kuesioner` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Opsi_Jawaban
CREATE TABLE `Opsi_Jawaban` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Soal` int(11) DEFAULT NULL,
  `Teks` varchar(255) NOT NULL,
  `Urutan` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_Opsi_Soal` (`Id_Soal`),
  CONSTRAINT `fk_Opsi_Soal` FOREIGN KEY (`Id_Soal`) REFERENCES `Soal_Kuesioner` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Jawaban_Kuesioner
CREATE TABLE `Jawaban_Kuesioner` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Siswa` int(11) DEFAULT NULL,
  `Id_Soal` int(11) DEFAULT NULL,
  `Id_Opsi` int(11) DEFAULT NULL,
  `Teks_Jawaban` text DEFAULT NULL,
  `Answered_At` timestamp NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`),
  KEY `fk_Jawaban_Siswa` (`Id_Siswa`),
  KEY `fk_Jawaban_Soal` (`Id_Soal`),
  KEY `fk_Jawaban_Opsi` (`Id_Opsi`),
  CONSTRAINT `fk_Jawaban_Siswa` FOREIGN KEY (`Id_Siswa`) REFERENCES `Siswa` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Jawaban_Soal` FOREIGN KEY (`Id_Soal`) REFERENCES `Soal_Kuesioner` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Jawaban_Opsi` FOREIGN KEY (`Id_Opsi`) REFERENCES `Opsi_Jawaban` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Status_Submit_Kuesioner
CREATE TABLE `Status_Submit_Kuesioner` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Id_Siswa` int(11) DEFAULT NULL,
  `Id_Kuesioner` int(11) DEFAULT NULL,
  `Submitted_At` timestamp NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`Id`),
  KEY `fk_Submit_Siswa` (`Id_Siswa`),
  KEY `fk_Submit_Kuesioner` (`Id_Kuesioner`),
  CONSTRAINT `fk_Submit_Siswa` FOREIGN KEY (`Id_Siswa`) REFERENCES `Siswa` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_Submit_Kuesioner` FOREIGN KEY (`Id_Kuesioner`) REFERENCES `Kuesioner` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*M!100616 SET NOTE_VERBOSITY=@OLD_NOTE_VERBOSITY */;

-- Repaired on 2026-09-18
