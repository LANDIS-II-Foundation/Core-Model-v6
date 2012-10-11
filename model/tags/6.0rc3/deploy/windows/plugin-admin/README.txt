The current Windows installers (a.k.a. setup programs) for LANDIS-II
extensions are hard-wired to expect the extension administration tool
at:

  C:\Program Files\LANDIS-II\6.0\bin\Landis.PlugIns.Admin.exe

This hard-wiring requires that every installer to be updated when a
new core version is released (e.g., 6.1).  For greater flexibility
and less burden for developers and users, the installers should run
the "landis-extensions.cmd" script.  Minimally, they should run the
extensions script for the core major version that the extension is
targeting.  For example, if it targets LANDIS-II version 6, then the
extension installer could run "landis-v6-extensions.cmd".

As an interim solution until the installers are fixed (updated), this
folder contains a transitional tool.  This tool has two parts: 1) an
executable program that is installed at the hard-wired path above,
and 2) a command script called by the program.  The script calls the
current extension administration tool, and moves files & folders from
LANDIS-II\6.0\ into their counterparts in LANDIS-II\v6\.

To build the executable, run MSBuild in this folder:

  C:\WINDOWS\Microsoft.NET\Framework\v3.5\MSBuild.exe
