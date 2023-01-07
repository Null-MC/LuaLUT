using System;
using System.Collections.Generic;

namespace LuaLUT.Internal.Samplers;

internal class LuaSamplers
{
    public IDictionary<string, ISampler> Samplers {get; set;}


    public LuaSamplers(IDictionary<string, ISampler> samplers)
    {
        Samplers = samplers ?? new Dictionary<string, ISampler>(StringComparer.InvariantCultureIgnoreCase);
    }

    // ReSharper disable once IdentifierTypo
    public int TexelFetch(string samplerName, object texcoord, out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        if (string.IsNullOrWhiteSpace(samplerName))
            throw new ArgumentNullException(nameof(samplerName), "Sampler name is undefined!");

        if (!Samplers.TryGetValue(samplerName, out var sampler))
            throw new ApplicationException($"Sampler '{samplerName}' not found!");

        return sampler.TexelFetch(texcoord, out pixelR, out pixelG, out pixelB, out pixelA);
    }

    // ReSharper disable once IdentifierTypo
    public int Texture(string samplerName, object texcoord, out double pixelR, out double pixelG, out double pixelB, out double pixelA)
    {
        if (string.IsNullOrWhiteSpace(samplerName))
            throw new ArgumentNullException(nameof(samplerName), "Sampler name is undefined!");

        if (!Samplers.TryGetValue(samplerName, out var sampler))
            throw new ApplicationException($"Sampler '{samplerName}' not found!");

        return sampler.Texture(texcoord, out pixelR, out pixelG, out pixelB, out pixelA);
    }
}
