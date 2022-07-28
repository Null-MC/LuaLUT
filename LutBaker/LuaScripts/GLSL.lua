function saturate(val)
	return math.max(math.min(val, 1), 0)
end

function rcp(val)
	return 1 / val
end
