select *
from 
(
select s.id, s.stock_name, avg(d.volume * d.close) as "avgvol"
from at_spolki2 s, at_dzienne0 d
where s.stock_type=0 and s.enabled=true
and s.id=d.fk_id_spolki
and d.ts > '2021-09-01'
group by s.id, s.stock_name
) as t
where t.avgvol >= 500000
order by t.avgvol desc