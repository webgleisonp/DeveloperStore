using System.Reflection;

namespace DeveloperStore.Application;

public static class ApplicationAssembly
{
    public static Assembly Get() => Assembly.GetAssembly(typeof(ApplicationAssembly))!;
}