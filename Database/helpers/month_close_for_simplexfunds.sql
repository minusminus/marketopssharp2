select *
from crosstab(
'select d.ts, s.stock_name, d.close
from at_mies4 d, at_spolki2 s
where d.fk_id_spolki = s.id
and s.stock_name in (''PKO008'', ''PKO014'', ''PKO015'', ''PKO020'', ''PKO021'', ''PKO026'', ''PKO027'', ''PKO028'', ''PKO029'', ''PKO057'', ''PKO072'', ''PKO073'', ''PKO074'', ''PKO097'', ''PKO098'', ''PKO909'', ''PKO910'', ''PKO913'', ''PKO918'', ''PKO919'', ''PKO925'')
and ts = ''2021-06-01''
order by s.stock_name'
)
as final_result(ts timestamp without time zone, PKO008 numeric, PKO014 numeric, PKO015 numeric, PKO020 numeric, PKO021 numeric, PKO026 numeric, PKO027 numeric, PKO028 numeric, PKO029 numeric, PKO057 numeric, PKO072 numeric, PKO073 numeric, PKO074 numeric, PKO097 numeric, PKO098 numeric, PKO909 numeric, PKO910 numeric, PKO913 numeric, PKO918 numeric, PKO919 numeric, PKO925 numeric)
