#! /bin/sh

#  Run this script in the directory where it's located
scriptDir=`dirname $0`
cd $scriptDir

#  Read LSML version # and the SHA1 checksum for that version
LibraryVer=`awk '{print $1}' version.txt`
LibrarySHA1=`awk '{print $2}' version.txt`

#  Download the specific library version
LibraryFileName=LSML-${LibraryVer}.zip
LibraryURL=http://landis-spatial.googlecode.com/files/${LibraryFileName}
DownloadDir=download
LibraryPackage=${DownloadDir}/${LibraryFileName}
if [ ! -x $DownloadDir ] ; then
  echo Making directory "$DownloadDir" ...
  mkdir $DownloadDir
fi
if [ -f $LibraryPackage ] ; then
  echo $LibraryFileName already downloaded.
else
  echo Downloading $LibraryFileName ...
  curl --progress-bar --url $LibraryURL -o $LibraryPackage
  
  echo Verifying checksum of $LibraryPackage ...
  if [ `uname` = Darwin ] ; then
    ComputedSHA1=`openssl sha1 $LibraryPackage | sed 's/^.*= //' `
  else
    ComputedSHA1=`sha1sum $LibraryPackage | sed 's/ .*//' `
  fi
  if [ "$ComputedSHA1" != "$LibrarySHA1" ] ; then
    echo ERROR: Invalid checksum
    echo Expected SHA1 = $LibrarySHA1
    echo Computed SHA1 = $ComputedSHA1
    exit 1
  fi
fi

#  Unpack the library if not done already
if [ -f Landis.SpatialModeling.dll ] ; then
  echo Library assemblies have already been unpacked.
else
  echo Unpacking $LibraryPackage ...
  unzip $LibraryPackage
fi

exit 0
