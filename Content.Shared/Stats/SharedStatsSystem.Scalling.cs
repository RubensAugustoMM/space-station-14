using Content.Shared.Stats.Components;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared.Stats;

public abstract partial class SharedStatsSystem
{
    [Dependency] private readonly StatsMath _statsMath = default!;

    public void InitializeScalling()
    {
        SubscribeLocalEvent<StatScaledDamageComponent,MeleeHitEvent>(IncreaseMeleeDamage);
    }

    private void IncreaseMeleeDamage(Entity<StatScaledDamageComponent> ent, ref MeleeHitEvent args)
    {
        if (!TryComp<StatsComponent>(args.User, out var statsComponent)
            || !statsComponent.Stats.ContainsKey(ent.Comp.ScalingStat))
            return;

        var statValue = GetStat(args.User, ent.Comp.ScalingStat, statsComponent);

        args.ModifiersList.Add(_statsMath.MultiplyDamageModifier(ent.Comp.Modifiers, ent.Comp.BaseMultiplier + ent.Comp.ValueAdded*statValue));
    }
}
