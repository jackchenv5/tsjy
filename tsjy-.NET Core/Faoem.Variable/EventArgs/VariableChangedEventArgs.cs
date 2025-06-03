using Faoem.Variable.Definitions;

namespace Faoem.Variable.EventArgs;

public class VariableChangedEventArgs(List<AppVariable> variables)
{
    public List<AppVariable> Variables { get; set; } = variables;
}