-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: butce_takip
-- ------------------------------------------------------
-- Server version	8.0.42

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `budget_entries`
--

DROP TABLE IF EXISTS `budget_entries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `budget_entries` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(100) DEFAULT NULL,
  `category` varchar(100) DEFAULT NULL,
  `amount` decimal(10,2) DEFAULT NULL,
  `description` text,
  `date` date DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `budget_entries`
--

LOCK TABLES `budget_entries` WRITE;
/*!40000 ALTER TABLE `budget_entries` DISABLE KEYS */;
INSERT INTO `budget_entries` VALUES (1,'qq','Gıda',-2000.00,'Yemek','2025-05-22'),(2,'qq','Ulaşım',-1000.00,'otobüs','2025-05-22'),(3,'qq','Faturalar',-3000.00,'elektrik su doğalgaz internet telefon toplam','2025-05-22'),(4,'qq','Eğlence',-2000.00,'cartcurt','2025-05-22'),(5,'qq','Sağlık',-1000.00,'bakım','2025-05-22'),(6,'qq','Kira',-5000.00,'mayıs kira','2025-05-22'),(7,'qq','Diğer',-500.00,'ıvır zıvır','2025-05-22'),(8,'qq','Harçlık',12500.00,'babamsağolsun','2025-05-22'),(9,'qq','Burs',3000.00,'mayıs kyk','2025-05-22'),(10,'qq','Gıda',-3000.00,'yemek','2025-04-18'),(11,'qq','Ulaşım',-1000.00,'otobüs','2025-04-18'),(12,'qq','Faturalar',-2500.00,'faturalar','2025-04-18'),(13,'qq','Eğlence',-1600.00,'etkinlikler','2025-04-18'),(14,'qq','Sağlık',-750.00,'bakım','2025-04-18'),(15,'qq','Kira',-5000.00,'nisan kira','2025-04-18'),(16,'qq','Diğer',-400.00,'ıvır zıvır','2025-04-18'),(17,'qq','Maaş',12500.00,'Harçlığım','2025-04-18'),(18,'qq','Burs',3000.00,'nisan kyk','2025-04-18');
/*!40000 ALTER TABLE `budget_entries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(100) NOT NULL,
  `password` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'qq','qq');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-05-29  2:41:52
