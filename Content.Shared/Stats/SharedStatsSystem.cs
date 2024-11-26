using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Stats.Components;
using JetBrains.Annotations;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Stats;

public abstract partial class SharedStatsSystem : EntitySystem
{
    [Dependency] protected readonly IRobustRandom RobustRandom = default!;
    [Dependency] protected readonly MobThresholdSystem MobThresholdSystem = default!;
    [Dependency] protected readonly SharedPopupSystem PopupSystem = default!;
    [Dependency] protected readonly INetManager NetManager = default!;
    [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;

    private ISawmill _sawmill = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<StatsComponent, ComponentInit>(OnCompInit);

        _sawmill = Logger.GetSawmill("stat");
    }

    private void OnCompInit(EntityUid uid, StatsComponent component, ComponentInit args)
    {
        foreach (var (key, val) in component.BaseStats)
        {
            component.Stats[key] = default;
            SetStatValue(uid, key,val,component);
        }
        Dirty(component);
    }

    #region Stat Setters
    [PublicAPI]
    public void ModifyStatValue(EntityUid uid, string stat, int delta, StatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;
        if (!component.Stats.TryGetValue(stat, out var val))
            return;

        SetStatValue(uid, stat, val + delta, component);
    }

    [PublicAPI]
    public void ModifyStatValue(EntityUid uid, StatsPrototype stat, int delta, StatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;
        if (!component.Stats.TryGetValue(stat.ID, out var val))
            return;

        SetStatValue(uid, stat,val + delta, component);
    }
    [PublicAPI]
    public void SetStatValue(EntityUid uid, string stat, int value, StatsComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        if (!PrototypeManager.TryIndex<StatsPrototype>(stat, out var statsPrototype))
        {
            _sawmill.Error($"Invalid sat prototype ID: \"{stat}\"");
            return;
        }

        _sawmill.Info($"Stat {stat} define with sucess!");
        SetStatValue(uid, statsPrototype, value, component);
    }

    [PublicAPI]
    public void SetStatValue(EntityUid uid, StatsPrototype stat, int value, StatsComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        if (!component.Stats.ContainsKey(stat.ID))
            return;

        var clampedVal = Math.Clamp(value, stat.MinValue, stat.MaxValue);
        var statsChangedEvent = new StatChangedEvent(uid, stat, component.Stats[stat.ID], clampedVal);
        var netStatsChagedEvent = new NetworkStatChangedEvent(GetNetEntity(uid), component.Stats[stat.ID], clampedVal);
        component.Stats[stat.ID] = clampedVal;
        Dirty(uid, component);
        RaiseLocalEvent(uid, ref statsChangedEvent, true);
        RaiseNetworkEvent(netStatsChagedEvent, uid);
    }
    #endregion

    #region Stat Getters

    [PublicAPI]
    public int GetStat(EntityUid uid, StatsPrototype stat, StatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return 0;

        return GetStat(uid, stat.ID, component);
    }

    public int GetStat(EntityUid uid, string stat, StatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return 0;
        if (!component.Stats.ContainsKey(stat))
            return 0;

        return component.Stats.GetValueOrDefault(stat);
    }
    #endregion
}
