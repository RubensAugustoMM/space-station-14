using Robust.Shared.Prototypes;

namespace Content.Shared.Stats;

[Prototype("stat")]
public sealed class StatsPrototype : IPrototype
{
    [IdDataField] public string ID { get; } = default!;
    [DataField("name")] public string Name = String.Empty;
    [DataField("minValue")] public int MinValue = 0;
    [DataField("maxValue")] public int MaxValue = 99;
}
