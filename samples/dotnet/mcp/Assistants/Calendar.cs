namespace Assistants;

using System.ComponentModel;

using Microsoft.SemanticKernel;

public class Calendar
{
    [KernelFunction, Description("Gets the current date in yyyy-MM-dd format")]
    public string GetCurrentDate() => DateTime.Now.ToString("yyyy-MM-dd");
}
