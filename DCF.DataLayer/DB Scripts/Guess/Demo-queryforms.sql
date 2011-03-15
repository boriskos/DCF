-- phpMyAdmin SQL Dump
-- version 3.2.0.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Mar 10, 2011 at 10:26 PM
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

-- --------------------------------------------------------

--
-- Table structure for table `queryforms`
--

DROP TABLE IF EXISTS `queryforms`;
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

--
-- Dumping data for table `queryforms`
--

INSERT INTO `queryforms` (`id`, `FormName`, `FormDescription`, `FormTitle`, `FormQuestionText`, `ParamsTableName`, `ParamsColNames`, `QueryBody`, `TopicType`) VALUES
(1, 'Celebrities', 'Names of Celebrities', 'Celebrity', 'What is {0}?', 'celebrity_topics_view', 'TopicName', 'select i.name as Name,  sf.score as Confidence from items i, topics t, scoredfacts sf where sf.topicid=t.topicid and t.topicname="{0}" and i.id=sf.itemid order by Confidence desc LIMIT 0, 20', 0),
(2, 'Capital Cities', 'Capital Cities Around the World', 'Cleaning Results', 'What is the capital of {0}?', 'Capital_cities_view', 'TopicName', 'select i.name as Name,  sf.score as Confidence from items i, topics t, scoredfacts sf where sf.topicid=t.topicid and t.topicname="{0}" and i.id=sf.itemid order by Confidence desc LIMIT 0, 20', 0),
(3, 'All Users', 'Scores of Trivia Users', 'User Cleaning Scores', 'Click ''Query'' to See Scores of All Trivia Users', 'userscores', 'userid', 'select u.username ''User Name'', us.belief Confidence from userscores us, users u where us.userid=u.id order by us.belief desc', 1),
(4, 'Query Fact Scores', 'Administrator', 'Fact Scores', 'Choose topic to see items in it {0}', 'Topics', 'TopicName', 'select i.name as Name, sf.Score as Confidence from items i, scoredfacts sf, topics t where sf.itemid=i.id and sf.topicid=t.topicid and t.topicname="{0}" order by Score desc', 0),
(5, 'Trivia Questions', 'Query Various Trivia Topics', 'Trivia Questions', 'Names of {0}:', 'multiple_answers_view', 'TopicName', 'select i.name as Name,  sf.score as Confidence from items i, topics t, scoredfacts sf where sf.topicid=t.topicid and t.topicname="{0}" and i.id=sf.itemid order by Confidence desc LIMIT 0, 20', 1);
