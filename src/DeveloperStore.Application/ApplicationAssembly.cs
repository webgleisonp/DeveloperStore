using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DeveloperStore.Application.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace DeveloperStore.Application;

public static class ApplicationAssembly
{
    public static Assembly Get() => typeof(ApplicationAssembly).Assembly;
}