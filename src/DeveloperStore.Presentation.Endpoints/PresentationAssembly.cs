using System.Reflection;

namespace DeveloperStore.Presentation.Endpoints;

public static class PresentationAssembly
{
    public static Assembly Get() => Assembly.GetAssembly(typeof(PresentationAssembly))!;
}