Title:			README_CoreModel6.2.1
Project:		LANDIS-II Landscape Change Model
Project Component:	Core Model
Component Deposition:	https://github.com/LANDIS-II-Foundation/Core-Model
Author:			LANDIS-II Foundation
Origin Date:		17 Mar 2017
Final Date:		16 Jun 2017



The procedure below describes building, staging, and testing Core Model in Windows 10.
See the README_LINUX.txt file for instructions on how to build and run
LANDIS-II on Linux, along with a list of troubleshooting tips.

NB. The unit tests can be run directly within MonoDevelop. 
To run the unit tests with Visual Studio, you'll need to install a third-party add-in such
as:
  Visual Nunit   -- https://sourceforge.net/projects/visualnunit/
  TestDriven.Net -- http://www.testdriven.net/

It is also possible to run the unit tests with NUnit's GUI.  Open the GUI, and then in the File menu,
select "Open Project...".  Navigate to one of the configuration build directories (build/Debug, build/Release), 
and select one of the test assemblies (e.g., Landis.*.Tests.dll).  After NUnit loads the tests, click
the "Run" button.




#############################################
Stage One Rebuild -- Add support libraries
#############################################

Building the Core Model requires various .dll dependencies.
To ensure the latest versions, these libraries are downloaded
just prior to build and placed in a new folder called, "Libs".

	a. run the following at the (ADM) command line prompt:
C:\Users\...\Core-Model\model>install-libs_CoreModel.cmd



####################################################################
Stage Two Rebuild -- Generate .sln and .csproj file with Premake
####################################################################

Useful Premake websites:
https://github.com/premake/premake-core/wiki
http://premake.github.io/


The Premake build configuration tool is needed to generate the C# solution file 
(.sln) and the project files (.csproj) for rebuilding Core Model. Premake will generate 
the requisite Visual Studio files (the .sln file the .csproj files) for a rebuild.
Premake itself is built on Lua, a fast, light-weight scripting language. Premake 
scripts are actually Lua programs (.lua files). Using Visual Studio 2015 or greater is
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
 
	

################################################################
Stage Three Rebuild -- Perform a Visual Studio (Release) build
#################################################################

NB. The "LANDIS-II.sln" file can be opened in a variety of IDE environments including,
Visual Studio (VS) and MonoDevelop. VS2015 is recommended.

NB. If the post-build goal is staging for Debug testing, see the file,
...\model\deploy\README_Debug-testing-the-build.txt PRIOR to building the .sln file in Debug mode.




	a. Open the LANDIS-II.sln file in VS 2015

	b. Use the pull down menu that currently shows, "Debug" and select, "Release"

	c. In the Solution Explorer tab, highlight " Solution "LANDIS-II" " and select the
	   Properties icon (the wrench)
	c1. Under Solution "LANDIS-II" Property Pages, select Configuration Properties ==> Configuration
	c2. Unselect Ecoregions_Tests and Species_Tests (ie, do not build these .csproj files); hit "Apply"
	    and then "OK"  

	d. Build the LANDIS-II.sln in Release mode (ie, NOT Debug mode)
	d1. Expected VS output:

========== Build: 5 succeeded, 0 failed, 0 up-to-date, 2 skipped ==========

	e. expected contents of C:\Users\...\...\model\build\Release 

Edu.Wisc.Forest.Flel.Util.dll
Landis.Console-6.1.exe
Landis.Console-6.1.pdb
Landis.Core.dll
Landis.Core.Implementation.dll
Landis.Core.Implementation.pdb
Landis.Core.pdb
Landis.Core.xml
Landis.Extensions.Dataset.dll
Landis.Extensions.Dataset.pdb
Landis.Extensions.exe
Landis.Extensions.pdb
1Landis.Landscapes.dll
Landis.RasterIO.dll
Landis.RasterIO.Gdal.dll
Landis.SpatialModeling.dll
log4net.dll
Troschuetz.Random.dll




.
############################################################
Stage Four Rebuild -- Staging for Installer Prep or Testing
############################################################

After a solution has been built, the newly-minted Landis.Core.dll (plus all of its attendant files) is 
called a "configuration". A configuration is then re-organized for the subsequent installer (Stage Five Rebuild below)
but also can be "staged" for test purposes.

	Option1: If the subsequent step is generating an installer, see Stage Five Rebuild -- Installer below. 
	Option2: If the subsequent step is Debug testing, see ...\model\deploy\README_Debug-testing-the-build.txt. 

NB. Running the following .lua script will install all of the ...\model\build\Release files (the configuration), 
plus some additional files, into the ...\build\install\Release\ directory.  The final ...\build\install\Release\ 
directory has the same directory structure as that produced by a LANDIS-II installation. 


	a. designate the LANDIS-II (ie, Core Model) release status. 
	a1. Enter the software's release status on the first line of the file called
	  "release-status.txt".  The format of the release status is one of these:

	    alpha release (#)
 	    beta release (#)
	    release candidate (#)
	    official release

	  where (#) is an integer >= 1.  Examples:
		"alpha release 3" 	OR  
		"beta release 2" 	OR 
		"release candidate 2" 	OR
		"official release"


	b. run the ...\model\deploy\premake5.lua script
	b1. at the (ADM) command line prompt:
C:\Users\...\Core-Model\model\deploy>premake5 install Release


	c. expected contents of C:\Users\...\...\model\build\install\Release

bin\
landis-6..cmd         landis-v6-extensions.cmd  uninstall-landis.sh
landis-extensions.cmd  uninstall-extensions.cmd
landis-ii.cmd          uninstall-landis.cmd

GDAL\
  \1.9

v6\
  \bin
  \licenses




############################################
Stage Five Rebuild -- Create the installer
############################################

The contents of C:\Users\...\...\model\build\install\Release is subsequently accessed
by the InnoSetup installer (LANDIS-II-6.2.1.iss) to create an .exe installer.

	a. open "...\model\deploy\installer\LANDIS-II-6.2.1.iss" in Inno Script Studio

	b. compile the LANDIS-II-6.2.1.iss script (Ctrl-F9)

	c. the expected output is a newly-minted installer found in the same directory as LANDIS-II.iss.
	c1. For the LANDIS-II (official release) 6.2.1, the installer is, 

LANDIS-II-6.2-setup64.exe











