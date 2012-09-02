-- Utility functions for Premake v4
-- Copyright 2012 Green Code LLC
--
-- This library is free software; you can redistribute it and/or
-- modify it under the terms of the GNU Lesser General Public
-- License as published by the Free Software Foundation; either
-- version 2.1 of the License, or (at your option) any later version.
-- 
-- This library is distributed in the hope that it will be useful,
-- but WITHOUT ANY WARRANTY; without even the implied warranty of
-- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
-- GNU Lesser General Public License for more details.
-- 
-- You should have received a copy of the GNU Lesser General Public
-- License along with this library; if not, see
-- <http://www.gnu.org/licenses/>.
--
-- Contributors:
--   James Domingo, Green Code LLC
-- ==========================================================================

-- Release history:
--   2012-08-26 : Initial release
--   2012-09-01 : Changed license from BSD to LGPL.
--                Improved documentation in comments.

-- ==========================================================================

-- Returns a boolean indicating whether Premake is running on Windows or not

function runningOnWindows()
  if string.match(_PREMAKE_VERSION, "^4.[123]") then
    -- Premake 4.3 or earlier.  Since os.getversion() added in Premake 4.4, use
    -- a simple test (does PATH env var have ";"?) to determine if on Windows.
    return string.find(os.getenv("PATH"), ";")
  else
	-- Premake 4.4 or later
    local osVersion = os.getversion()
    return string.find(osVersion.description, "Windows")
  end
end

-- ==========================================================================

-- Hook in a custom function that will be called *after* the selected Premake
-- action is executed.
--
-- Example:
--
--   afterAction_call(myCustomFunc)
--
--   function myCustomFunc()
--     if _ACTION and _ACTION ~= "clean" then
--       -- do something with generated project files
--       ...
--     end
--   end

function afterAction_call(func)
  if _ACTION then
    local action = premake.action.get(_ACTION)
    if not action then
      -- An unknown action was specified (user made typing mistake?)
    else
      local triggerAction = premake.action.get(action.trigger)
      local originalExecute = triggerAction.execute
      triggerAction.execute = function()
        if originalExecute then originalExecute() end
        func()
      end
    end
  end
end
