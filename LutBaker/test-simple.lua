function processPixel(x, y)
	f = vec(x, y) / vec(width, height)
	color = vec(f.x, f.y, f.x * f.y)
	return color
end
