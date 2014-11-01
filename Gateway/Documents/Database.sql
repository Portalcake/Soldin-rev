-- MySQL dump 10.13  Distrib 5.5.9, for Win32 (x86)
--
-- Host: localhost    Database: soldinrev
-- ------------------------------------------------------
-- Server version	5.5.11

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `soldinrev`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `soldinrev` /*!40100 DEFAULT CHARACTER SET utf8 */;

USE `soldinrev`;

--
-- Table structure for table `accounts`
--

DROP TABLE IF EXISTS `accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `accounts` (
  `Name` varchar(50) NOT NULL,
  `Password` varchar(50) NOT NULL,
  `CreatedOn` datetime DEFAULT NULL,
  `LastLoginOn` datetime DEFAULT NULL,
  `Characters` int(11) DEFAULT '0',
  `CharacterSlots` int(11) DEFAULT '3',
  `SecondaryPassword` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`Name`),
  UNIQUE KEY `Name_UNIQUE` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts`
--

LOCK TABLES `accounts` WRITE;
/*!40000 ALTER TABLE `accounts` DISABLE KEYS */;
INSERT INTO `accounts` VALUES ('dhummel@gmail.com','',NULL,NULL,0,3,NULL);
/*!40000 ALTER TABLE `accounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_licenses`
--

DROP TABLE IF EXISTS `character_licenses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `character_licenses` (
  `Account` varchar(50) NOT NULL,
  `Character` int(10) unsigned NOT NULL,
  UNIQUE KEY `UNQ_AccountName_Character` (`Account`,`Character`),
  KEY `IDX_AccountName` (`Account`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `character_licenses`
--

LOCK TABLES `character_licenses` WRITE;
/*!40000 ALTER TABLE `character_licenses` DISABLE KEYS */;
INSERT INTO `character_licenses` VALUES ('dhummel@gmail.com',4),('dhummel@gmail.com',5);
/*!40000 ALTER TABLE `character_licenses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characters`
--

DROP TABLE IF EXISTS `characters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `characters` (
  `Account` varchar(50) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Class` smallint(6) NOT NULL DEFAULT '0',
  `StageLevel` smallint(6) NOT NULL DEFAULT '1',
  `StageExperience` int(11) NOT NULL DEFAULT '0',
  `PvpLevel` smallint(6) NOT NULL DEFAULT '1',
  `PvpExperience` int(11) NOT NULL DEFAULT '0',
  `WarLevel` smallint(6) NOT NULL DEFAULT '1',
  `WarExperience` int(11) NOT NULL DEFAULT '0',
  `CreatedOn` datetime DEFAULT NULL,
  `LastLoggedOn` datetime DEFAULT NULL,
  `SkillPoints` smallint(6) NOT NULL DEFAULT '0',
  `BagCount` smallint(6) NOT NULL DEFAULT '0',
  `BagMoney` bigint(20) NOT NULL DEFAULT '0',
  `BankBagCount` smallint(6) NOT NULL DEFAULT '0',
  `BankMoney` bigint(20) NOT NULL DEFAULT '0',
  `RebirthLevel` smallint(6) NOT NULL DEFAULT '0',
  `RebirthCount` smallint(6) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Name`),
  UNIQUE KEY `CharacterName_UNIQUE` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters`
--

LOCK TABLES `characters` WRITE;
/*!40000 ALTER TABLE `characters` DISABLE KEYS */;
INSERT INTO `characters` VALUES ('dhummel@gmail.com','Voldrin',6,54,1176123,1,0,1,0,'2012-04-08 20:38:11','0001-01-01 00:00:00',0,0,0,0,0,55,1);
/*!40000 ALTER TABLE `characters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `items`
--

DROP TABLE IF EXISTS `items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `items` (
  `Character` varchar(50) NOT NULL,
  `Hash` int(10) unsigned NOT NULL,
  `Quantity` tinyint(3) unsigned NOT NULL,
  `Bag` tinyint(3) unsigned NOT NULL,
  `Position` tinyint(3) unsigned NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `items`
--

LOCK TABLES `items` WRITE;
/*!40000 ALTER TABLE `items` DISABLE KEYS */;
INSERT INTO `items` VALUES ('Voldrin',24715729,1,99,0),('Voldrin',59339194,1,99,1),('Voldrin',58691405,1,99,2),('Voldrin',30142305,1,99,3),('Voldrin',40132307,1,99,4),('Voldrin',17622298,1,99,5),('Voldrin',57371887,1,99,6),('Voldrin',17159113,1,99,7),('Voldrin',15271122,1,99,8);
/*!40000 ALTER TABLE `items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `keybindings`
--

DROP TABLE IF EXISTS `keybindings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `keybindings` (
  `Account` varchar(50) NOT NULL,
  `Data` text NOT NULL,
  PRIMARY KEY (`Account`),
  UNIQUE KEY `Account_UNIQUE` (`Account`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sessions`
--

DROP TABLE IF EXISTS `sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sessions` (
  `Key` varchar(30) NOT NULL,
  `Account` varchar(50) DEFAULT NULL,
  `Character` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Key`),
  UNIQUE KEY `Key_UNIQUE` (`Key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `squares`
--

DROP TABLE IF EXISTS `squares`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `squares` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `Status` int(11) NOT NULL DEFAULT '1',
  `Type` int(11) NOT NULL DEFAULT '1',
  `Capacity` int(11) NOT NULL DEFAULT '70',
  `IP` varchar(15) NOT NULL DEFAULT '127.0.0.1',
  `Port` smallint(6) NOT NULL DEFAULT '15555',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `squares`
--

LOCK TABLES `squares` WRITE;
/*!40000 ALTER TABLE `squares` DISABLE KEYS */;
INSERT INTO `squares` VALUES (1,'Soldin',1,1,70,'127.0.0.1',15555);
/*!40000 ALTER TABLE `squares` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2012-04-15 14:00:39
