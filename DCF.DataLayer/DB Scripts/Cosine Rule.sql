UPDATE UserScores us SET us.Belief = (1-0.2)*us.Belief + 0.2*
(2*(SELECT SUM(sfu1.Score) FROM ScoredFacts sfu1, itemsmentions_v im1 WHERE im1.UserId = us.UserId 
    AND sfu1.ItemId=im1.ItemId AND sfu1.Category='Demo') -
(SELECT SUM(sf2.Score) FROM ScoredFacts sf2 WHERE sf2.Factor>0 AND sf2.TopicId IN 
    (SELECT im2.TopicId FROM ItemsMentions_v im2 where im2.Userid=us.UserId) AND sf2.Category='Demo'))/
(SELECT SQRT(COUNT(*)*SUM(sf1.Score*sf1.Score)) FROM ScoredFacts sf1 WHERE sf1.Factor>0 AND sf1.TopicId in 
    (SELECT im3.TopicId FROM ItemsMentions_v im3 where im3.UserId=us.UserId) AND sf1.Category='Demo');
    

UPDATE ScoredFacts sf SET sf.Score = 2*
IFNULL((SELECT SUM(POW(us1.Belief, 3)) FROM UserScores us1, ItemsMentions_v sfu1 
WHERE us1.UserId=sfu1.UserId AND sfu1.ItemId=sf.ItemId), 0)/ 
(SELECT SUM(POW(us2.Belief, 3)) FROM UserScores us2 WHERE us2.UserId in 
    (SELECT im2.Userid FROM ItemsMentions_v im2 WHERE im2.Topicid=sf.TopicId)) - 1 
WHERE Category='Demo'


