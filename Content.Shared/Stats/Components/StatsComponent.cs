using JetBrains.Annotations;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Stats.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedStatsSystem)), AutoGenerateComponentState]
public sealed partial class StatsComponent : Component
{
    [AutoNetworkedField]
    [DataField("stats", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<int, StatsPrototype>))]
    public Dictionary<string, int> Stats = new();

    [AutoNetworkedField]
    [DataField("baseStats", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<int, StatsPrototype>))]
    public Dictionary<string, int> BaseStats = new();
}

[ByRefEvent]
public readonly record struct StatChangedEvent(EntityUid Entity, StatsPrototype Stat, int OldValue, int NewValue)
{
    /// <summary>
    /// The change in value.
    /// </summary>
    [PublicAPI]
    public int Delta => NewValue - OldValue;
}

[Serializable, NetSerializable]
public sealed class NetworkStatChangedEvent : EntityEventArgs
{
    public NetEntity Entity;

    public int OldValue;

    public int NewValue;

    public int Delta => NewValue - OldValue;

    public NetworkStatChangedEvent(NetEntity entity, int oldValue, int newValue)
    {
        Entity = entity;
        OldValue = oldValue;
        NewValue = newValue;
    }
}
