LANDIS-II Software Development Kit (SDK)

This SDK targets version 6.0 of the LANDIS-II model.  It's designed to work
with releases up to release candidate 2 (rc2) where the build directory is:

  C:\Program Files\LANDIS-II\6.0\bin


To obtain this SDK, export or checkout a read-only working copy of the
first release (r01) of version 6.0:

  {svn-repository}/sdk/tags/6.0,r01

To use this SDK with Inno Scripts for extension installers, set the
LANDIS_DEPLOY environment variable to this project's packaging folder.  For
example:

  LANDIS_DEPLOY=C:\Users\jdoe\Documents\LANDIS-II\sdk\6.0,r01\packaging

After you set the variable via the control panel, you'll need to restart
your Inno Setup IDE (e.g., Inno Setup Studio).
