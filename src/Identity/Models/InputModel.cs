using Ory.Kratos.Client.Model;

namespace Identity.Models;

public sealed class InputModel
{
    public KratosUiNodeInputAttributes Attributes { get; set; }
    public string Label { get; set; }

    public InputModel(KratosUiNodeInputAttributes attributes, string label)
    {
        Attributes = attributes;
        Label = label;
    }
}