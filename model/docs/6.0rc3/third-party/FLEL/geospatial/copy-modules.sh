#! /bin/sh

#  Copy missing binary files to target directory

targetDir=$1
targetDirName=`basename $targetDir`

geospatialDir=`dirname $0`
geospatialBin=$geospatialDir/bin

for F in $geospatialBin/*.netmodule $geospatialBin/*.pdb ; do
  Fname=`basename $F`
  if [ ! -f $targetDir/$Fname ] ; then
    cp $geospatialBin/$Fname "$targetDir/$Fname"
    printf "Copied geospatial/bin/$Fname into $targetDirName\n"
  fi
done
