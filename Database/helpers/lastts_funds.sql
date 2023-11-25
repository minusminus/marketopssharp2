select t.maxts, s.*
from at_spolki2 s,
(
select fk_id_spolki, max(ts) as "maxts"
from at_dzienne4
group by fk_id_spolki
) t
where s.stock_type=4 and s.enabled=true
and s.id=t.fk_id_spolki
--and t.maxts < '2021-01-01'
--and stock_fullname like 'PKO%'
--order by t.maxts desc
