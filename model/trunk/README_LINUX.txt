Although developed mainly on Windows, LANDIS-II can be deployed natively on Linux using the Mono environment. 
This file gives instructions to set-up and run LANDIS-II on Linux, along with a list of troubleshooting tips.

Section A (INSTALLATION) describes the Linux-specific procedure to build LANDIS-II.
Section B (STEP-BY-STEP TUTORIAL) provides a step-by-step example to set-up and run an extension.
Section C (KNOWN ERRORS AND SOLUTIONS) lists common error messages and their solutions.


A) INSTALLATION:
================

Compiling LANDIS-II requires a working mono environment with nunit as well as premake4. The installer also needs curl to download extra packages. On Ubuntu 16.04, the following command installs all the required dependencies:
```
$ sudo apt-get install mono-complete premake4 curl nunit
```

The first step of compilation is to run premake4. Go to the directory where the premake.lua is (it should be Core-Model/model/trunk/), and prepare the linux makefiles with:
```
$ premake4 gmake
```

Building LANDIS-II is done with:
```
$ make config=release
```
Alternatively, to build the debug version type "make config=debug" (or simply "make").


The installer might complains during the "make" step about the file "nunit.framework.dll" being not found. In this case, you can create a link to it with:
```
$ ln -s /usr/lib/cli/nunit.framework-2.6.3/nunit.framework.dll core/test/species 
$ ln -s /usr/lib/cli/nunit.framework-2.6.3/nunit.framework.dll core/test/ecoregions 
```
(This is the default location on Ubuntu 16.04 with nunit 2.6.3 - you can found the proper path on you system with `find /usr/lib/ -iname 'nunit.framework.dll'`)


B) STEP-BY-STEP TUTORIAL:
=========================

These instructions describe all the steps required to install a LANDIS-II extension (previously compiled on Windows) and run a scenario.

The example used in this tutorial is available as the "SpruceBudworm" branch of the Extensions-Succession github repository. This use-case contains the deployment file of the Biomass Succession extension, all the already compiled dll files, with a scenario to try it. You can download it at https://github.com/LANDIS-II-Foundation/Extensions-Succession/tree/master/biomass-succession/branches/SpruceBudworm .

Step 1) Adding the extension.
This is done with the Landis.Extensions.exe program, with the `add` directive. Type on a commandline:
```
  cd /path/to/the/extension/directory/deploy
  mono /path/to/mono/build/Debug/Landis.Extensions.exe add Biomass\ Succession.txt
```

Step 2) Copying the extension dll files.
To complete the installation of the extension we need to copy the required dll files. For this example, we need to copy six dlls to the `bin` directory:
```
user@machine:~/Extensions-Succession/biomass-succession/branches/SpruceBudworm/src/bin/Debug$ cp Landis.Extension.Succession.Biomass.dll Landis.Library.AgeOnlyCohorts.dll Landis.Library.BiomassCohorts.dll Landis.Library.Biomass.dll Landis.Library.Cohorts.dll Landis.Library.Succession.dll ~/Core-Model/model/trunk/build/Debug/
```

Step 3) Running the scenario.
Running the scenario should be done with the wrapper landis_console.sh (using the executable results in a GDAL-related error, see a list of common errors below). Don't forget to move to the scenario directory beforehand, and type:
```
user@machine:~/Extensions-Succession/biomass-succession/branches/SpruceBudworm/deploy/examples$ ~/Core-Model/model/trunk/build/Debug/landis_console.sh scenario.txt 
```


C) KNOWN ERRORS AND SOLUTIONS:
==============================

Several Linux-specific problems can arise when running LANDIS-II + extensions. We list them here along with instructions on how to fix them.

*) Error when forgetting to deploy the extension:
```
Error at line 15 of file "scenario.txt":
  Error reading input value for Extension:
    No extension with the name "Biomass Succession".
```

=> this is solved by installing the extension using Landis.Extensions.exe. Refer to Steps 1 and 2 of the above tutorial for instructions.


*) When forgetting to copy a .dll file compiled on Windows:
```
Internal error occurred within the program:
  Could not load file or assembly 'Landis.Library.BiomassCohorts, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies.

Stack trace:
  at Landis.Model.Run (System.String scenarioPath, IUserInterface ui) <0x40294480 + 0x00d91> in <filename unknown>:0 
  at Landis.App.Main (System.String[] args) <0x401c5d60 + 0x00607> in <filename unknown>:0 
```

=> this is solved by making sure that all the dlls are present. The error message gives away the missing dll, "Landis.Library.BiomassCohorts.dll" here.


*) When using the base Landis program instead of the wrapper:
```
Internal error occurred within the program:
  The type initializer for 'Landis.RasterIO.Gdal.RasterFactory' threw an exception.
  The type initializer for 'Landis.RasterIO.Gdal.GdalSystem' threw an exception.

Stack trace:
  at Landis.App.Main (System.String[] args) <0x41098d60 + 0x00533> in <filename unknown>:0 
```

=> this is solved by invoking landis_console.sh instead of mono Landis.Console.exe. Refer to Step 3 of the above tutorial for an example.

