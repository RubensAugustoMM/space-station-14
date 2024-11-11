using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.PlayerStatus.UI;
using Content.Shared.Stats.Components;
using Robust.Client.Player;

namespace Content.Client.PlayerStatus.UI;

public sealed partial class PlayerStatusUISystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly MobThresholdSystem _mobThresholdSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlayerStatusUIComponent, DamageChangedEvent>(FireResetUIEvent);
        SubscribeLocalEvent<PlayerStatusUIComponent,StatChangedEvent>(RefFireResetUIEvent);

        SubscribeLocalEvent<DamageableComponent,GetStatusUIControlEvent>(AddHealthBar);
    }

    private void FireResetUIEvent<TEvent>(EntityUid uid,PlayerStatusUIComponent comp, TEvent args)
    {
        if (_playerManager.LocalSession == null ||
            _playerManager.LocalSession.AttachedEntity != uid)
            return;

        RaiseLocalEvent(new UpdateStatusUIEvent());
    }

    private void RefFireResetUIEvent<TEvent>(EntityUid uid, PlayerStatusUIComponent comp, ref TEvent args)
    {
        if (_playerManager.LocalSession == null ||
            _playerManager.LocalSession.AttachedEntity != uid)
            return;

        RaiseLocalEvent(new UpdateStatusUIEvent());
    }

    private void AddHealthBar(EntityUid uid, DamageableComponent comp, GetStatusUIControlsEvent args)
    {


    }
}
