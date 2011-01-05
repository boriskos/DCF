SELECT it.itemname, tp.value, count(*) support 
FROM itemsmentions im, items it, topics tp 
where im.itemid = it.itemid and it.topicid=tp.topicid
group by it.itemid
order by support desc;
