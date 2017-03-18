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

NOTE FOR DEBUG MODE: in App.cs 3 lines must be uncommented. Search for this string 
"Pre-pending the GDAL directory is required to run the Console project in debug mode" 
and uncomment out the following 3 lines before building in Visual Studio:

     // string path = Environment.GetEnvironmentVariable("PATH");
    // string newPath = "C:\\Program Files\\LANDIS-II\\GDAL\\1.9;" + path;
    // Environment.SetEnvironmentVariable("PATH", newPath);



Staging a Configuration For Testing
-----------------------------------

After a configuration has been built, it can be "staged" for test purposes.
When staged, a configuration's files are copied into the directory structure
used for an LANDIS-II installation.  

How-to Stage Configuration:
1. retrieve Support-Library-Dlls
--------------------------------
Download https://github.com/LANDIS-II-Foundation/Support-Library-Dlls and place contents in some/path/model/libs

2. run premake5 script
------------------------
in the directory with this README file, run the premake5 script.  For example, 

  premake4.4 install Debug

This will install the Debug configuration's files along with other files into
the ../build/install/Debug/ directory.  

3. install extensions
---------------------
For general testing you can either build each extension seperatley from its respective repo and place contents in
  
  some/path/to/model/build/install/debug/v6 

OR
--

Download extensions from www.landis-ii.org and copy contents of installed files 
  FROM:   path/to/LANDIS-II/v6 
  TO:     some/path/to/model/build/install/debug/v6 

4. Running staged configuration
-------------------------------
This configuration of LANDIS-II can
be run by using the scripts in the installation's bin/ directory:

  cd some/path/to/test/scenarios
  path_to_model_project/build/install/Debug/bin/landis-ii scenario_1.txt