thirdPartyDir = "third-party"
thirdPartyLibs = {
  FLEL          = thirdPartyDir .. "/FLEL/util/bin/Edu.Wisc.Forest.Flel.Util.dll",
  LSML          = thirdPartyDir .. "/LSML/Landis.SpatialModeling.dll",
  Landscapes    = thirdPartyDir .. "/LSML/Landis.Landscapes.dll",
  RasterIO      = thirdPartyDir .. "/LSML/Landis.RasterIO.dll",
  RasterIO_Gdal = thirdPartyDir .. "/LSML/Landis.RasterIO.Gdal.dll",
  log4net       = thirdPartyDir .. "/log4net/bin/log4net.dll",
  Troschuetz    = thirdPartyDir .. "/Troschuetz/Troschuetz.Random.dll"
}

-- Are we running on Windows?
if string.match(_PREMAKE_VERSION, "^4.[123]") then
  -- Premake 4.3 or earlier.  Since os.getversion() added in Premake 4.4, use
  -- a simple test (does PATH env var have ";"?) to determine if on Windows.
  onWindows = string.find(os.getenv("PATH"), ";")
else
  -- Premake 4.4 or later
  local osVersion = os.getversion()
  onWindows = string.find(osVersion.description, "Windows")
end

-- Fetch LSML if it's not present and we're generating project files
if _ACTION and _ACTION ~= "clean" then
  if not os.isfile(thirdPartyLibs["LSML"]) then
    print("Fetching LSML ...")
    local scriptExt = iif(onWindows, "cmd", "sh")
    local LSMLscript = thirdPartyDir .. "/LSML/get-LSML." .. scriptExt
    if onWindows then
      LSMLscript = path.translate(LSMLscript, "\\")
    end
    os.execute(LSMLscript)
  end
end

buildDir = "build"

-- ==========================================================================

solution "LANDIS-II"

  language "C#"    -- by default, Premake uses "Any CPU" for platform
  framework "3.5"

  configurations { "Debug", "Release" }
 
  configuration "Debug"
    defines { "DEBUG" }
    flags { "Symbols" }
    targetdir ( buildDir .. "/Debug" )
 
  configuration "Release"
    flags { "OptimizeSize" }
    targetdir ( buildDir .. "/Release" )
 
  -- The core's API (referenced by LANDIS-II extensions)
  project "Core"
    location "core/src"
    kind "SharedLib"
    targetname "Landis.Core"
    files {
      "SharedAssemblyInfo.cs",
      "core/src/*.cs"
    }
    links {
      "System",
      "System.Core",
      thirdPartyLibs["FLEL"],
      thirdPartyLibs["LSML"],
      thirdPartyLibs["Troschuetz"]
    }

  -- The core's implementation
  project "Core_Implementation"
    location "core/src/Implementation"
    kind "SharedLib"
    targetname "Landis.Core.Implementation"
    files {
      "SharedAssemblyInfo.cs",
      "core/src/Implementation/**.cs"
    }
    links {
      "Core",
      "System",
      "System.Core",
      thirdPartyLibs["FLEL"],
      thirdPartyLibs["LSML"],
      thirdPartyLibs["log4net"],
      thirdPartyLibs["Troschuetz"]
    }

  -- Unit tests for Species module
  project "Species_Tests"
    location "core/test/species"
    kind "SharedLib"
    targetname "Landis.Species.Tests"
    files {
      "SharedAssemblyInfo.cs",
      "test-util/Data.cs",
      "core/test/ModuleData.cs",
      "core/test/species/**.cs"
    }
    links {
      "Core",
      "Core_Implementation",
      "nunit.framework",
      "System",
      "System.Core",
      thirdPartyLibs["FLEL"]
    }

  -- Unit tests for Ecoregions module
  project "Ecoregions_Tests"
    location "core/test/ecoregions"
    kind "SharedLib"
    targetname "Landis.Ecoregions.Tests"
    files {
      "SharedAssemblyInfo.cs",
      "test-util/Data.cs",
      "core/test/ModuleData.cs",
      "core/test/ecoregions/**.cs"
    }
    links {
      "Core",
      "Core_Implementation",
      "nunit.framework",
      "System",
      "System.Core",
      thirdPartyLibs["FLEL"],
      thirdPartyLibs["LSML"],
      thirdPartyLibs["RasterIO"]
    }

  -- Extension dataset library
  project "Extension_Dataset"
    location "ext-dataset/src"
    kind "SharedLib"
    targetname "Landis.Extensions.Dataset"
    files {
      "SharedAssemblyInfo.cs",
      "ext-dataset/src/*.cs"
    }
    links {
      "Core",
      "System",
      "System.Core",
      "System.Xml"
    }

  -- Extension administration tool
  project "Extension_Admin"
    location "ext-admin"
    kind "ConsoleApp"
    targetname "Landis.Extensions.Admin"
    files {
      "SharedAssemblyInfo.cs",
      "ext-admin/*.cs"
    }
    links {
      "Core",
      "Extension_Dataset",
      "System",
      "System.Core",
      "System.Xml",
      thirdPartyLibs["FLEL"],
    }

  -- Console user interface
  project "Console"
    location "console"
    kind "ConsoleApp"
    targetname "Landis.Console"
    files {
      "SharedAssemblyInfo.cs",
      "console/*.cs"
    }
    links {
      "Core",
      "Core_Implementation",
      "Extension_Dataset",
      "System",
      "System.Configuration",
      "System.Core",
      thirdPartyLibs["FLEL"],
      thirdPartyLibs["LSML"],
      thirdPartyLibs["Landscapes"],
      thirdPartyLibs["RasterIO"],
      thirdPartyLibs["RasterIO_Gdal"],
      thirdPartyLibs["log4net"]
    }

-- ==========================================================================

-- Hook in a custom function "postActionExecute" so that it's called *after*
-- the selected action is executed.

if _ACTION then
  local action = premake.action.get(_ACTION)
  if not action then
    -- An unknown action was specified (typing mistake?)
  else
    local triggerAction = premake.action.get(action.trigger)
    local originalExecute = triggerAction.execute
    triggerAction.execute = function()
      if originalExecute then originalExecute() end
      postActionExecute()
    end
  end
end

function postActionExecute()
  --  If generating Visual Studio files, add HintPath elements for references
  --  with paths.
  if string.startswith(_ACTION, "vs") then
    modifyCSprojFiles()
  end
end


-- The function below modifies the all the projects' *.csproj files, by
-- changing each Reference that has a path in its Include attribute to use
-- a HintPath element instead.

function modifyCSprojFiles()
  for i, prj in ipairs(solution().projects) do
    local csprojFile = CSprojFile(prj)
    print("Modifying " .. csprojFile.relPath .. " ...")
    csprojFile:readLines()

    adjustReferencePaths(csprojFile)
    print("  <HintPath> elements added to the project's references")
    if _PREMAKE_VERSION == "4.3" then
      -- In 4.3, the "framework" function doesn't work
      addFramework(csprojFile)
      print("  <TargetFrameworkVersion> element added to the project's properties")
    end

    ok, err = csprojFile:writeLines()
    if not ok then
      error(err, 0)
    end
  end -- for each project
end

-- ==========================================================================

-- A class that represents a project's .csproj file.

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
