create or replace view ush_prob_stats as
(select version, min(userscore) MinScore, max(UserScore) MaxScore, sum(UserScore) TotalScore, count(*) UserCount 
from ush_prob group by version) ;

create or replace view ush_prob_norm as
(SELECT up.version, up.userid, if(up.UserScore=t.MinScore, 0, 
(up.userscore-t.MinScore)/(t.TotalScore-t.UserCount*t.MinScore)) UserScore
    FROM ush_prob up, ush_prob_stats t
    where up.version = t.version) ;

#validation that the views are correct
select t.version, sum(t.userscore) Score from ush_prob_norm t group by t.version;

#entropy calculation
select t.version, -sum(t.userscore*log(t.userscore)) entropy from ush_prob_norm t 
where t.userscore > 0 group by t.version;

