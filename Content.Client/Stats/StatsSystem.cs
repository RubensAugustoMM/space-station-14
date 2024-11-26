using System.Linq;
using Content.Client.CharacterInfo;
using Content.Client.Message;
using Content.Shared.Stats;
using Content.Shared.Stats.Components;
using Robust.Client.UserInterface.Controls;
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

   private void OnCharacterInfoEvent(EntityUid uid, StatsComponent component, ref CharacterInfoSystem.GetCharacterInfoControlsEvent args)
   {
       var allStats = new List<StatsPrototype>();

       foreach (var stat in component.Stats.Keys)
       {
           allStats.Add(_protoManager.Index<StatsPrototype>(stat));
       }

       var orderedStats = allStats.OrderBy(p => p.Order );

       foreach (var proto in orderedStats)
       {
           var box = new BoxContainer
           {
               Margin = new Thickness(5),
               Orientation = BoxContainer.LayoutOrientation.Vertical
           };

           var title = new RichTextLabel();
           title.SetMarkup(Loc.GetString("character-info-stats-name", ("stat", Loc.GetString(proto.Name))));
           var text = new RichTextLabel();
           text.SetMarkup(Loc.GetString("character-info-stats-level",
               ("amount", component.Stats[proto.ID]),
               ("max", proto.MaxValue)));

           box.AddChild(title);
           box.AddChild(text);

           args.Controls.Add(box);
       }
   }
}
