function VanDerCorput(n, base)
    local invBase = 1.0 / base
    local denom = 1.0
    local result = 0.0

    for i = 0,32,1 do
        if (n > 0) then
            local denom = mod(n, 2.0)
            result = result + denom * invBase
            invBase = invBase / 2.0
            n = floor(n / 2.0)
        end
    end

    return result;
end

function Hammersley(i, N)
    return vec(i/N, VanDerCorput(i, 2))
end

function ImportanceSampleGGX(Xi, N, roughness)
    local a = roughness*roughness
	
    local phi = 2.0 * PI * Xi.x
    local cosTheta = sqrt((1.0 - Xi.y) / (1.0 + (a*a - 1.0) * Xi.y))
    local sinTheta = sqrt(1.0 - cosTheta*cosTheta)

    -- from spherical coordinates to cartesian coordinates
    local H = vec(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta)
	
    -- from tangent-space vector to world-space sample vector
    local up
    if (abs(N.z) < 0.999) then
        up = vec(0.0, 0.0, 1.0)
    else
        up = vec(1.0, 0.0, 0.0)
    end

    local tangent = normalize(cross(up, N))
    local bitangent = cross(N, tangent)
	
    local sampleVec = tangent * H.x + bitangent * H.y + N * H.z
    return normalize(sampleVec)
end

function GeometrySchlickGGX(NoV, roughness)
    local a = roughness;
    local k = (a * a) / 2.0;

    local denom = NoV * (1.0 - k) + k;
    return NoV / denom;
end

function GeometrySmith(N, V, L, roughness)
    local NoV = max(dot(N, V), 0.0);
    local NoL = max(dot(N, L), 0.0);
    local ggx2 = GeometrySchlickGGX(NoV, roughness);
    local ggx1 = GeometrySchlickGGX(NoL, roughness);

    return ggx1 * ggx2;
end

function processPixel(x, y)
	local NoV = x / width
	local roughness = y / height

	local V = vec(sqrt(1.0 - NoV*NoV), 0.0, NoV)
	local result = vec(0.0, 0.0)
	local N = vec(0.0, 0.0, 1.0)

	for i = 0,SAMPLE_COUNT,1 do
		local Xi = Hammersley(i, SAMPLE_COUNT)
		local H = ImportanceSampleGGX(Xi, N, roughness)
		local L = normalize(H * dot(V, H) * 2.0 - V)

		local NoL = max(L.z, 0.0)
        local NoH = max(H.z, 0.0)
        local VoH = max(dot(V, H), 0.0)

        if (NoL > 0.0) then
            local G = GeometrySmith(N, V, L, roughness)
            local G_Vis = (G * VoH) / (NoH * NoV)
            local Fc = pow(1.0 - VoH, 5.0)
            
            result.x = result.x + (1.0 - Fc) * G_Vis
            result.y = result.y + Fc * G_Vis
        end
	end

	return result / SAMPLE_COUNT
end
