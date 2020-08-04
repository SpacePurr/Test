select e.name from
(select 
	id
	from employees e
		join salaries s on(e.id = s.worker_id)
	where month(date) = 4
) result
left join employees e on e.id != result.id