select count(*)/(select count(*) from topics) from (
select * from (

select i.topicid, i.name, sum(us.belief) as ItemScore
from items i, userscores us, itemsmentions im
where i.id = im.itemid and us.userid=im.userid
group by im.itemid
order by i.topicid, ItemScore desc

) t group by t.topicid
)  b where RIGHT(b.name, 2) = "i0"