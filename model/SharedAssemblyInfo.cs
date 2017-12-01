using System.Reflection;

[assembly: AssemblyProduct("LANDIS-II")]
[assembly: AssemblyVersion("7.0.*")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
