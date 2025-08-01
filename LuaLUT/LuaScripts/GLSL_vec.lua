if vec ~= nil then return end

-------------------------------- swizzles --------------------------------

local swizzles = {
	[("x"):byte()] = 1,
	[("y"):byte()] = 2,
	[("z"):byte()] = 3,
	[("w"):byte()] = 4,

	[("r"):byte()] = 1,
	[("g"):byte()] = 2,
	[("b"):byte()] = 3,
	[("a"):byte()] = 4
}

local function getSwizzle(components, index)
	local result = swizzles[components:byte(index)]
	if result ~= nil then
		return result
	else
		error("Invalid swizzle: " .. components)
	end
end

local function getComponent(v, index)
	if index > 0 and index <= #v then
		return rawget(v, index)
	else
		error("Invalid index: " .. index .. " exceedes " .. #v .. "-dimensional vector")
	end
end

local function setComponent(v, index, value)
	if index > 0 and index <= #v then
		rawset(v, index, value)
	else
		error("Invalid index: " .. index .. " exceedes " .. #v .. "-dimensional vector")
	end
end

-------------------------------- declarations --------------------------------

local vecMeta  = {} --the metatable of vectors.
local apiTable = {} --the "vec" table.
local apiMeta  = {} --the metatable of the "vec" table. only contains __call, used for vec() constructor syntax.

-------------------------------- generic operations --------------------------------

apiTable.class = vecMeta
apiTable.isVec = function(obj)
	return getmetatable(obj) == vecMeta
end

apiTable.assertIsVec = function(obj)
	if apiTable.isVec(obj) then
		return obj
	else
		error("Not a vector: " .. tostring(obj))
	end
end

apiTable.hash = function(this, componentHasher)
	if componentHasher == nil then componentHasher = ops.identity end
	local hash = rawget(apiTable.assertIsVec(this), 1)
	for i = 2, #this do
		hash = hash * 31 + componentHasher(rawget(this, i))
	end
	return hash
end

apiTable.assertEqualDimensions = function(...)
	local vectorCount = select("#", ...)
	if vectorCount == 0 then
		error("Must provide at least one vector.")
	end
	local dimensions = #apiTable.assertIsVec(select(1, ...))
	for i = 2, vectorCount do
		if dimensions ~= #apiTable.assertIsVec(select(i, ...)) then
			error("Provided vectors do not have the same number of dimensions.")
		end
	end
	return dimensions
end

apiTable.unaryOp = function(a, func)
	local result = {}
	if apiTable.isVec(a) then
		for i = 1, #a do
			result[i] = func(rawget(a, i))
		end
	else
		error("Someone put the vector metatable somewhere it doesn't belong...")
	end
	return setmetatable(result, vecMeta)
end

local function makeUnaryOp(func)
	return function(a)
		return apiTable.unaryOp(a, func)
	end
end

apiTable.binaryOp = function(a, b, func)
	local result = {}
	if apiTable.isVec(a) then
		if apiTable.isVec(b) then
			if #a == #b then
				for i = 1, #a do
					result[i] = func(rawget(a, i), rawget(b, i))
				end
			else
				error("Vectors have different numbers of dimensions: " .. #a .. " and " .. #b)
			end
		else
			for i = 1, #a do
				result[i] = func(rawget(a, i), b)
			end
		end
	else
		if apiTable.isVec(b) then
			for i = 1, #b do
				result[i] = func(a, rawget(b, i))
			end
		else
			error("Someone put the vector metatable somewhere it doesn't belong...")
		end
	end
	return setmetatable(result, vecMeta)
end

local function makeBinaryOp(func)
	return function(a, b)
		return apiTable.binaryOp(a, b, func)
	end
end

apiTable.tertiaryOp = function(a, b, c, func)
	local result = {}
	if apiTable.isVec(a) then
		if apiTable.isVec(b) then
			if apiTable.isVec(c) then
				if #a == #b and #b == #c then
					for i = 1, #a do result[i] = func(rawget(a, i), rawget(b, i), rawget(c, i)) end
				else
					error("Vectors a, b, and c have different numbers of dimensions: " .. #a .. ", " .. #b .. ", and " .. #c)
				end
			else
				if #a == #b then
					for i = 1, #a do result[i] = func(rawget(a, i), rawget(b, i), c) end
				else
					error("Vectors a and b have different numbers of dimensions: " .. #a .. " and " .. #b)
				end
			end
		else
			if apiTable.isVec(c) then
				if #a == #c then
					for i = 1, #a do result[i] = func(rawget(a, i), b, rawget(c, i)) end
				else
					error("Vectors a and c have different numbers of dimensions: " .. #a .. " and " .. #c)
				end
			else
				for i = 1, #a do result[i] = func(rawget(a, i), b, c) end
			end
		end
	else
		if apiTable.isVec(b) then
			if apiTable.isVec(c) then
				if #b == #c then
					for i = 1, #b do result[i] = func(a, rawget(b, i), rawget(c, i)) end
				else
					error("Vectors b and c have different numbers of dimensions: " .. #b .. " and " .. #c)
				end
			else
				for i = 1, #b do result[i] = func(a, rawget(b, i), c) end
			end
		else
			if apiTable.isVec(c) then
				for i = 1, #c do result[i] = func(a, b, rawget(c, i)) end
			else
				error("Someone put the vector metatable somewhere it doesn't belong...")
			end
		end
	end
	return setmetatable(result, vecMeta)
end

local function makeTertiaryOp(func)
	return function(a, b, c)
		return apiTable.tertiaryOp(a, b, c, func)
	end
end

-------------------------------- vecMeta operations --------------------------------

vecMeta.__index = function(this, components)
	if type(components) == "number" then
		return getComponent(this, components)
	elseif type(components) == "string" then
		if #components == 1 then
			return getComponent(this, getSwizzle(components, 1))
		else
			local result = {}
			for i = 1, #components do
				result[i] = getComponent(this, getSwizzle(components, i))
			end
			return setmetatable(result, vecMeta)
		end
	else
		error("components must be string or number, got " .. tostring(components))
	end
end

vecMeta.__newindex = function(this, components, value)
	if type(components) == "number" then
		setComponent(this, components, value)
	elseif type(components) == "string" then
		if #components == 1 then
			setComponent(this, getSwizzle(components, 1), value)
		else
			for i = 1, #components do
				setComponent(this, getSwizzle(components, i), getComponent(value, i))
			end
		end
	else
		error("components must be string or number, got " .. tostring(components))
	end
end

--arithmetic
vecMeta.__add  = makeBinaryOp(ops.add)
vecMeta.__sub  = makeBinaryOp(ops.sub)
vecMeta.__mul  = function(a, b)
	if mat and mat.isMat(b) then
		if #a ~= #rawget(b, 1) then
			error("Matrix column vector count must equal vector dimensions.")
		end
		local result = {}
		for i = 1, #a do
			result[i] = apiTable.dot(a, b[i])
		end
		return setmetatable(result, vecMeta)
	else
		return apiTable.binaryOp(a, b, ops.mul)
	end
end
vecMeta.__div  = makeBinaryOp(ops.div)
vecMeta.__mod  = makeBinaryOp(ops.mod)
vecMeta.__pow  = makeBinaryOp(ops.pow)
vecMeta.__idiv = makeBinaryOp(ops.idiv)
vecMeta.__unm  = makeUnaryOp (ops.unm)
--bitwise
vecMeta.__band = makeBinaryOp(ops.band)
vecMeta.__bor  = makeBinaryOp(ops.bor)
vecMeta.__bxor = makeBinaryOp(ops.bxor)
vecMeta.__shl  = makeBinaryOp(ops.shl)
vecMeta.__shr  = makeBinaryOp(ops.shr)
vecMeta.__bnot = makeUnaryOp (ops.bnot)
--comparisons
vecMeta.__eq = function(a, b)
	if apiTable.isVec(a) and apiTable.isVec(b) then
		if #a == #b then
			for i = 1, #a do
				if rawget(a, i) ~= rawget(b, i) then
					return false
				end
			end
			return true
		else
			return false
		end
	else
		return false
	end
end
--other
vecMeta.__tostring = function(this)
	local result = "vec(" .. tostring(rawget(this, 1))
	for i = 2, #this do
		result = result .. ", " .. tostring(rawget(this, i))
	end
	return result .. ")"
end

-------------------------------- math operations --------------------------------

math.clamp = function(value, min, max)
	if value <= min then return min end
	if value >= max then return max end
	return value
end
math.mix = function(low, high, frac)
	return (high - low) * frac + low
end
math.smoothstep = function(low, high, frac)
	local t = math.unmix(low, high, frac)
	if t >= 1.0 then return 1.0 end
	if t <= 0.0 then return 0.0 end
	return t * t * (t * -2.0 + 3.0)
end
math.unmix = function(low, high, frac)
	return (frac - low) / (high - low)
end

apiTable.abs  = makeUnaryOp(math.abs )
apiTable.acos = makeUnaryOp(math.acos)
apiTable.all = function(v)
	for i = 1, #v do
		if not rawget(v, i) then return false end
	end
	return true
end
apiTable.any = function(v)
	for i = 1, #v do
		if rawget(v, i) then return true end
	end
	return false
end
apiTable.asin = makeUnaryOp(math.asin)
apiTable.atan = makeUnaryOp(math.atan)
apiTable.ceil = makeUnaryOp(math.ceil)
apiTable.clamp = makeTertiaryOp(math.clamp)
apiTable.cos  = makeUnaryOp(math.cos )
apiTable.cosh = makeUnaryOp(math.cosh)

local function c(vals)
	if #vals == 0 then return "" end

	local result = tostring(vals[1])
	for i = 2, #vals do
		result = result .. ", " .. tostring(vals[i])
	end
	return result
end

local function detImpl(rows, slots)
	--print("Computing det on rows " .. c(rows) .. " and slots " .. c(slots))
	local result
	if #slots == 1 then
		result = rows[#rows][slots[1]]
	elseif #slots == 2 then
		local top = rows[#rows - 1]
		local bottom = rows[#rows]
		local left = slots[1]
		local right = slots[2]
		result = (
			top[left ] * bottom[right] -
			top[right] * bottom[left ]
		)
	else
		local startRow = #rows - #slots + 1
		local sum = 0
		for slotIndex = 1, #slots do
			local slot = table.remove(slots, slotIndex)
			local det = detImpl(rows, slots) * rows[startRow][slot]
			sum = (slotIndex & 1) ~= 0 and sum + det or sum - det
			table.insert(slots, slotIndex, slot)
		end
		result = sum
	end
	--print("--> " .. tostring(result))
	return result
end

apiTable.cross = function(...)
	local numberOfVectors = select("#", ...)
	local numberOfDimensions = apiTable.assertEqualDimensions(...)
	if numberOfVectors ~= numberOfDimensions - 1 then
		error("Number of vectors must be 1 less than the number of dimensions.")
	end
	local vectors = { ... }
	local result = {}
	local slots = {}
	for i = 1, numberOfDimensions do slots[i] = i end
	for slotIndex = 1, numberOfDimensions do
		local slot = table.remove(slots, slotIndex)
		local det = detImpl(vectors, slots)
		result[slotIndex] = (slotIndex & 1) ~= 0 and det or -det
		table.insert(slots, slotIndex, slot)
	end
	return setmetatable(result, vec.class)
end
apiTable.distance = function(a, b)
	local dimensions = apiTable.assertEqualDimensions(a, b)
	local result = (rawget(a, 1) - rawget(b, 1)) ^ 2
	for i = 2, dimensions do
		result = result + (rawget(a, i) - rawget(b, i)) ^ 2
	end
	return result
end
apiTable.dot = function(a, b)
	if apiTable.isVec(a) and apiTable.isVec(b) then
		local dimensions = apiTable.assertEqualDimensions(a, b)
		local result = rawget(a, 1) * rawget(b, 1)
		for i = 2, dimensions do
			result = result + rawget(a, i) * rawget(b, i)
		end
		return result
	elseif apiTable.isVec(a) and type(b) == "number" then
		local result = rawget(a, 1)
		for i = 2, #a do
			result = result + rawget(a, i)
		end
		return result * b
	elseif type(a) == "number" and apiTable.isVec(b) then
		local result = rawget(b, 1)
		for i = 2, #b do
			result = result + rawget(b, i)
		end
		return a * result
	else
		error("Must call dot with (vec, vec), (vec, number), or (number, vec)")
	end
end
apiTable.equal = makeBinaryOp(ops.eq, "==")
apiTable.exp = function(a, base)
	if base ~= nil then
		return apiTable.binaryOp(a, base, math.exp)
	else
		return apiTable.unaryOp(a, math.exp)
	end
end
apiTable.floor = makeUnaryOp (math.floor)
apiTable.fmod  = makeBinaryOp(math.fmod)
apiTable.fract = makeUnaryOp (function(a1) return a1 % 1.0 end)
apiTable.greaterThan      = makeBinaryOp(ops.gt)
apiTable.greaterThanEqual = makeBinaryOp(ops.ge)
apiTable.length = function(a)
	local result = rawget(a, 1) ^ 2
	for i = 2, #a do
		result = result + rawget(a, i) ^ 2
	end
	return math.sqrt(result)
end
apiTable.lessThan      = makeBinaryOp(ops.lt)
apiTable.lessThanEqual = makeBinaryOp(ops.le)
apiTable.log = function(a, base)
	if base ~= nil then
		return apiTable.binaryOp(a, base, math.exp)
	else
		return apiTable.unaryOp(a, math.exp)
	end
end
apiTable.max = makeBinaryOp  (math.max)
apiTable.min = makeBinaryOp  (math.min)
apiTable.mix = makeTertiaryOp(math.mix)
--apiTable.mod = makeUnaryOp   (math.mod)
apiTable.modf = function(a)
	local intPart = {}
	local fracPart = {}
	for i = 1, #a do
		intPart[i], fracPart[i] = math.modf(rawget(a, i))
	end
	return setmetatable(intPart, vecMeta), setmetatable(fracPart, vecMeta)
end
apiTable.normalize = function(a) return a * (1.0 / apiTable.length(a)) end
apiTable.notEqual = makeBinaryOp(function(a1, b1) return a1 ~= b1 end)
apiTable.reflect = function(v, normal)
	local dot2 = apiTable.dot(v, normal) * 2.0
	return apiTable.binaryOp(v, normal, function(v1, n1) return v1 - dot2 * n1 end)
end
apiTable.refract = function(v, normal, eta)
	local dot = apiTable.dot(v, normal)
	local k = 1.0 - eta ^ 2 * (1.0 - dot ^ 2)
	if k >= 0.0 then
		local normalMultiplier = eta * dot + sqrt(k)
		return apiTable.binaryOp(v, normal, function(v1, n1) return eta * v1 - normalMultiplier * n1 end)
	else
		local result = {}
		for i = 1, #v do
			result[i] = 0.0
		end
		return setmetatable(result, vecMeta)
	end
end
apiTable.sign       = makeUnaryOp   (math.sign)
apiTable.sin        = makeUnaryOp   (math.sin )
apiTable.sinh       = makeUnaryOp   (math.sinh)
apiTable.smoothstep = makeTertiaryOp(math.smoothstep)
apiTable.sqrt       = makeUnaryOp   (math.sqrt )
apiTable.tan        = makeUnaryOp   (math.tan  )
apiTable.tanh       = makeUnaryOp   (math.tanh )
apiTable.unmix      = makeTertiaryOp(math.unmix)

-------------------------------- vec constructor --------------------------------

apiMeta.__call = function(self, ...)
	local result = {}
	for i = 1, select("#", ...) do
		local arg = select(i, ...)
		if apiTable.isVec(arg) then
			for j = 1, #arg do
				result[#result + 1] = rawget(arg, j)
			end
		else
			result[#result + 1] = arg
		end
	end
	if #result <= 1 then
		error("Vectors must have at least 2 dimensions")
	else
		return setmetatable(result, vecMeta)
	end
end

-------------------------------- make public --------------------------------

_G.vec = setmetatable(apiTable, apiMeta)
