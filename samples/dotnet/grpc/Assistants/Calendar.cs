namespace Assistants;

using System.ComponentModel;

using Microsoft.SemanticKernel;

public class Calendar
{
    [KernelFunction, Description("Gets the current date in yyyy-MM-dd format")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Needs to be instance member for SK plugin creation")]
    public string GetCurrentDate() => DateTime.Now.ToString("yyyy-MM-dd");
}
