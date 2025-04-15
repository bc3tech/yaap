namespace Common;
using System;
using System.Diagnostics;

using Microsoft.SemanticKernel;

public record AgentDefinition(string Name, string Description, Uri Endpoint)
{
    [Conditional("DEBUG")]
    public static void OutputRegisteredSkFunctions(Kernel? sk, TextWriter? target = null)
    {
        if (sk?.Plugins is not null)
        {
            foreach (KernelFunctionMetadata f in sk.Plugins.SelectMany(i => i.GetFunctionsMetadata()).Where(i => i is not null).Select(i => i!))
            {
                var output = $@"
// Description: {f.Description}{(f.ReturnParameter is not null ? $"\n// returns: {f.ReturnParameter.Description}" : string.Empty)}
{f.Name}({(f.Parameters.Any() ? "\n" : string.Empty)}{string.Join(",\n", f.Parameters.Where(i => !string.IsNullOrEmpty(i.Description)).Select(p => $"    // {p.Description}\n    {(p.IsRequired ? string.Empty : "(optional) ")}{p.Name}"))})";

                if (target is not null)
                {
                    target.WriteLine(output);
                }
                else
                {
                    Debug.WriteLine(output);
                }
            }
        }
    }
}
