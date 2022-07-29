function processPixel(x, y)
	local f = vec(x, y) / vec(width, height)
	return vec(f.x, f.y, f.x * f.y)
end
