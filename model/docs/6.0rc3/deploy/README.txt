LANDIS-II's Release Status
--------------------------

Enter the software's release status on the first line of the file called
"release-status.txt".  The format of the release status is one of these:

    alpha release (#)
    beta release (#)
    release candidate (#)
    official release

where (#) is an integer = or > 1.  For example:

    alpha release 3
    beta release 2
    release candidate 1


Staging a Configuration For Testing
-----------------------------------

After a configuration has been built, it can be "staged" for test purposes.
When staged, a configuration's files are copied into the directory structure
used for an LANDIS-II installation.  To stage a configuration, run Premake
in the directory with this README file.  For example, 

  premake4.4 install Debug

This will install the Debug configuration's files along with other files into
the ../build/install/Debug/ directory.  This configuration of LANDIS-II can
be run by using the scripts in the installation's bin/ directory:

  cd some/path/to/test/scenarios
  path_to_model_project/build/install/Debug/bin/landis-ii scenario_1.txt
