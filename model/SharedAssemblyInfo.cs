using System.Reflection;

[assembly: AssemblyProduct("LANDIS-II")]
[assembly: AssemblyVersion("6.2.1")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
