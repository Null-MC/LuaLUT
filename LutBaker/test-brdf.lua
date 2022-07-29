sampleCount = 1024

function RadicalInverse_VdC(bits)
    bits = (bits << 16u) | (bits >> 16u);
    bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
    bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
    bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
    bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
    return float(bits) * 2.3283064365386963e-10; // / 0x100000000
	return 1.0
end

function hammersley(i)
	x = i / sampleCount
	y = RadicalInverse_VdC(i)
	return x, y
end

function processPixel(x, y)
	local NoV = x / width;
	local roughness = y / height;

	local v = vec(sqrt(1.0 - NoV*NoV), 0.0, NoV)

	local A = 0.0
	local B = 0.0

	local N = vec(0.0, 0.0, 1.0)

	for i = 0,sampleCount,1	do
		local xi_x, xi_y = hammersley(i)
		local h_x, h_y, h_z = ImportanceSampleGGX(xi_x, xi_y, N, roughness)
		local l_x, l_y, l_z = normalize(2.0 * dot(V, H) * H - V)

		local NoL = max(L.z, 0.0);
        local NoH = max(H.z, 0.0);
        local VoH = max(dot(V, H), 0.0);

        if (NoL > 0.0) then
            local G = GeometrySmith(N, V, L, roughness);
            local G_Vis = (G * VoH) / (NoH * NoV);
            local Fc = pow(1.0 - VoH, 5.0);

            A = A + (1.0 - Fc) * G_Vis;
            B = B + Fc * G_Vis;
        end
	end

	return A / sampleCount, B / sampleCount
end
