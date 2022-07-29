# LuaLUT
A small tool for baking LUT images for Minecraft shaders using Lua scripting.

## Usage
Using the lua script below saved to a file "lut.lua"

```lua
function processPixel(x, y)
    f = vec(x, y) / vec(width, height)
    color = vec(f.x, f.y, f.x * f.y)
    return color
end
```

You can run LUT-Baker using the following command to generate a LUT.png image.

```
lualut -s "lut.lua" -o "lut.png" -i "PNG" -f "RGB" -t "BYTE" -w 128 -h 128
```
