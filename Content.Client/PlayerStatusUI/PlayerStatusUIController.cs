using System.Linq;
using Content.Client.Gameplay;
using Content.Client.Message;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.PlayerStatus.UI;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Player;

namespace Content.Client.PlayerStatus.UI;

public sealed partial class PlayerStatusUIController : UIController, IOnStateEntered<GameplayState>
{
    private PlayerStatusUI? Gui => UIManager.GetActiveUIWidgetOrNull<PlayerStatusUI>();

    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UpdateStatusUIEvent>(UpdateEvent);
        SubscribeLocalEvent<PlayerAttachedEvent>(PlayerAttach);
        SubscribeLocalEvent<PlayerDetachedEvent>(PlayerDetach);

        var gameStateLoad = UIManager.GetUIController<GameplayStateLoadController>();
        gameStateLoad.OnScreenLoad += OnScreenLoad;
    }

    private void OnScreenLoad()
    {
        if (_playerManager.LocalSession == null)
            return;

        UpdateUI(_playerManager.LocalSession.AttachedEntity);
    }

    private void PlayerDetach(PlayerDetachedEvent ev)
    {
        UpdateUI(ev.Entity);
    }

    private void PlayerAttach(PlayerAttachedEvent ev)
    {
        UpdateUI(ev.Entity);
    }

    private void UpdateEvent(UpdateStatusUIEvent ev)
    {
        if (_playerManager.LocalSession == null)
            return;

        UpdateUI(_playerManager.LocalSession.AttachedEntity);
    }

    public void OnStateEntered(GameplayState state)
    {
        if (_playerManager.LocalSession == null)
            return;

        UpdateUI(_playerManager.LocalSession.AttachedEntity);
    }

    private void UpdateUI(EntityUid? uid)
    {
        if (Gui == null || uid == null)
            return;

        UpdateUI(uid.Value);
    }

    private void UpdateUI(EntityUid uid)
    {
        if (Gui == null)
            return;

        if (!_entityManager.HasComponent<PlayerStatusUIComponent>(uid))
        {
            Gui.Visible = false;
            return;
        }

        Gui.ResetUI();
        Gui.Visible = true;
        Gui.EntNameText.SetMarkup(Loc.GetString("status-ui-entity-name", ("entity", uid)));

        var controlsEv = new GetStatusUIControlsEvent(new());
        _entityManager.EventBus.RaiseLocalEvent(uid,controlsEv);

        var orderedControls = controlsEv.Controls.OrderBy(i => i.order);

        foreach (var (_,control) in orderedControls)
        {
            Gui.ControlContainer.AddChild(control);
        }
    }
}
