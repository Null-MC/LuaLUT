# LuaLUT
A small tool for baking LUT images for Minecraft shaders using Lua scripting.

## Usage
Using the lua script below saved to a file "lut.lua"

```lua
function processPixel(x, y)
    f = vec(x, y) / vec(width, height)
    return vec(f.x, f.y, f.x * f.y)
end
```

You can run LUT-Baker using the following command to generate a LUT.png image.

```
lualut -s "lut.lua" -o "lut.png" -i "PNG" -f "RGB" -t "BYTE" -w 128 -h 128
```

## Custom Variables
You can pass custom variables in from the commandline to your lua script.

```lua
function processPixel(x, y)
    return vec(red, green, blue)
end
```

```
lualut <...> -var "red=1.0;green=0.5;blue=0.0"
```
