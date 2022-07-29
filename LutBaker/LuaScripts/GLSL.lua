function abs(val)
	return math.abs(val)
end

function cos(ang)
	return math.cos(ang)
end

function cross(a, b)
	return vec.cross(a, b)
end

function dot(a, b)
    return vec.dot(a, b)
end

function floor(val)
	return math.floor(val)
end

function max(a, b)
	return math.max(a, b)
end

function min(a, b)
	return math.min(a, b)
end

function mod(a, b)
	return a % b
end

function normalize(val)
	return vec.normalize(val)
end

function pow(val, power)
	return val ^ power
end

function rcp(val)
	return 1 / val
end

function saturate(val)
	return math.max(math.min(val, 1), 0)
end

function sin(ang)
	return math.sin(ang)
end

function sqrt(val)
	return math.sqrt(val)
end
