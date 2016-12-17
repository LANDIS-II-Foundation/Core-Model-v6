-- Based on http://lua-users.org/wiki/SimpleLuaClasses
-- Simplified to remove inheritance
--
-- Example:
--
--   Person = class()
--
--   function Person:__init(name, age)
--       self.name = name
--       self.age = age
--   end
--
--   function Person:sayHi()
--       print("Hi, my name is " .. self.name)
--   end
--
--   Bob = Person("Bob", 41)
--   Bob:sayHi()

function class()
  local c = {}    -- a new class instance

  -- the class will be the metatable for all its objects,
  -- and they will look up their methods in it.
  c.__index = c

  -- expose a constructor which can be called by <classname>(<args>)
  local mt = {}
  mt.__call = function(class_tbl, ...)
    local obj = {}
    setmetatable(obj, c)
    -- call the class:__init method if it's been defined
    if class_tbl.__init then
      class_tbl.__init(obj, ...)
    end
    return obj
  end

  setmetatable(c, mt)
  return c
end
