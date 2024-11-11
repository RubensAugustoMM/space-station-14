using Content.Client.Gameplay;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client.PlayerStatus.UI;

public sealed partial class PlayerStatusUIController : UIController, IOnStateEntered<GameplayState>
{
    private PlayerStatusUI? Gui => UIManager.GetActiveUIWidgetOrNull<PlayerStatusUI>();

    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ACUpdate>();
    }

}
