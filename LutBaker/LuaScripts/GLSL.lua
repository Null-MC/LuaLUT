function dot(a, b)
    if (is_vec3(lhs) and is_vec3(rhs))
    then
	    return vec3.dot(lhs, rhs)
    end

    -- else error?
end

function mod(a, b)
	return a % b
end

function rcp(val)
	return 1 / val
end

function saturate(val)
	return math.max(math.min(val, 1), 0)
end

function sqrt(val)
	return math.sqrt(val)
end
