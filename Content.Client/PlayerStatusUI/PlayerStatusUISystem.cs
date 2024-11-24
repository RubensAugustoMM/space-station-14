using System.Numerics;
using Content.Client.Message;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.PlayerStatus.UI;
using Content.Shared.Stats.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

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

        SubscribeLocalEvent<DamageableComponent,GetStatusUIControlsEvent>(AddHealthBar);
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
        if (_playerManager.LocalSession == null ||
            _playerManager.LocalSession.AttachedEntity != uid)
            return;

        if (!_entityManager.TryGetComponent<DamageableComponent>(uid, out var damageComp) ||
            !_mobThresholdSystem.TryGetDeadThreshold(uid, out var maxHP))
            return;

        var hp = (float)maxHP - (float)damageComp.Damage.GetTotal();

        var bar = new ProgressBar
        {
            ForegroundStyleBoxOverride = new StyleBoxFlat{BackgroundColor = Color.Red},
            MaxValue = (float) maxHP,
            Value = hp,
            MaxSize = new Vector2(250,12),
            SetSize = new Vector2(250,12)
        };

        bar.MaxValue = (float)maxHP;

        var label = new RichTextLabel
        {
            HorizontalAlignment = Control.HAlignment.Center,
            VerticalAlignment = Control.VAlignment.Top,
            Margin = new Thickness(-8f)
        };

        label.SetMarkup(
            Loc.GetString("status-ui-health-value",
                ("value", Math.Ceiling(hp)),
                ("maxValue", Math.Ceiling((float)maxHP))));

        bar.AddChild(label);

        args.Controls.Add((0, bar));
    }
}

public sealed partial class UpdateStatusUIEvent :EntityEventArgs{}

public sealed partial class GetStatusUIControlsEvent : EntityEventArgs
{
    public List<(int order, Control control)> Controls;

    public GetStatusUIControlsEvent(List<(int order, Control control)> controls)
    {
        Controls = controls;
    }
}
