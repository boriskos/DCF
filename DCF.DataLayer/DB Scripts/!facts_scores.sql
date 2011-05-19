select a.TopicName, a.ItemName, a.NormalizedScore from (

SELECT t.TopicName, i.ItemName, sf.score/total.TotalScore as NormalizedScore, t.topicid FROM 
(select iv.itemid, sum(us.belief) as score from items_v iv, userscores us, itemsmentions_v imv 
    where iv.itemid=imv.itemid and imv.userid=us.userid group by iv.itemid) sf, items_v i, topics t,
(select imv1.TopicId, sum(us1.belief) TotalScore from userscores us1, itemsmentions_v imv1 
    where imv1.Userid=us1.Userid group by imv1.TopicId) total
where i.itemid=sf.itemid and t.topicid=i.topicid and total.topicid=i.topicid order by t.topicid, sf.score desc 

) a where a.topicid < 30 group by a.topicname