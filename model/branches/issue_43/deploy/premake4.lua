-- Copyright 2012,2013 Green Code LLC
-- All rights reserved. 
--
-- Licensed under the Apache License, Version 2.0 (the "License");
-- you may not use this file except in compliance with the License.
-- You may obtain a copy of the License at
--
--     http://www.apache.org/licenses/LICENSE-2.0
--
-- Unless required by applicable law or agreed to in writing, software
-- distributed under the License is distributed on an "AS IS" BASIS,
-- WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
-- See the License for the specific language governing permissions and
-- limitations under the License.
--
-- Contributors:
--   James Domingo, Green Code LLC

-- ============================================================================

function main()
  if not _ACTION or _ACTION == "help" then
    printHelp()
    return 0
  end
  if _ACTION ~= "install" then
    print('Error: invalid action \"'.._ACTION..'"')
    printHelp()
    return 1
  end
  if # _ARGS == 0 then
    print("No configurations specified on command line.")
	printHelp()
	return 0
  end

  installDir = "../build/install"

  majorVersion, minorVersion = readMajorMinor("../SharedAssemblyInfo.cs")
  majorMinor = majorVersion..'.'..minorVersion
  releaseStatus = readFirstLine("release-status.txt")
  GDALversion = readFirstLine("../third-party/LSML/GDAL-version.txt")
  GDALmajorMinor = string.match(GDALversion, "^(%d+\.%d+)")
  print("Installing LANDIS-II "..majorMinor.." ("..releaseStatus..") with GDAL "..GDALversion)

  for _, config in ipairs(_ARGS) do
    configInstallDir = installDir.."/"..config
    installConfig(config)
  end

  return 0
end

-- ============================================================================

function printHelp()
  local premakeName = path.getname(_PREMAKE_COMMAND)
  print()
  print("Usage: "..premakeName.." action [configurations]")
  print()
  print("ACTIONS")
  print()
  print(" install    Install configuration(s) into ../build/install/CONFIG")
  print(" help       Display this message")
 end

-- ============================================================================

function readMajorMinor(pathToAssemblyInfoSrc)
  for line in io.lines(pathToAssemblyInfoSrc) do
    local major, minor = string.match(line, 'AssemblyVersion%("(%d+).(%d+).*"%)')
    if major then
      return major, minor
    end
  end
  return nil
end

-- ============================================================================

function readFirstLine(filePath)
  for line in io.lines(filePath) do
    return line
  end
  return nil -- if file is empty (no lines)
end

-- ============================================================================

-- Install a build configuration

function installConfig(config)
  print("Installing "..config.." configuration into "..configInstallDir.."/ ...")

  if not os.isdir("../build/"..config) then
    print("Error: The "..config.." configuration has not been built")
    return
  end

  installCount = 0

  -- {InstallDir}/bin/

  install { file="bin/landis.cmd",            from="deploy/bin/" }
  install { file="bin/landis-ii.cmd",         from="deploy/bin/" }
  install { file="bin/landis-{X.Y}.cmd",      from="deploy/bin/landis-X.Y.cmd", replace={ ["{VERSION}"]=GDALmajorMinor } }

  install { file="bin/landis-extensions.cmd",      from="deploy/bin/" }
  install { file="bin/landis-v{X}-extensions.cmd", from="deploy/bin/landis-vX-extensions.cmd" }

  install { file="bin/uninstall-landis.cmd",  from="deploy/bin/" }
  install { file="bin/uninstall-landis.sh",   from="deploy/bin/" }

  -- {InstallDir}/vX/bin/

  GDALcsharpVersion = getAssemblyVersion("../third-party/LSML/GDAL/managed/gdal_csharp.dll")  

  install { file="v{X}/bin/Landis.Console-{X.Y}.exe",        from="build/"..config.."/Landis.Console-"..majorMinor..".exe" }
  install { file="v{X}/bin/Landis.Console-{X.Y}.exe.config", from="console/Landis.Console-X.Y.exe.config",
                                                             replace={ ["{RELEASE}"]=releaseStatus,
                                                                       ["{GDAL_CSHARP_VERSION}"]=GDALcsharpVersion } }

  install { file="v{X}/bin/Landis.Extensions.exe",        from="build/"..config }
  install { file="v{X}/bin/Landis.Extensions.exe.config", from="ext-admin/Landis.Extensions.exe.config" }

  -- {InstallDir}/vX/bin/extensions/

  install { file="v{X}/bin/extensions/Landis.Extensions.Dataset.dll", from="build/"..config }

  -- {InstallDir}/vX/bin/X.Y/

  install { file="v{X}/bin/{X.Y}/Landis.Core.dll",                from="build/"..config }
  install { file="v{X}/bin/{X.Y}/Landis.Core.Implementation.dll", from="build/"..config }

  install { file="v{X}/bin/{X.Y}/log4net.dll",                    from="third-party/log4net/bin/" }
  install { file="v{X}/bin/{X.Y}/Edu.Wisc.Forest.Flel.Util.dll",  from="third-party/FLEL/util/bin/" }
  install { file="v{X}/bin/{X.Y}/Troschuetz.Random.dll",          from="third-party/Troschuetz/" }

  install { file="v{X}/bin/{X.Y}/Landis.SpatialModeling.dll",     from="third-party/LSML/" }
  install { file="v{X}/bin/{X.Y}/Landis.Landscapes.dll",          from="third-party/LSML/" }
  install { file="v{X}/bin/{X.Y}/Landis.RasterIO.dll",            from="third-party/LSML/" }
  install { file="v{X}/bin/{X.Y}/Landis.RasterIO.Gdal.dll",       from="third-party/LSML/" }
  install { file="v{X}/bin/{X.Y}/gdal_csharp.dll",                from="third-party/LSML/GDAL/managed/" }

  install { file="v{X}/bin/{X.Y}/uninstall-list.txt",             from="deploy/", replace={ ["{X}"]=majorVersion,
                                                                                            ["{X.Y}"]=majorMinor } }

  -- {InstallDir}/GDAL/#.#/

  install { dir="GDAL/"..GDALmajorMinor, source="third-party/LSML/GDAL/native/" }

  -- {InstallDir}/vX/licenses/

  install { file="v{X}/licenses/+NOTICE.txt",                                from="+NOTICE.txt" }
  install { file="v{X}/licenses/Apache License 2.0.txt",                     from="third-party/log4net/LICENSE.txt" }
  install { file="v{X}/licenses/GNU Lesser General Public License 2.1.txt",  from="third-party/Troschuetz/LICENSE.txt" }
  install { file="v{X}/licenses/LANDIS-II Binary License.rtf",               from="deploy/windows/LANDIS-II_Binary_license.rtf" }

  if installCount == 0 then
    print("All installed files are up-to-date.")
  end
end

-- ============================================================================

-- Install a file or directory

function install(info)
  -- Install a file
  if info.file then
    -- The destination path is relative to the configuration's installation
    -- directory.
    local filePath = string.gsub(info.file, "{X.Y}", majorMinor)
    filePath = string.gsub(filePath, "{X}", majorVersion)
    local destPath = configInstallDir.."/"..filePath

	-- The source path is relative to the project's root directory.
    local srcPath = "../"..trimTrailingSlash(info.from)
    if string.endswith(info.from, "/") and not os.isdir(srcPath) then
      error('No such directory "'..srcPath..'"')
    elseif os.isdir(srcPath) then
      srcPath = path.join(srcPath, path.getname(destPath))
    elseif not os.isfile(srcPath) then
      error('No such file "'..srcPath..'"')
    end
    installFile(srcPath, destPath, info.replace)

  -- Install a directory
  elseif info.dir then
    -- The destination path is relative to the configuration's installation
    -- directory.
    local destDir = configInstallDir.."/"..trimTrailingSlash(info.dir)

    if os.isfile(destDir) then
      error('"'..destDir..'" is not a directory')
    end
    if not os.isdir(destDir) then
      makeDir(destDir)
    end

	-- The source directory is relative to the project's root directory.
    local srcDir = "../"..trimTrailingSlash(info.source)
    if not os.isdir(srcDir) then
      error('No such directory "'..srcDir..'"')
    end

    copyDir(srcDir, destDir)

  else
    error("no file or directory specified")
  end
end

-- ============================================================================

-- Trim trailing separator from a path string.  Necessary because os.dir on
-- Windows doesn't work with a trailing slash (forward or backward).

function trimTrailingSlash(pathStr)
  if pathStr:endswith("/") then
    return pathStr:sub(1,-2)
  else
    return pathStr
  end
end

-- ============================================================================

-- Install (copy) a file to a specific location if the file doesn't exist at
-- location or if the installed file is out-of-date.

function installFile(srcPath, destPath, textReplacements)
  if os.isdir(destPath) then
    error(destPath.." is a directory, not a file")
  end
  if isUpToDate(destPath, srcPath) then
    return
  end

  local destDir = path.getdirectory(destPath)
  if not os.isdir(destDir) then
    makeDir(destDir)
  end

  if textReplacements then
    copyFileAndReplaceText(srcPath, destPath, textReplacements)
  else
    if not os.copyfile(srcPath, destPath) then
      error("Cannot copy "..srcPath.." to "..destPath)
    end
  end

  print("Installed "..destPath)
  if printFrom then
    print("     from "..srcPath) 
  end

  installCount = installCount + 1
end

-- ============================================================================

function copyDir(srcDir, destDir)
  local filesInSrcDir = os.matchfiles(srcDir.."/*")
  for i, srcPath in ipairs(filesInSrcDir) do
    local destPath = destDir.."/"..path.getname(srcPath)
    installFile(srcPath, destPath)
  end

  local dirsInSrcDir = os.matchdirs(srcDir.."/*")
  for i, srcPath in ipairs(dirsInSrcDir) do
    local destPath = destDir.."/"..path.getname(srcPath)
    copyDir(srcPath, destPath)
  end
end
 
-- ============================================================================

function makeDir(dirPath)
  if os.mkdir(dirPath) then
    print("Created directory "..dirPath.."/")
  else
    error("Cannot create directory "..dirPath.."/")
  end
end

-- ============================================================================

-- Is a destination file up-to-date with a source file?
--
-- The destination file is up-to-date if its modification time is newer than
-- the source file's modification time, or their modification times and sizes
-- are both equal.

function isUpToDate(destPath, srcPath)
  if os.isdir(destPath) then
    error('"' .. destPath .. '" is a directory, not a file', 2)
  elseif not os.isfile(destPath) then
    return false
  else
    local destStat = os.stat(destPath)
    local srcStat = os.stat(srcPath)
    if srcStat.mtime < destStat.mtime then
      -- destination file is newer
      return true
    elseif destStat.mtime < srcStat.mtime then
      -- source file is newer
      return false
    else
      -- mod times are equal; their sizes must also be equal.
      return srcStat.size == destStat.size
    end
  end
end

-- ============================================================================

-- Copy a file and replace certain text strings in its contents.

function copyFileAndReplaceText(srcPath, destPath, textReplacements)
  local outFile, errMessage = io.open(destPath, "w")
  if not outFile then
    error(errMessage)
  end
  if textReplacements == nil then
    textReplacements = {}
  end
  for line in io.lines(srcPath) do
    for text, replacement in pairs(textReplacements) do
      line = string.gsub(line, text, replacement)
    end
    outFile:write(line.."\n")
  end
  io.close(outFile)
end

-- ============================================================================

-- Get the version number of an assembly.

function getAssemblyVersion(assemblyPath)
  local scriptEngine = path.translate("../third-party/CS-Script/cscs.exe")
  local outFileName = "assembly-version.txt"
  assemblyPath = path.translate(assemblyPath)
  local status = os.execute(scriptEngine.." /nl assembly-version.cs "..assemblyPath.." >"..outFileName)
  if status ~= 0 then
    error("Unable to get assembly version for '"..assemblyPath.."'")
  end
  local version = readFirstLine(outFileName)
  return version
end

-- ============================================================================

exitCode = main()
os.exit(exitCode)
