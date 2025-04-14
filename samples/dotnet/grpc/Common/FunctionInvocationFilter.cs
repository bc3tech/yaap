namespace Common;

using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
public class FunctionInvocationFilter(Func<FunctionInvocationContext, Task>? onStart = null, Func<FunctionInvocationContext, Task>? onEnd = null, Func<AutoFunctionInvocationContext, Task>? onAutoStart = null, Func<AutoFunctionInvocationContext, Task>? onAutoEnd = null) : IFunctionInvocationFilter, IAutoFunctionInvocationFilter
{

    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        await (onAutoStart?.Invoke(context /*($"Running {context.Function.Name} ({context.Function.Description}) ...", null)*/) ?? Task.CompletedTask);

        await next(context);

        await (onAutoEnd?.Invoke(context /*($"{context.Function.Name} completed.", _includeResult ? $@" Result: {context.Result}" : null)*/) ?? Task.CompletedTask);
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        await (onStart?.Invoke(context /*($"Running {context.Function.Name} ({context.Function.Description}) ...", null)*/) ?? Task.CompletedTask);

        await next(context);

        await (onEnd?.Invoke(context /*($"{context.Function.Name} completed.", _includeResult ? $@" Result: {context.Result}" : null)*/) ?? Task.CompletedTask);
    }
}
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.