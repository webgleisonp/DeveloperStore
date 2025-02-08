using System.Reflection;

namespace DeveloperStore.Infra.Security;

public static class SecurityAssembly
{
    public static Assembly Assembly => typeof(SecurityAssembly).Assembly;
}