-- phpMyAdmin SQL Dump
-- version 3.2.0.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Mar 04, 2011 at 10:49 PM
-- Server version: 5.1.36
-- PHP Version: 5.3.0

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `demo`
--
CREATE DATABASE `demo` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `demo`;

-- --------------------------------------------------------

--
-- Stand-in structure for view `capital_cities_view`
--
CREATE TABLE IF NOT EXISTS `capital_cities_view` (
`TopicName` varchar(200)
);
-- --------------------------------------------------------

--
-- Stand-in structure for view `celebrity_topics_view`
--
CREATE TABLE IF NOT EXISTS `celebrity_topics_view` (
`topicname` varchar(200)
);
-- --------------------------------------------------------

--
-- Table structure for table `correctfacts`
--

CREATE TABLE IF NOT EXISTS `correctfacts` (
  `ItemID` int(10) unsigned NOT NULL DEFAULT '0',
  `ItemName` varchar(500) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `topicID` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `globalvariables`
--

CREATE TABLE IF NOT EXISTS `globalvariables` (
  `key` varchar(50) COLLATE utf8_bin NOT NULL,
  `value` varchar(50) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `items`
--

CREATE TABLE IF NOT EXISTS `items` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(500) COLLATE utf8_bin NOT NULL,
  `topicID` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `topicID` (`topicID`),
  KEY `name` (`name`(255))
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=134 ;

-- --------------------------------------------------------

--
-- Stand-in structure for view `items_v`
--
CREATE TABLE IF NOT EXISTS `items_v` (
`ItemID` int(10) unsigned
,`ItemName` varchar(500)
,`topicID` int(11)
);
-- --------------------------------------------------------

--
-- Table structure for table `itemsmentions`
--

CREATE TABLE IF NOT EXISTS `itemsmentions` (
  `ID` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userID` varchar(100) COLLATE utf8_bin NOT NULL,
  `itemID` int(10) unsigned NOT NULL,
  `topicID` int(10) unsigned NOT NULL,
  `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`),
  KEY `itemID` (`itemID`),
  KEY `topicID` (`topicID`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=261 ;

-- --------------------------------------------------------

--
-- Stand-in structure for view `itemsmentions_v`
--
CREATE TABLE IF NOT EXISTS `itemsmentions_v` (
`ID` int(11) unsigned
,`UserId` int(11) unsigned
,`itemID` int(10) unsigned
,`TopicId` int(10) unsigned
,`time` timestamp
);
-- --------------------------------------------------------

--
-- Table structure for table `queryforms`
--

CREATE TABLE IF NOT EXISTS `queryforms` (
  `id` int(11) NOT NULL,
  `FormName` varchar(100) COLLATE latin1_bin NOT NULL,
  `FormDescription` varchar(1000) COLLATE latin1_bin DEFAULT NULL,
  `FormTitle` varchar(50) COLLATE latin1_bin DEFAULT NULL,
  `FormQuestionText` varchar(500) COLLATE latin1_bin DEFAULT NULL,
  `ParamsTableName` varchar(45) COLLATE latin1_bin DEFAULT NULL,
  `ParamsColNames` varchar(250) COLLATE latin1_bin DEFAULT NULL,
  `QueryBody` varchar(1000) COLLATE latin1_bin DEFAULT NULL,
  `TopicType` int(1) unsigned zerofill NOT NULL DEFAULT '0' COMMENT 'Possilbe values 0 - single correct 1 - multiple correct answers',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 COLLATE=latin1_bin COMMENT='Keeps information that describes Question Answering forms';

-- --------------------------------------------------------

--
-- Table structure for table `topics`
--

CREATE TABLE IF NOT EXISTS `topics` (
  `TopicId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `TopicName` varchar(200) COLLATE utf8_bin NOT NULL,
  `TopicText` varchar(200) COLLATE utf8_bin NOT NULL DEFAULT '-',
  `TopicType` smallint(1) unsigned zerofill NOT NULL,
  `Category` varchar(70) COLLATE utf8_bin NOT NULL DEFAULT '-',
  `Rank` double NOT NULL DEFAULT '0',
  `Value` varchar(100) COLLATE utf8_bin DEFAULT NULL,
  PRIMARY KEY (`TopicId`),
  UNIQUE KEY `id` (`TopicId`,`TopicName`),
  KEY `name` (`TopicName`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=17 ;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE IF NOT EXISTS `users` (
  `ID` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userID` varchar(50) COLLATE utf8_bin NOT NULL COMMENT 'actually user Email ',
  `Points` int(10) unsigned NOT NULL DEFAULT '0',
  `time` datetime NOT NULL,
  `dividendPoints` int(10) NOT NULL DEFAULT '0',
  `pointsSinceLastLogin` int(11) NOT NULL DEFAULT '0',
  `userName` varchar(100) COLLATE utf8_bin NOT NULL,
  `password` varchar(50) COLLATE utf8_bin DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `Points_4` (`Points`,`dividendPoints`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=23 ;

-- --------------------------------------------------------

--
-- Stand-in structure for view `users_v`
--
CREATE TABLE IF NOT EXISTS `users_v` (
`UserId` int(11) unsigned
);
-- --------------------------------------------------------

--
-- Structure for view `capital_cities_view`
--
DROP TABLE IF EXISTS `capital_cities_view`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `capital_cities_view` AS select `topics`.`TopicName` AS `TopicName` from `topics` where (`topics`.`TopicId` <= 10);

-- --------------------------------------------------------

--
-- Structure for view `celebrity_topics_view`
--
DROP TABLE IF EXISTS `celebrity_topics_view`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `celebrity_topics_view` AS select `topics`.`TopicName` AS `topicname` from `topics` where (`topics`.`TopicId` > 10);

-- --------------------------------------------------------

--
-- Structure for view `items_v`
--
DROP TABLE IF EXISTS `items_v`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `items_v` AS select `items`.`id` AS `ItemID`,`items`.`name` AS `ItemName`,`items`.`topicID` AS `topicID` from `items`;

-- --------------------------------------------------------

--
-- Structure for view `itemsmentions_v`
--
DROP TABLE IF EXISTS `itemsmentions_v`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `itemsmentions_v` AS select `im`.`ID` AS `ID`,`u`.`ID` AS `UserId`,`im`.`itemID` AS `itemID`,`im`.`topicID` AS `TopicId`,`im`.`time` AS `time` from (`itemsmentions` `im` join `users` `u`) where (`im`.`userID` = `u`.`userID`);

-- --------------------------------------------------------

--
-- Structure for view `users_v`
--
DROP TABLE IF EXISTS `users_v`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `users_v` AS select `users`.`ID` AS `UserId` from `users`;
