#! /bin/sh

#  ---------------------------------------------------------------------------

function printHelp()
{
  echo "Usage: $0 #.#"
  echo "where #.# is the version of LANDIS-II to uninstall"
}

#  ---------------------------------------------------------------------------

function deletePath()
{
  if [ -f "$1" ] ; then
    rm -f "$1"
    echo "Deleted $1"
  elif [ -d "$1" ] ; then
    rm -fr "$1"
    echo "Deleted $1/"
  fi
}

#  ---------------------------------------------------------------------------

if [ "$1" = "" ] ; then
  printHelp
  exit 0
fi
Major=${1%%.*}
Minor=${1##*.}
if [ "$Major" = "" ] ; then
  echo "Error: Missing major version in \"$1\""
  exit 1
fi
if [ "$Minor" = "" ] ; then
  echo "Error: Missing minor version in \"$1\""
  exit 1
fi
MajorMinor=${Major}.${Minor}

#  Run in the parent directory of this script
pushd `dirname $0`/..

UninstallList=v${Major}/bin/${MajorMinor}/uninstall-list.txt
if [ ! -f $UninstallList ] ; then
  echo "LANDIS-II $MajorMinor is not installed"
  exit 0
fi

echo "Uninstalling LANDIS-II $MajorMinor"
cat $UninstallList | while read ; do
  deletePath "$REPLY"
done

popd
exit 0
