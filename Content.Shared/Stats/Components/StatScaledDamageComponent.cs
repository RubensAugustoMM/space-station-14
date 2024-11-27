using Content.Shared.Damage;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Stats.Components;

[RegisterComponent,Access(typeof(SharedStatsSystem))]
public sealed partial class StatScaledDamageComponent:Component
{
    [DataField("modifiers",required:true)]
    public DamageModifierSet Modifiers = default!;

    [DataField("baseMultiplier")]
    public float BaseMultiplier = 1f;

    [DataField("valueAdded")]
    public float ValueAdded = 0.1f;

    [DataField("scalingStat", customTypeSerializer: typeof(PrototypeIdSerializer<StatsPrototype>))]
    public string ScalingStat = "strength";
}
