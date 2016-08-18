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

newoption {
  trigger     = "dist",
  description = 'Clean to distribution state (remove downloaded files too)'
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
    end

    -- Fetch LSML and GDAL C# bindings if they're not present
    if  _ACTION ~= "clean" and not os.isfile(thirdPartyLibs["LSML"]) then
      LSMLadmin("get")
    end

    -- If cleaning, then have LSML-admin clean too.
    if _ACTION == "clean" then
      LSMLadmin(iif(_OPTIONS["dist"], "distclean", "clean"))
    end

    --  Run custom pre-compilation commands on linux
    if _ACTION == "gmake" then
      linuxAddons()
    end
  end
)

-- The function below modifies the all the projects' *.csproj files, by
-- changing each Reference that has a path in its Include attribute to use
-- a HintPath element instead.

require "CSprojFile"

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

	-- Generate XML documentation for core API assembly (for inclusion in SDK)
	if prj.name == "Core" then
	  enableXMLdocumentation(csprojFile)
	  print("  Enabled the generation of XML documentation file")
	end

    -- Add version number as "-X.Y" to the console's assembly name
    if prj.name == "Console" then
      local assemblyName = appendVersionToAssemblyName(csprojFile)
      print("  Changed <AssemblyName> element to \"" .. assemblyName .. "\"")
    end

    ok, err = csprojFile:writeLines()
    if not ok then
      error(err, 0)
    end
  end -- for each project
end

-- ==========================================================================

-- Append the model's version # to the AssemblyName of a C# project

function appendVersionToAssemblyName(csprojFile)
  -- Get model version (major.minor) from the shared assembly source file
  local modelVersion = readAssemblyVersion("SharedAssemblyInfo.cs")
  local majorMinor = string.match(modelVersion, "^(%d+\.%d+)")

  local pattern = "^(%s*)<AssemblyName>(.*)</AssemblyName>"
  for i, line in ipairs(csprojFile.lines) do
    -- Look for <AssemblyName>Example.Assembly</AssemblyName>
    local indent, name = string.match(line, pattern)
    if indent then
      name = name .. "-" .. majorMinor
      csprojFile.lines[i] = string.format("%s<AssemblyName>%s</AssemblyName>",
                                          indent, name)
      return name
    end
  end
end

-- ==========================================================================

-- Read the AssemblyVersion attribute in a C# source file

function readAssemblyVersion(sourceFile)
  local pattern = "AssemblyVersion%(\"(.*)\"%)"
  for line in io.lines(sourceFile) do
    local versionStr = string.match(line, pattern)
    if versionStr then
      return versionStr
    end
  end
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

-- ===========================================================================

-- linux-specific build instructions

function linuxAddons()
  if not runningOnWindows() then
     print("Running linux specific commands...")
     local base_dir = os.getcwd()
     -- correct faulty makefiles
     os.execute("find . -name 'Makefile' | xargs sed -i -e 's/gmcs/mcs/g'")
     os.execute("find . -name 'Makefile' | xargs sed -i -e 's/dll\.dll/dll/g'")
     -- initialize the build directory
     for i,conf in pairs(configurations()) do
       -- create Debug and Release directories
       os.execute("mkdir -p build/" .. conf)
       local conf_dir = base_dir .. "/build/" .. conf
       -- copy the console .config file
       os.execute("cp console/Landis.Console-X.Y.exe.config " .. conf_dir .. "/Landis.Console.exe.config")
       -- create symbolic links to the pre-compiled GDAL libraries
       os.execute("ln -fs " .. base_dir .. "/third-party/LSML/GDAL/libgdal.so.* " .. conf_dir .. "/libgdal.so")
       os.execute("ln -fs " .. base_dir .. "/third-party/LSML/GDAL/libgdal_wrap.so " .. conf_dir .. "/libgdal_wrap.so")
       os.execute("ln -fs " .. base_dir .. "/third-party/LSML/GDAL/gdal_csharp.dll " .. conf_dir .. "/gdal_csharp.dll")
       -- create symbolic links to all other third party libraries
       for i,lib in pairs(thirdPartyLibs) do
         os.execute("ln -fs " .. base_dir .. "/" .. lib .. " " .. conf_dir .. "/")
       end
       -- build the linux executable script
       os.execute("echo '#!/bin/bash' > " .. conf_dir .. "/landis_console.sh")
       os.execute("echo 'LD_PRELOAD=\"" .. conf_dir .. "/libgdal.so " .. conf_dir .. "/libgdal_wrap.so\" exec mono " .. conf_dir .. "/Landis.Console.exe \"$@\"' >> " .. conf_dir .. "/landis_console.sh")
       os.execute("chmod +x " .. conf_dir .. "/landis_console.sh")
     end
     print("Done.")
  end
end
