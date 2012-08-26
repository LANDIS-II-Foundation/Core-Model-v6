-- Copyright 2012 Green Code LLC
-- All rights reserved.
--
-- The copyright holders license this file under the New (3-clause) BSD
-- License (the "License").  You may not use this file except in
-- compliance with the License.  A copy of the License is available at
--
--   http://www.opensource.org/licenses/BSD-3-Clause
--
-- and is included in the NOTICE.txt file distributed with this work.
--
-- Contributors:
--   James Domingo, Green Code LLC
-- ==========================================================================

-- Utility functions for Premake
--
-- Revision history:
--   2012-08-26 : Initial release

-- ==========================================================================

-- Returns a boolean indicating where Premake is running on Windows

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
--   function myCustomFunc()
--     -- do something with generated project files
--     ...
--   end
--
--   afterAction_call(myCustomFunc)

function afterAction_call(func)
  if _ACTION then
    local action = premake.action.get(_ACTION)
    if not action then
      -- An unknown action was specified (typing mistake?)
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
