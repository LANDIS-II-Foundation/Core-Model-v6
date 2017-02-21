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
used for an LANDIS-II installation.  

How-to Stage Configuration:
1. retrieve Support-Library-Dlls
--------------------------------
Download https://github.com/LANDIS-II-Foundation/Support-Library-Dlls and place contents in some/path/model/libs

2. run premake4.4 script
------------------------
in the directory with this README file, run the premake4 script.  For example, 

  premake4.4 install Debug

This will install the Debug configuration's files along with other files into
the ../build/install/Debug/ directory.  

3. install extensions
---------------------
For general testing you can either build each extension seperatly from its respective repo and place contents in
some/path/to/model/build/install/debug/v6 

OR
--

Download extensions from www.landis-ii.org and copy contents of installed files 
FROM path/to/LANDIS-II/v6 TO some/path/to/model/build/install/debug/v6 

4. Running staged configuration
-------------------------------
This configuration of LANDIS-II can
be run by using the scripts in the installation's bin/ directory:
  cd some/path/to/test/scenarios
  path_to_model_project/build/install/Debug/bin/landis-ii scenario_1.txt