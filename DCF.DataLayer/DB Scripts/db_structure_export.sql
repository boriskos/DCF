-- phpMyAdmin SQL Dump
-- version 3.2.0.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Feb 23, 2011 at 08:40 AM
-- Server version: 5.1.36
-- PHP Version: 5.3.0

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

-- --------------------------------------------------------

--
-- Table structure for table `globalvariables`
--

CREATE TABLE IF NOT EXISTS `globalvariables` (
  `key` varchar(50) COLLATE utf8_bin NOT NULL,
  `value` varchar(50) COLLATE utf8_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Dumping data for table `globalvariables`
--

INSERT INTO `globalvariables` (`key`, `value`) VALUES
('currentCategoryID', '94');

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
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=8235 ;

--
-- Stand-in structure for view `items_v`
--
CREATE OR REPLACE VIEW `items_v` AS 
select `items`.`id` AS `ItemID`,`items`.`name` AS `ItemName`,`items`.`topicID` AS `topicID` 
from `items`;
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
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=41 ;

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
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=13055 ;


--
-- Stand-in structure for view `itemsmentions_v`
--
CREATE OR REPLACE VIEW `itemsmentions_v` AS 
select `im`.`ID` AS `ID`,`u`.`ID` AS `UserId`,`im`.`itemID` AS `itemID`,`im`.`time` AS `time` 
from (`itemsmentions` `im` join `users` `u`) where (`im`.`userID` = `u`.`userID`);

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
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 COLLATE=latin1_bin COMMENT='Keeps information that describes Question Answering forms';

--
-- Dumping data for table `queryforms`
--

INSERT INTO `queryforms` (`id`, `FormName`, `FormDescription`, `FormTitle`, `FormQuestionText`, `ParamsTableName`, `ParamsColNames`, `QueryBody`) VALUES
(1, 'Query Guess', 'Queries guess topics', 'Gueeses', 'Please select topic to query: {0}', 'topics', 'TopicName', 'SELECT it.name AS ItemName, count( * ) AS Support FROM items it, itemsmentions im WHERE it.id = im.itemid AND im.topicid in (select TopicId from topics where TopicName = ''{0}'') GROUP BY it.name ORDER BY Support DESC LIMIT 0 , 15'),
(2, 'Query Cleaning', 'Queries cleaning results on guess table', 'Cleaning Results', 'Choose topic to see items in it {0}', 'Topics', 'TopicName', 'select i.name as Name,  sf.score as Score from items i, topics t, scoredfacts sf where sf.topicid=t.topicid and t.topicname=''{0}'' and i.id=sf.itemid order by score desc LIMIT 0, 20');

-- --------------------------------------------------------

--
-- Table structure for table `repkeyresults`
--

CREATE TABLE IF NOT EXISTS `repkeyresults` (
  `FactId` int(11) unsigned NOT NULL
) ENGINE=MEMORY DEFAULT CHARSET=latin1;

--
-- Dumping data for table `repkeyresults`
--


-- --------------------------------------------------------

--
-- Table structure for table `scoredfacts`
--

CREATE TABLE IF NOT EXISTS `scoredfacts` (
  `ItemID` int(11) unsigned NOT NULL,
  `TopicID` int(11) unsigned NOT NULL,
  `Factor` int(11) NOT NULL,
  `Score` double NOT NULL,
  `Category` varchar(70) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `Correctness` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`ItemID`),
  KEY `sfTopicID_fkey` (`TopicID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

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
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=131 ;



-- --------------------------------------------------------

--
-- Stand-in structure for view `users_v`
--
CREATE OR REPLACE VIEW `users_v` AS select `users`.`ID` AS `UserId` from `users`;
-- --------------------------------------------------------

--
-- Table structure for table `userscores`
--

CREATE TABLE IF NOT EXISTS `userscores` (
  `UserID` int(11) unsigned NOT NULL,
  `Belief` double NOT NULL,
  `Version` int(11) DEFAULT NULL,
  `NumOfFacts` int(11) NOT NULL,
  PRIMARY KEY (`UserID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `userscoreshistory`
--

CREATE TABLE IF NOT EXISTS `userscoreshistory` (
  `Version` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `UserScore` double NOT NULL,
  KEY `userscoreshistory_userIdx` (`UserId`)
) ENGINE=MEMORY DEFAULT CHARSET=latin1;

--
-- Dumping data for table `userscoreshistory`
--


-- --------------------------------------------------------

--
-- Structure for view `items_v`
--
-- DROP TABLE IF EXISTS `items_v`;

-- CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `items_v` AS select `items`.`id` AS `ItemID`,`items`.`name` AS `ItemName`,`items`.`topicID` AS `topicID` from `items`;

-- --------------------------------------------------------

--
-- Structure for view `itemsmentions_v`
--
-- DROP TABLE IF EXISTS `itemsmentions_v`;

-- CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `itemsmentions_v` AS select `im`.`ID` AS `ID`,`u`.`ID` AS `UserId`,`im`.`itemID` AS `itemID`,`im`.`time` AS `time` from (`itemsmentions` `im` join `users` `u`) where (`im`.`userID` = `u`.`userID`);

-- --------------------------------------------------------

--
-- Structure for view `users_v`
--
-- DROP TABLE IF EXISTS `users_v`;

-- CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `users_v` AS select `users`.`ID` AS `UserId` from `users`;
