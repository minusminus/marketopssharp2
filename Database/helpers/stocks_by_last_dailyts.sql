select d.ts, s.*
from at_spolki2 s, 
(select fk_id_spolki, max(ts) as "ts" from at_dzienne0 group by fk_id_spolki) d
where s.stock_type=0 and s.enabled=true
and d.fk_id_spolki=s.id
and d.ts < '2022-02-01'
order by d.ts
