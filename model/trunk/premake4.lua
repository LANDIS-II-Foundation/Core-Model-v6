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
      "System.Xml",
      thirdPartyLibs["FLEL"]
    }

  -- Extension administration tool
  project "Extension_Admin"
    location "ext-admin"
    kind "ConsoleApp"
    targetname "Landis.Extensions"
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
      thirdPartyLibs["LSML"]
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

-- Hook in a custom function that it's called *after*
-- the selected action is executed.

require "premake4_util"

afterAction_call(
  function()
    --  If generating Visual Studio files, add HintPath elements for references
    --  with paths.
    if string.startswith(_ACTION, "vs") then
      modifyCSprojFiles()

      -- Fetch LSML and GDAL C# bindings if they're not present
      if not os.isfile(thirdPartyLibs["LSML"]) then
        LSMLadmin("get")
      end
    end

  end
)

-- The function below modifies the all the projects' *.csproj files, by
-- changing each Reference that has a path in its Include attribute to use
-- a HintPath element instead.

require "CSProjFile"

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

	if _ACTION == "vs2010" then
	  -- Target the full framework, not the Client profile
	  removeFrameworkProfile(csprojFile)
	  print("  <TargetFrameworkProfile> element removed")
	end

    ok, err = csprojFile:writeLines()
    if not ok then
      error(err, 0)
    end
  end -- for each project
end
-- ==========================================================================

-- Run the LSML-admin script with a specific action

function LSMLadmin(action)
  local onWindows = runningOnWindows()
  local scriptExt = iif(onWindows, "cmd", "sh")
  local adminScript = thirdPartyDir .. "/LSML/LSML-admin." .. scriptExt
  if onWindows then
    adminScript = path.translate(adminScript, "\\")
  end
  print("Running " .. adminScript .. " '" .. action .. "'...")
  os.execute(adminScript .. " " .. action)
end
