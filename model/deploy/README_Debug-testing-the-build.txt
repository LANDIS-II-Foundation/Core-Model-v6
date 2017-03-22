Title:			README_Debug-testing-the-build
Project:		LANDIS-II Landscape Change Model
Project Component:	Core Model
Component Deposition:	https://github.com/LANDIS-II-Foundation/Core-Model
Author:			LANDIS-II Foundation
Origin Date:		17 Mar 2017
Final Date:		




NOTE FOR DEBUG TESTING MODE: Three (3) lines must be uncommented in "App.cs" PRIOR to building
the LANDIS-II.sln file. Search "App.cs" for this string,
"Pre-pending the GDAL directory is required to run the Console project in debug mode" 

Uncomment out the following three (3) lines before building LANDIS-II.sln in Visual Studio:
     // string path = Environment.GetEnvironmentVariable("PATH");
    // string newPath = "C:\\Program Files\\LANDIS-II\\GDAL\\1.9;" + path;
    // Environment.SetEnvironmentVariable("PATH", newPath);


#####################################
Debug Testing a Staged Configuration 
#####################################

After a configuration has been built, it can be "staged" for test purposes.
When staged, a configuration's files are copied into the same directory structure
as that used for a LANDIS-II installation. The only difference is that the directory
structure is not located at C:\Program Files\LANDIS-II 

	a. To create the directory structure required for Debug testing, 
	   see "Stage Four Rebuild -- Staging for Installer Prep or Testing" in
	   README_CoreModel6.1.txt.

	b. Install one or more extensions. For general testing you can either
	b1.  build each extension seperatley from its respective repo and place the contents in,
	   ...\model\build\install\debug\v6 

			OR

	b2. Download extensions from www.landis-ii.org and copy contents of the installed files 
  	    FROM:   path/to/LANDIS-II/v6 
  	    TO:     .../model/build/install/debug/v6 


	c. Debug testing is done by running  one or more of the scenario.txt scripts in an installation's 
	   bin/ directory at the (ADM) command line. As, for example,
		
	   C:\...\build\install\Debug\bin>landis-ii some\path\to\test\scenarios\scenario.txt