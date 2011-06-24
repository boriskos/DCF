    delimiter $$
drop procedure if exists GenSyntheticDataset $$

CREATE PROCEDURE GenSyntheticDataset (IN nFacts INT, IN prob DOUBLE)
BEGIN
    DECLARE ntopics INT;
    DECLARE nusers INT;
    DECLARE nitems INT;
    DECLARE i INT DEFAULT 0;

    set ntopics = 115;
    set nitems = 10;
    set nusers = nfacts / 100 / 1 + 1;

    # recreate all the tables and views
    DROP VIEW IF EXISTS ItemsMentions_v;
    DROP TABLE IF EXISTS ItemsMentions;
    DROP VIEW IF EXISTS Items_v;
    DROP TABLE IF EXISTS Items;
    DROP TABLE IF EXISTS Topics;
    DROP VIEW IF EXISTS Users_v;
    DROP TABLE IF EXISTS Users;
    DROP TABLE IF EXISTS CorrectFacts;
    
    CREATE TABLE `users` (
      `ID` int(11) unsigned NOT NULL,
      `userID` varchar(50) COLLATE utf8_bin NOT NULL COMMENT 'actually user Email ',
      `Points` int(10) unsigned NOT NULL DEFAULT '0',
      `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
      `dividendPoints` int(10) NOT NULL DEFAULT '0',
      `pointsSinceLastLogin` int(11) NOT NULL DEFAULT '0',
      `userName` varchar(100) COLLATE utf8_bin NOT NULL,
      `password` varchar(50) COLLATE utf8_bin DEFAULT NULL,
      PRIMARY KEY (`ID`),
      KEY `Points_4` (`Points`,`dividendPoints`)
    ) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

    CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `users_v` AS 
    select `ID` AS `UserId` from `users`;

    CREATE TABLE `topics` (
      `TopicId` int(10) unsigned NOT NULL ,
      `TopicName` varchar(200) COLLATE utf8_bin NOT NULL,
      `TopicText` varchar(200) COLLATE utf8_bin NOT NULL DEFAULT '-',
      `TopicType` smallint(1) unsigned zerofill NOT NULL,
      `Category` varchar(70) COLLATE utf8_bin NOT NULL DEFAULT '-',
      `Rank` double NOT NULL DEFAULT '0',
      `Value` varchar(100) COLLATE utf8_bin DEFAULT NULL,
      PRIMARY KEY (`TopicId`),
      UNIQUE KEY `id` (`TopicId`,`TopicName`),
      KEY `name` (`TopicName`)
    ) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

    CREATE TABLE `items` (
      `id` int(10) unsigned NOT NULL ,
      `name` varchar(500) COLLATE utf8_bin NOT NULL,
      `topicID` int(11) NOT NULL,
      PRIMARY KEY (`id`),
      KEY `topicID` (`topicID`),
      KEY `name` (`name`(255))
    ) ENGINE=InnoDB AUTO_INCREMENT=1047 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

    CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `items_v` AS 
    select `items`.`id` AS `ItemID`,`items`.`name` AS `ItemName`,`items`.`topicID` AS `topicID` from `items`;

    CREATE TABLE `itemsmentions` (
      `ID` int(11) unsigned NOT NULL AUTO_INCREMENT,
      `userID` varchar(100) COLLATE utf8_bin NOT NULL,
      `itemID` int(10) unsigned NOT NULL,
      `topicID` int(10) unsigned NOT NULL,
      `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
      PRIMARY KEY (`ID`),
      KEY `itemID` (`itemID`),
      KEY `topicID` (`topicID`)
    ) ENGINE=InnoDB AUTO_INCREMENT=3491 DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

    CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `itemsmentions_v` AS 
    select `im`.`ID` AS `ID`,`u`.`ID` AS `UserId`,`im`.`itemID` AS `itemID`,`im`.`topicID` AS `TopicId`,`im`.`time` AS `time` 
    from (`itemsmentions` `im` join `users` `u`) where (`im`.`userID` = `u`.`userID`);

    CREATE TABLE `correctfacts` (
      `ItemID` int(10) unsigned NOT NULL DEFAULT '0',
      `ItemName` varchar(500) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
      `topicID` int(11) NOT NULL
    ) ENGINE=MyISAM DEFAULT CHARSET=latin1;


    # generation assumption N topics is 100 and N answers per topic 10
    WHILE i<ntopics DO
        #create topic
        INSERT INTO topics (TopicId, TopicName, TopicType) VALUES (i, CONCAT("t", cast(i as char)), 0);
        SET i = i + 1;
    END WHILE;
    set i=0;
    while i<nitems*ntopics do
        INSERT INTO Items (id, `name`,`topicID`) 
            VALUES ( i, CONCAT("t", cast((i DIV nitems) as char), "i", cast(i % nitems as char)) , i DIV nitems);
        set i = i + 1;
    end while;
    set i=0;
    while i<nusers do
        INSERT INTO `users` (`ID`, `userID`, `userName`) 
            VALUES (i, concat("u",cast(i as CHAR), "@tau.ac.il"), concat("u",cast(i as CHAR)) );
        set i = i + 1;
    end while;
    

    set i=0;
    while i<nusers do
        #generate user activity
        insert into itemsmentions (userId, itemID, topicId)
        select UserId, if(l.p < 0.3, l.topicid*10, l.topicid*10+floor(9*rand()+1)) as itemid, l.topicid 
        from (select i as UserId, t.topicid, rand() as p from 
            (select t.topicid from topics t order by rand() limit 100) t 
        ) l;
        
        set i = i + 1;
    end while;
END
$$
delimiter ;