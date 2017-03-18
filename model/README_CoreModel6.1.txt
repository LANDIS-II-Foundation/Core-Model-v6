Title:			README_CoreModel6.1
Project:		LANDIS-II Landscape Change Model
Project Component:	Core Model
Component Deposition:	https://github.com/LANDIS-II-Foundation/Core-Model
Author:			LANDIS-II Foundation
Origin Date:		17 Mar 2017
Final Date:		



The procedure below describes building, staging, and testing Core Model in Windows 10.
See the README_LINUX.txt file for instructions on how to build and run
LANDIS-II on Linux, along with a list of troubleshooting tips.


#############################################
Stage One Rebuild -- Support libraries
#############################################

NB. Building the Core Model requires various .dll dependencies.
To ensure the latest versions, these libraries are downloaded
just prior to build and placed in a new folder called, "Libs".

	a. run the following at the (ADM) command line prompt:
C:\Users\...\Core-Model\model>install-libs_CoreModel.cmd



################################
Stage Two Rebuild -- Premake
#################################

Useful Premake websites:
https://github.com/premake/premake-4.x/wiki
http://premake.github.io/


The Premake build configuration tool is needed to generate the C# solution file 
(.sln) and project files (.csproj) for rebuilding Core Model. Premake will generate 
the requisite Visual Studio files ( the .csproj files and a .sln file) for a rebuild.
Premake itself is built on Lua, a fast, light-weight scripting language. Premake 
scripts are actually Lua programs (.lua files). Using Visual Studio 2010 or greater is
highly recommended. 


NB. If you going to Visual Studio/C# Express to build, you'll need to install
the NUnit testing framework (http://www.nunit.org).  The unit tests require
that framework.  Note that NUnit is included with SharpDevelop and
MonoDevelop.


NB. Currently, the rebuild process is set for the use of Premake5.0 with an output of VS 2015 .sln
and .csproj files. Premake will look for a file named, "premake5.lua" by default, much like 
"make" looks for a file named Makefile.




	a. If not already present, download a copy of the latest Premake 5.0 (alpha)  
	   (http://premake.github.io/) and place "premake5.exe" in the folder with this README.txt
	   file. (It is also possible to install Premake in your PATH.)

	b. run the ...\model\premake5.lua script
	b1. run the following at the (ADM) command line prompt:
C:\Users\...\CoreModel\Core-Model\model>premake5 vs2015


	c. the following files will be created:
 	model/LANDIS-II.sln
        model/console/Console.csproj
        model/core/src/Core.csproj
        model/core/src/Implementation/Core_Implementation.csproj
        model/core/test/ecoregions/Ecoregions_Tests.csproj
        model/core/test/species/Species_Tests.csproj
        model/ext-admin/Extension_Admin.csproj
        model/ext-dataset/src/Extension_Dataset.csproj
 
	

############################################
Stage Three Rebuild -- Visual Studio build
############################################


NB. The "LANDIS-II.sln" file can be opened in a variety of IDE environments including,
Visual Studio, Visual C# Express, SharpDevelop, MonoDevelop.


	a. Open the LANDIS-II.sln file in Visual Studio 2015

	b. Build the solution; seven, .csproj files are built. VS output:

========== Build: 7 succeeded, 0 failed, 0 up-to-date, 0 skipped ========== 



############################################
Stage Four Rebuild -- Staging
############################################

After a solution has been built, it is called a "configuration". A configuration can 
be "staged" for test purposes. When staged, all of a configuration's files are copied into a directory 
structure that will be used for a LANDIS-II installation. 

NB. Running the following .lua script will install all of the ...\model\buildDebug configuration's files, 
along with other files, into the ..\build\install\Debug\ directory.  This directory is subsequently accessed
by the InnoSetup installer (LANDIS-II.iss) to create an .exe installer.


	a. run the ...\model\deploy\premake5.lua script
	a1. run the following at the (ADM) command line prompt:
C:\Users\...\Core-Model\model\deploy>premake5 install Debug



############################################
Stage Five Rebuild -- Installer
############################################


	a. open "...\model\deploy\installer\LANDIS-II.iss" in Inno Script Studio

	b. compile LANDIS-II.iss script (F9)









The unit tests can be run within SharpDevelop and MonoDevelop.  To run the
tests with Visual Studio, you'll need to install a third-party add-in such
as:

  Visual Nunit   -- https://sourceforge.net/projects/visualnunit/
  TestDriven.Net -- http://www.testdriven.net/

Note that Visual C# Express does not allow add-ins, so you will have to run
the unit tests with NUnit's GUI.  Open the GUI, and then in the File menu,
select "Open Project...".  Navigate to one of the configuration build
directories (build/Debug, build/Release), and select one of the test
assemblies (e.g., Landis.*.Tests.dll).  After NUnit loads the tests, click
the "Run" button.





