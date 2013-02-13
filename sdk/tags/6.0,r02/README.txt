LANDIS-II Software Development Kit (SDK)

This SDK targets version 6.0 of the LANDIS-II model.  It's designed to work
with release candidate 3 (rc3) and later releases where the extension
directory is:

  C:\Program Files\LANDIS-II\v6\bin\extensions

To use this SDK with Inno Setup scripts for extension installers, set the
LANDIS_SDK environment variable to the SDK's top folder.  For example, if
you have release 17 of this SDK in this folder:

  C:\Users\jdoe\Documents\LANDIS-II\sdk\6.0,r17

then set the environment variable to that path.

After you set the variable via the control panel, you'll need to restart
your Inno Setup IDE (e.g., Inno Setup Studio).
