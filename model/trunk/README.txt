The Premake build configuration tool is needed to generate the C# solution
and project files.  Premake is available from:

  http://industriousone.com/premake

For convenience, it's recommended that you install Premake in a folder that
is in your PATH (for example, "My Documents\bin\" on Windows XP).  So, you
can run the tool in the command prompt by simply entering its name.

To allow multiple versions of Premake to be installed side-by-side, it's
recommended that you rename each executable to include its version number.
For example, on Windows,

  premake4.3.exe  -- Premake 4.3, current stable version
  premake4.4.exe  -- Premake 4.4-beta 4, current development version

Premake 4.3 has been used successfully to generate VS2008 project files, by
running this command in the folder with this README.txt file.

  premake4.3 vs2008

In order to generate VS2010 project files, Premake 4.4 (currently, beta 4)
is needed because Premake 4.3 doesn't support VS2010 C# projects.

  premake4.4 vs2010

If you going to Visual Studio/C# Express to build, you'll need to install
the NUnit testing framework (http://www.nunit.org).  The unit tests require
that framework.  Note that NUnit is included with SharpDevelop and
MonoDevelop.

After running Premake, open the LANDIS-II.sln solution file in an IDE
(Visual Studio, Visual C# Express, SharpDevelop, MonoDevelop) and build the
solution.

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


Staging a Configuration For Testing
-----------------------------------

After a configuration has been built, it can be "staged" for test purposes.
When staged, a configuration's files are copied into the directory structure
used for an LANDIS-II installation.  To stage a configuration, run Premake
in the deploy/ subdirectory.  For example, 

  cd deploy
  premake4.4 install Debug

This will install the Debug configuration's files along with other files into
the build/install/Debug/ directory.  This configuration of LANDIS-II can be
run by using the scripts in the installation's bin/ directory:

  cd some/path/to/test/scenarios
  path_to_model_project/build/install/Debug/bin/landis-ii scenario_1.txt

