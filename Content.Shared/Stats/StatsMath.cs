using Content.Shared.Damage;

namespace Content.Shared.Stats;

public sealed partial class StatsMath : EntitySystem
{
    public DamageModifierSet MultiplyDamageModifier(DamageModifierSet damageModifierSet, float multiplier)
    {
        DamageModifierSet newModifier = new();

        foreach (var coefficient in damageModifierSet.Coefficients)
        {
            newModifier.Coefficients[coefficient.Key] = coefficient.Value * multiplier;
        }

        foreach (var flatReduction in damageModifierSet.FlatReduction)
        {
            newModifier.FlatReduction[flatReduction.Key] = flatReduction.Value * multiplier;
        }

        return newModifier;
    }
}
