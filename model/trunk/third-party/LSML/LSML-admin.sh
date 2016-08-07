#! /bin/bash

# ----------------------------------------------------------------------------

function printUsage()
{
  cat <<EOT
Usage: $ScriptName [ACTION]
where ACTION is:
   get       -- download and unpack LSML and GDAL
   clean     -- remove all unpacked files
   distclean -- Same as "clean" action, plus remove all downloaded files
   help      -- display this message (default)
EOT
}

# ----------------------------------------------------------------------------

function processArgs()
{
  Action=
  if [ "$1" = "" ] ; then
    Action=help
  else
    case $1 in
      get | clean | distclean | help ) Action=$1;;
      *) usageError "unknown action \"$1\"";;
    esac
  fi
  if [ "$3" != "" ] ; then
    usageError "extra arguments after \"$1\" action: $2 ..."
  fi
  if [ "$2" != "" ] ; then
    usageError "extra argument after \"$1\" action: $2"
  fi
}

# ----------------------------------------------------------------------------

function usageError()
{
  printf "Error: $1\n"
  printUsage
  exit 1
}

# ----------------------------------------------------------------------------

function setEnvVars()
{
  #  Read LSML version # and the SHA1 checksum for that version
  LibraryVer=`cut -f 1 version.txt`
  LibrarySHA1=`cut -f 2 version.txt | sed 's/.$//'` # !!!! the sed part removes the WINDOWS file ending

  #  Set environment variables for the specific library version
  LibraryFileName=LSML-${LibraryVer}.zip
  LibraryURL=http://landis-spatial.googlecode.com/files/${LibraryFileName}
  DownloadDir=download
  LibraryPackage=${DownloadDir}/${LibraryFileName}
}

# ----------------------------------------------------------------------------

function getLibrary()
{
  if [ ! -x $DownloadDir ] ; then
    echo Making directory "$DownloadDir" ...
    mkdir $DownloadDir
  fi
  if [ -f $LibraryPackage ] ; then
    echo $LibraryPackage already downloaded.
  else
    echo Downloading $LibraryPackage ...
    curl --progress-bar --url $LibraryURL -o $LibraryPackage
  
    echo Verifying checksum of $LibraryPackage ...
    if [ `uname` = Darwin ] ; then
      ComputedSHA1=`openssl sha1 $LibraryPackage | sed 's/^.*= //' `
    else
      ComputedSHA1=`sha1sum $LibraryPackage | sed 's/ .*//' `
    fi
    if [ "$ComputedSHA1" != "$LibrarySHA1" ] ; then
      echo ERROR: Invalid checksum
      echo Computed SHA1 = $ComputedSHA1 ...
      echo Expected SHA1 =  $LibrarySHA1 ...
      exit 1
    else
      echo Matching checksum, continuing...
    fi
  fi


  #  Unpack the library if not done already
  if [ -f Landis.SpatialModeling.dll ] ; then
    echo $LibraryPackage has already been unpacked.
  else
    echo Unpacking $LibraryPackage ...
    unzip $LibraryPackage
  fi

  #  Get the GDAL libraries and C# bindings
  GDALadmin get
}

# ----------------------------------------------------------------------------

function cleanFiles()
{
  GDALadmin clean
  for file in *.dll README.txt ; do
    deleteFile $file
  done
}

# ----------------------------------------------------------------------------

function distClean()
{
  GDALadmin distclean
  for file in Landis.SpatialModeling.xml GDAL-version.txt GDAL-admin.* ; do
    deleteFile $file
  done

  cleanFiles
  deleteFile $LibraryPackage
  if [ -d $DownloadDir ] ; then
    rm -fR $DownloadDir
    echo Deleted $DownloadDir/
  fi
}

# ----------------------------------------------------------------------------

function deleteFile()
{
  if [ -f $1 ] ; then
    rm $1
    echo Deleted $1
  fi
}

# ----------------------------------------------------------------------------

function GDALadmin()
{
  if [ "$Action" = "get" ] ; then
    read -p "Download pre-compiled GDAL binaries for linux (with C# binding, total ~25 MB)? If you choose No, you will need to compile GDAL yourself. [Y/n]" answer
    case ${answer:0:1} in
      ""|y|Y )
         mkdir GDAL
         # Download pre-compiled binaries from github.
         curl -L --url https://github.com/jealie/binaries_GDAL_Csharp/blob/master/linux64/gdal.tar.gz?raw=true | tar -zx -C GDAL/
      ;;
      * )
          echo "LSML installer will proceed without downloading GDAL."
      ;;
    esac
  else
    deleteFile GDAL/libgdal.so.*
    deleteFile GDAL/libgdal_wrap.so
    deleteFile GDAL/gdal_csharp.dll
    if [ -d GDAL ] ; then
      rmdir GDAL
      echo Deleted GDAL
    fi
  fi
}

# ----------------------------------------------------------------------------

ScriptName=`basename $0`
processArgs $*
if [ "$Action" = "help" ] ; then
  printUsage
  exit 0
fi

#  Do the action in the directory where this script is located
OriginalWD=`pwd`
ScriptDir=`dirname $0`
cd $ScriptDir
if [ $OriginalWD == `pwd` ] ; then
  OriginalWD=
else
  echo now in `pwd`
fi

setEnvVars

export GdalAdmin_VersionFile=GDAL-version.txt
export GdalAdmin_InstallDir=GDAL

case $Action in
  get)        getLibrary;;
  clean)      cleanFiles;;
  distclean)  distClean;;
esac

if [ "$OriginalWD" != "" ] ; then
  cd $OriginalWD
  echo now in `pwd`
fi
exit 0
