using System.Linq;
using Content.Client.CharacterInfo;
using Content.Shared.Stats;
using Content.Shared.Stats.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Stats;

public sealed class StatsSystem : SharedStatsSystem
{
   [Dependency] private readonly IPrototypeManager _protoManager = default!;
   [Dependency] private readonly CharacterInfoSystem _characterInfoSystem = default!;

   public event Action<EntityUid>? OnStatsChanged;

   public override void Initialize()
   {
       base.Initialize();

       SubscribeLocalEvent<NetworkStatChangedEvent>(OnNetworkStatChanged);
       SubscribeLocalEvent<StatsComponent, CharacterInfoSystem.GetCharacterInfoControlsEvent>(OnCharacterInfoEvent);
   }

   private void OnNetworkStatChanged(NetworkStatChangedEvent msg, EntitySessionEventArgs args)
   {
       _characterInfoSystem.RequestCharacterInfo();
   }

   private void OnCharacterInfoEvent(EntityUid uid,
       StatsComponent component,
       ref CharacterInfoSystem.GetCharacterInfoControlsEvent args)
   {
       var allStats = new List<StatsPrototype>();

       foreach (var stat in component.Stats.Keys)
       {
           allStats.Add(_protoManager.Index<StatsPrototype>(stat));
       }

       var orderedStats = allStats.OrderBy(p => p.Order );
       //fiquei por aqui
   }
}
