LANDIS-II Software Development Kit (SDK)

This SDK targets the version 6.X series of the LANDIS-II model.  It's designed
to work with version 6.0 release candidate 3 (rc3) and later versions where
the extension directory is:

  C:\Program Files\LANDIS-II\v6\bin\extensions


Configuration
-------------

To use this SDK, set the LANDIS_SDK environment variable to the SDK's top
folder.  For example, if you have release 17 of this SDK in this folder:

  C:\Users\jdoe\Documents\LANDIS-II\sdk\v6-r17

then set the environment variable to that path.


Staging Assemblies on Windows
-----------------------------

Since Windows Vista, the the C:\Program Files\ folder has been protected.
Changes to this protected folder require explicit permission from an
administrator.  With the recommended setting for User Account Control (UAC),
a developer cannot build a project's assembly directly in the LANDIS-II
folder structure, even if her user account is an administrator.  In other
words, she cannot set the project's Output Path to any subfolder inside the
C:\Program Files\ system folder (unless she disables UAC).

The SDK contains scripts to help stage a project's assembly file into the
C:\Program Files\LANDIS-II\ directory structure without disabling UAC.  These
scripts are located in the SDK's staging\ folder.  To enable them, run the
staging\create-task.cmd script once.  You must run it as an administrator
(i.e., right click the script, select "Run as administrator").  It creates a
scheduled task with elevated privileges, which can copy files into the
C:\Program Files\ protected folder. 

To use the staging scripts in a project, create a text file called
"staging-list.txt" with the following contents:

  {OutDir}\NAME.OF.PROJECT.ASSEMBLY.dll

For example, if the project's assembly name "Landis.Library.Example.dll", then
the staging-list.txt file would contain this line:

  {OutDir}\Landis.Library.Example.dll

This file can list multiple files to be staged.  Continuing the previous
example, the assembly's symbol and documentation files can also be specified
if the project's configured to generate them:

  {OutDir}\Landis.Library.Example.dll
  {OutDir}\Landis.Library.Example.pdb
  {OutDir}\Landis.Library.Example.xml

After creating this text file, edit the project's Post build event (in Visual
Studio: Project --> Properties --> Build Events), and enter this command line:

  "$(LANDIS_SDK)\staging\copy-to-build-dir.cmd" "$(ProjectDir)staging-list.txt" $(OutDir)  

This event will invokes the appropriate script to copy the project's assembly
into a specific folder designated for developer use:

  C:\Program Files\LANDIS-II\v6\bin\build

This folder keeps the assemblies that a developer is actively working on
separate from the installed LANDIS-II extensions, which are located in the
sibling folder:

  C:\Program Files\LANDIS-II\v6\bin\extensions

The LANDIS-II model is configured to look for extensions and libraries in both
folders (it looks in build\ first, then in extensions\).


Relocating or Updating the SDK
------------------------------

If you relocate the SDK to a different folder, or want to use a newer version
that you've installed in a different folder, then the LANDIS_SDK environment
variable will need to be modified to have the path to this new folder.

After the variable is updated on a Windows system, the scheduled task that's
associated with the staging scripts needs to also be updated.  This is done
with the staging\update-task.cmd script in the new SDK location.  This script
must be "Run as administrator".  Because it's modifying a task with elevated
privileges, it will prompt you for your password to confirm your access to
change the task.


Extension Installers with Inno Setup
------------------------------------

After you set the variable via the control panel, you'll need to restart
your Inno Setup IDE (e.g., Inno Setup Studio).


Packaging Libraries
-------------------

The SDK includes the NuGet command-line tool (version 2.8) for creating NuGet
packages for libraries.  It's located in the tools/ subfolder.