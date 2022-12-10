function abs(val)
	if vec.isVec(val) then
		return vec.abs(val)
	else
		return math.abs(val)
	end
end

function acos(val)
	return math.acos(val)
end

function clamp(val, min, max)
	if vec.isVec(val) then
		return vec.clamp(val, min, max)
	else
		return math.clamp(val, min, max)
	end
end

function cos(ang)
	return math.cos(ang)
end

function cross(a, b)
	return vec.cross(a, b)
end

function dot(vecA, vecB)
    return vec.dot(vecA, vecB)
end

function exp(val, power)
	if vec.isVec(val) then
		return vec.exp(val, power)
	else
		return math.exp(val, power)
	end
end

function exp2(val, power)
    return math.exp2(val, power)
end

function floor(val)
	if vec.isVec(val) then
		return vec.floor(val)
	else
		return math.floor(val)
	end
end

function fract(val)
	if vec.isVec(val) then
		return vec.fract(val)
	else
		return val % 1
	end
end

function length(vector)
	return vec.length(vector)
end

function lerp(x, y, weight)
	return mix(x, y, weight)
end

function max(valA, valB)
	if vec.isVec(valA) or vec.isVec(valB) then
		return vec.max(valA, valB)
	else
		return math.max(valA, valB)
	end
end

function min(valA, valB)
	return math.min(valA, valB)
end

function mix(x, y, weight)
	if vec.isVec(x) or vec.isVec(y) then
		return vec.mix(x, y, weight)
	else
		return x * (1.0 - weight) + y * weight
	end
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
	return clamp(val, 0.0, 1.0)
end

function sin(ang)
	if vec.isVec(ang) then
		return vec.sin(ang)
	else
		return math.sin(ang)
	end
end

function sqrt(val)
	return math.sqrt(val)
end
