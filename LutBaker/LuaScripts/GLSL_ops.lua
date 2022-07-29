if ops ~= nil then return end

local opsTable = {}

--identity
opsTable.noop = function(a   ) return      a end
--arithmetic
opsTable.add  = function(a, b) return a +  b end
opsTable.sub  = function(a, b) return a -  b end
opsTable.mul  = function(a, b) return a *  b end
opsTable.div  = function(a, b) return a /  b end
opsTable.mod  = function(a, b) return a %  b end
opsTable.pow  = function(a, b) return a ^  b end
opsTable.idiv = function(a, b) return a // b end
opsTable.unm  = function(a   ) return   -  a end
--bitwise
opsTable.band = function(a, b) return a &  b end
opsTable.bor  = function(a, b) return a |  b end
opsTable.bxor = function(a, b) return a ~  b end
opsTable.shl  = function(a, b) return a << b end
opsTable.shr  = function(a, b) return a >> b end
opsTable.bnot = function(a   ) return   ~  a end
--comparisons
opsTable.eq   = function(a, b) return a == b end
opsTable.ne   = function(a, b) return a ~= b end
opsTable.lt   = function(a, b) return a <  b end
opsTable.le   = function(a, b) return a <= b end
opsTable.gt   = function(a, b) return a >  b end
opsTable.ge   = function(a, b) return a >= b end
--booleans
opsTable.oand  = function(a, b) return a and b end
opsTable.oor   = function(a, b) return a or  b end
opsTable.onot  = function(a   ) return   not a end
--concatenation
opsTable.concat = function(a, b) return a .. b end
--length
opsTable.len = function(a) return #a end
--indexing
opsTable.index = function(tab, key) return tab[key] end
opsTable.newIndex = function(tab, key, value) tab[key] = value end
--call
opsTable.call = function(func, ...) return func(...) end

-------------------------------- make public --------------------------------

_G.ops = opsTable
