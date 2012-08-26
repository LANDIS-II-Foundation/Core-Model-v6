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

-- Class and utility functions for C# project files (*.csproj)
--
-- Revision history:
--   2012-08-26 : Initial release

-- ==========================================================================

-- A class that represents a C# project's .csproj file.
--
-- For simplicity, the XML in the file isn't parsed.  Instead, the lines in
-- file are stored (assumes the file is formatted in line-oriented way).

require 'class'
CSprojFile = class()

function CSprojFile:__init(proj)
  self.project = proj
  self.name = proj.name .. ".csproj"
  self.absDir = proj.location
  self.absPath = path.join(self.absDir, self.name)
  self.relDir = path.getrelative(os.getcwd(), proj.location)
  self.relPath = path.join(self.relDir, self.name)
  self.lines = nil
end

function CSprojFile:readLines()
  self.lines = { }
  for line in io.lines(self.absPath) do
    table.insert(self.lines, line)
  end
end

-- Write the modified lines out to the .csproj file.
-- Return true if successful; otherwise, return false, error message
function CSprojFile:writeLines()
  local outFile, errMessage = io.open(self.absPath, "w")
  if not outFile then
    return false, string.format("Cannot open \"%s\" for writing: %s",
                                self.relPath, errMessage)
  end
  for _, line in ipairs(self.lines) do
    outFile:write(line .. "\n")
  end
  outFile:close()
  return true
end

-- ==========================================================================

-- MonoDevelop cannot find referenced assemblies which have paths in their
-- Include attribute (see https://bugzilla.xamarin.com/show_bug.cgi?id=6008).

-- This function scans the lines of a *.csproj file for Reference elements
-- with a path in their Include attribute.  For each such Reference, the
-- path is removed from its Include attribute, and inserted into a new
-- HintPath element.  For example, this line:
--
--    <Reference Include="some\path\to\Example.Assembly.dll" />
--
-- is replaced with 3 lines:
--
--    <Reference Include="Example.Assembly">
--      <HintPath>some\path\to\Example.Assembly.dll</HintPath>
--    </Reference>

function adjustReferencePaths(csprojFile)
  local lines = { }
  for _, line in ipairs(csprojFile.lines) do
    -- Look for <Reference Include="some\path\to\Example.Assembly.dll" />
    local pattern = "(%s*)<Reference%s+Include=\"(.*)\\(.+)\""
    local indent, path, fileName = string.match(line, pattern)
    if path then
      fileNoExt = string.gsub(fileName, "\.[^.]+$", "")
      local newLines = {
        string.format("<Reference Include=\"%s\">", fileNoExt) ,
        string.format("  <HintPath>%s\\%s</HintPath>", path, fileName) ,
	              "</Reference>"
      }
      for _, newLine in ipairs(newLines) do
        table.insert(lines, indent .. newLine)
      end
    else
      table.insert(lines, line)
    end
  end -- for each line in file
  csprojFile.lines = lines
end

-- ==========================================================================

-- This function adds a new line with the target framework, right after the
-- line with the assembly's name:
--
--    <AssemblyName>Landis.Core</AssemblyName>
--    <TargetFrameworkVersion>v3.5</TargetFrameworkVersion>   <-- new line
--
-- This is only needed in Premake 4.3 because the framework function is
-- broken in that version.  For example:
--
--    if _PREMAKE_VERSION == "4.3" then
--      -- In 4.3, the "framework" function doesn't work
--      addFramework(csprojFile)
--      print("  <TargetFrameworkVersion> element added to the project's properties")
--    end

function addFramework(csprojFile)
  local framework = csprojFile.project.framework or solution().framework
  for i, line in ipairs(csprojFile.lines) do
    -- Look for <Reference Include="some\path\to\Example.Assembly.dll" />
    local pattern = "^(%s*)<AssemblyName>"
    local indent = string.match(line, pattern)
    if indent then
      local frameworkLine = string.format(
        "%s<TargetFrameworkVersion>v%s</TargetFrameworkVersion>",
        indent, framework)
      table.insert(csprojFile.lines, i + 1, frameworkLine)
      break
    end
  end -- for each line in file
end
