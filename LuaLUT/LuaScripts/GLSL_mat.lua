if mat ~= nil then return end

--dofile("newVector.lua")

local matMeta  = {} --the metatable of matrixes.
local apiTable = {} --the "mat" table.
local apiMeta  = {} --the metatable of the "mat" table. only contains __call, used for mat() constructor syntax.

-------------------------------- api --------------------------------

apiTable.class = matMeta
apiTable.isMat = function(obj)
	return getmetatable(obj) == matMeta
end
apiTable.assertIsMat = function(obj)
	if apiTable.isMat(obj) then
		return obj
	else
		error("Not a matrix: " .. tostring(obj))
	end
end

apiTable.identity = function(dimensions)
	local result = {}
	for columnIndex = 1, dimensions do
		local column = {}
		for rowIndex = 1, dimensions do
			column[rowIndex] = rowIndex == columnIndex and 1 or 0
		end
		result[columnIndex] = setmetatable(column, vec.class)
	end
	return setmetatable(result, matMeta)
end

apiTable.transpose = function(original)
	apiTable.assertIsMat(original)
	local columns = #original
	local rows = #rawget(original, 1)
	local result = setmetatable({}, matMeta)
	for i = 1, #rows do
		result[i] = setmetatable({}, vec.class)
	end
	for column = 1, columns do
		for row = 1, rows do
			result[row][column] = original[column][row]
		end
	end
	return result
end

local function detImpl(matrix, slots)
	if #slots == 1 then
		return matrix[slots[1]][#matrix]
	elseif #slots == 2 then
		local left = matrix[slots[1]]
		local right = matrix[slots[2]]
		local bottom = #matrix
		local top = bottom - 1
		return (
			left [top] * right[bottom] -
			right[top] * left [bottom]
		)
	else
		local startRow = #matrix - #slots + 1
		local sum = 0
		for slotIndex = 1, #slots do
			local slot = table.remove(slots, slotIndex)
			local nextSum = detImpl(matrix, slots) * matrix[slot][startRow]
			sum = (slotIndex & 1) ~= 0 and sum + nextSum or sum - nextSum
			table.insert(slots, slotIndex, slot)
		end
		return sum
	end
end

apiTable.det = function(matrix)
	if #apiTable.assertIsMat(matrix) ~= #rawget(matrix, 1) then
		error("Cannot take the determinant of a non-square matrix.")
	end
	local slots = {}
	for i = 1, #matrix do slots[i] = i end
	return detImpl(matrix, slots)
end

-------------------------------- operations --------------------------------

apiTable.unaryOp = function(a, func)
	local result = {}
	if apiTable.isMat(a) then
		for i = 1, #a do
			result[i] = func(rawget(a, i))
		end
	else
		error("Someone put the matrix metatable somewhere it doesn't belong...")
	end
	return setmetatable(result, matMeta)
end

local function makeUnaryOp(func)
	return function(a)
		return apiTable.unaryOp(a, func)
	end
end

apiTable.binaryOp = function(a, b, func)
	local result = {}
	if apiTable.isMat(a) then
		if apiTable.isMat(b) then
			if #a == #b then
				for i = 1, #a do
					result[i] = func(rawget(a, i), rawget(b, i))
				end
			else
				error("Matrixes have different numbers of column vectors: " .. #a .. " and " .. #b)
			end
		else
			for i = 1, #a do
				result[i] = func(rawget(a, i), b)
			end
		end
	else
		if apiTable.isMat(b) then
			for i = 1, #b do
				result[i] = func(a, rawget(b, i))
			end
		else
			error("Someone put the matrix metatable somewhere it doesn't belong...")
		end
	end
	return setmetatable(result, matMeta)
end

local function checkMatrix(matrix, name)
	if not apiTable.isMat(matrix) then
		error("Matrix must be on the left side of " .. name)
	end
end

local function checkedBinaryOp(a, b, func, name)
	checkMatrix(a, name)
	local result = {}
	if apiTable.isMat(b) then
		if #a == #b then
			for i = 1, #a do
				result[i] = func(rawget(a, i), rawget(b, i))
			end
		else
			error("Matrixes have different numbers of column vectors: " .. #a .. " and " .. #b)
		end
	else
		for i = 1, #a do
			result[i] = func(rawget(a, i), b)
		end
	end
	return setmetatable(result, matMeta)
end

local function makeBinaryOp(func, name)
	if name ~= nil then
		return function(a, b)
			return checkedBinaryOp(a, b, func, name)
		end
	else
		return function(a, b)
			return apiTable.binaryOp(a, b, func)
		end
	end
end

apiTable.tertiaryOp = function(a, b, c, func)
	local result = {}
	if apiTable.isMat(a) then
		if apiTable.isMat(b) then
			if apiTable.isMat(c) then
				if #a == #b and #b == #c then
					for i = 1, #a do result[i] = func(rawget(a, i), rawget(b, i), rawget(c, i)) end
				else
					error("Matrixes a, b, and c have different numbers of column vectors: " .. #a .. ", " .. #b .. ", and " .. #c)
				end
			else
				if #a == #b then
					for i = 1, #a do result[i] = func(rawget(a, i), rawget(b, i), c) end
				else
					error("Matrixes a and b have different numbers of column vectors: " .. #a .. " and " .. #b)
				end
			end
		else
			if apiTable.isMat(c) then
				if #a == #c then
					for i = 1, #a do result[i] = func(rawget(a, i), b, rawget(c, i)) end
				else
					error("Matrixes a and c have different numbers of column vectors: " .. #a .. " and " .. #c)
				end
			else
				for i = 1, #a do result[i] = func(rawget(a, i), b, c) end
			end
		end
	else
		if apiTable.isMat(b) then
			if apiTable.isMat(c) then
				if #b == #c then
					for i = 1, #b do result[i] = func(a, rawget(b, i), rawget(c, i)) end
				else
					error("Matrixes b and c have different numbers of column vectors: " .. #b .. " and " .. #c)
				end
			else
				for i = 1, #b do result[i] = func(a, rawget(b, i), c) end
			end
		else
			if apiTable.isMat(c) then
				for i = 1, #c do result[i] = func(a, b, rawget(c, i)) end
			else
				error("Someone put the matrix metatable somewhere it doesn't belong...")
			end
		end
	end
	return setmetatable(result, matMeta)
end

local function makeTertiaryOp(func)
	return function(a, b, c)
		return apiTable.tertiaryOp(a, b, c, func)
	end
end

matMeta.__add = makeBinaryOp(ops.add, "+")
matMeta.__sub = makeBinaryOp(ops.sub, "-")
local function transformVec(matrix, vector)
	if #matrix == #vector then
		local result = rawget(matrix, 1) * rawget(vector, 1)
		for i = 2, #matrix do result = result + rawget(matrix, i) * rawget(vector, i) end
		return result
	else
		error("Matrix column vector count must equal vector dimensions.")
	end
end
local function composeMat(a, b)
	if #a == #b then
		local result = {}
		for i = 1, #a do result[i] = a * rawget(b, i) end
		return setmetatable(result, matMeta)
	else
		error("Matrix column vector counts must equal each other.")
	end
end
matMeta.__mul = function(l, r)
	checkMatrix(l, "*")
	if apiTable.isMat(r) then
		return composeMat(l, r)
	elseif vec.isVec(r) then
		return transformVec(l, r)
	elseif type(r) == "number" then
		local result = {}
		for i = 1, #l do result[i] = rawget(l, i) * r end
		return setmetatable(result, matMeta)
	else
		error("Value on right hand side of * must be a mat, vec, or number.")
	end
end
matMeta.__call = matMeta.__mul
matMeta.__div = makeBinaryOp(ops.div, "/")
matMeta.__mod = makeBinaryOp(ops.mod, "%")
matMeta.__pow = function(matrix, exponent)
	if apiTable.isMat(matrix) then
		exponent = math.tointeger(exponent)
		if exponent == nil then
			error("b must be an integer.")
		elseif exponent < 0 then
			error("b must be >= 0.")
		elseif exponent == 0 then
			return apiTable.identity(#a)
		else
			local bit = exponent
			bit = bit | (bit >>  1)
			bit = bit | (bit >>  2)
			bit = bit | (bit >>  4)
			bit = bit | (bit >>  8)
			bit = bit | (bit >> 16)
			bit = bit | (bit >> 32)
			bit = (bit >> 1) & ~(bit >> 2)

			local result = matrix
			while bit ~= 0 do
				result = composeMat(result, result)
				if (exponent & bit) ~= 0 then
					result = composeMat(matrix, result)
				end
				bit = bit >> 1
			end
			return result
		end
	else
		error("Matrix must be on the left side of ^")
	end
end
matMeta.__unm = makeUnaryOp(ops.unm)
matMeta.__idiv = makeBinaryOp(ops.idiv, "//")
matMeta.__band = makeBinaryOp(ops.band, "&")
matMeta.__bor  = makeBinaryOp(ops.bor, "|")
matMeta.__bxor = makeBinaryOp(ops.bxor, "~")
matMeta.__shl  = makeBinaryOp(ops.shl, "<<")
matMeta.__shr  = makeBinaryOp(ops.shr, ">>")
matMeta.__bnot = makeUnaryOp (ops.bnot, "~")
matMeta.__eq = function(a, b)
	if apiTable.isMat(a) and apiTable.isMat(b) then
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
matMeta.__tostring = function(this)
	local result = "mat(\n\t" .. tostring(rawget(this, 1))
	for i = 2, #this do
		result = result .. ",\n\t" .. tostring(rawget(this, i))
	end
	return result .. "\n)"
end

-------------------------------- constructor --------------------------------

apiMeta.__call = function(self, ...)
	vec.assertEqualDimensions(...)
	return setmetatable({ ... }, matMeta)
end

-------------------------------- make public --------------------------------

_G.mat = setmetatable(apiTable, apiMeta)
