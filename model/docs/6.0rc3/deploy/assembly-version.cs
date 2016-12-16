using System;
using System.Reflection;

public class Script
{
    static public int Main(string[] args)
    {
        try {
            foreach (string path in args) {
                Assembly assembly = Assembly.ReflectionOnlyLoadFrom(path);
                Console.WriteLine(assembly.GetName().Version);            
            }
            return 0;
        }
        catch (Exception exc) {
            Console.Error.WriteLine("Error: {0}", exc.Message);
            return 1;
        }
    }
}
