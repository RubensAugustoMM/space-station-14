using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Stats.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class StatsComponent : Component
{
    [AutoNetworkedField]
    [DataField("stats", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<int, StatsPrototype>))]
    public Dictionary<string, int> Stats = new();

}
