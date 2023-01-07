function texelFetch(samplerName, texcoord)
	local pixelSize, pixelR, pixelG, pixelB, pixelA = samplers:TexelFetch(samplerName, texcoord)
	if pixelSize == 1 then return vec(pixelR) end
	if pixelSize == 2 then return vec(pixelR, pixelG) end
	if pixelSize == 3 then return vec(pixelR, pixelG, pixelB) end
	if pixelSize == 4 then return vec(pixelR, pixelG, pixelB, pixelA) end
	-- TODO: error!
end

function texture(samplerName, texcoord)
	local pixelSize, pixelR, pixelG, pixelB, pixelA = samplers:Texture(samplerName, texcoord)
	if pixelSize == 1 then return vec(pixelR) end
	if pixelSize == 2 then return vec(pixelR, pixelG) end
	if pixelSize == 3 then return vec(pixelR, pixelG, pixelB) end
	if pixelSize == 4 then return vec(pixelR, pixelG, pixelB, pixelA) end
	-- TODO: error!
end

function texture2D(samplerName, texcoord)
	return texture(samplerName, texcoord)
end
