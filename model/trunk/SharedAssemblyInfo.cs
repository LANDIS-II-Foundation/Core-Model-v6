using System.Reflection;

[assembly: AssemblyProduct("LANDIS-II")]
[assembly: AssemblyVersion("6.2.*")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
