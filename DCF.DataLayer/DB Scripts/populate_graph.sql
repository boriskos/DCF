DROP procedure IF EXISTS `populate_graph`;


-- --------------------------------------------------------------------------------
-- Routine DDL
-- Note: comments before and after the routine body will not be stored by the server
-- --------------------------------------------------------------------------------
DELIMITER $$

CREATE DEFINER=`root`@`localhost` PROCEDURE `populate_graph`()
BEGIN
    DECLARE i INT;
    declare cnt int;

    select count(*)  into cnt from users;
    
    CREATE TABLE IF NOT EXISTS UserGraph (
          ID INT(11) unsigned NOT NULL AUTO_INCREMENT,
          SourceUserID INT(11) unsigned NOT NULL,
          TargetUserID INT(11) unsigned NOT NULL,
          Weight DOUBLE NOT NULL DEFAULT 0,
          
          primary key (ID),
          INDEX `source_idx` USING BTREE (`SourceUserID` ASC) 
          ) ENGINE=MyISAM;

    truncate table usergraph;

    set i=0;
lbl: loop
#   create records by chunks
        INSERT INTO UserGraph (SourceUserID, TargetUserID, Weight)
        select a.SourceUserID, a.TargetUserId, count(*) from (
            SELECT im1.UserId as SourceUserID, im2.UserId as TargetUserId
            FROM ItemsMentions im1, ItemsMentions im2 
            WHERE im1.UserId=i and im1.ItemId = im2.ItemId and im1.UserId <> im2.UserId ) a 
        group by a.TargetUserId;         
        
        update UserGraph ug, 
            (select sum(Weight) TotalWeight from UserGraph where SourceUSerID=i) t
        set ug.Weight = ug.Weight/t.TotalWeight
        where ug.SourceUserID = i;
        
        set i = i + 1;
        if (i>=cnt) then
            leave lbl;
        end if;
    end loop;
END

$$
delimiter ;