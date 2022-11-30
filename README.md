# LuaLUT
A small tool for baking LUT images for Minecraft shaders using Lua scripting.

## Usage
### Generate 2D PNG
Using the lua script below saved to a file "lut.lua", this will generate a 2D RGB PNG texture using the normalized texel coordinates (0-1).

```lua
function processTexel(x, y)
    return 2.0 * vec(x, y, 1.0) - 1.0
end
```

Then run LuaLUT using the following command to generate a LUT.png image.

```
lualut -s "lut.lua" -i "PNG" -f "RGB" -t "BYTE" -w 128 -h 128
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
