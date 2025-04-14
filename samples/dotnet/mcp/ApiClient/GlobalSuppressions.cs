// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Consumed by LLM function calls", Scope = "module")]
