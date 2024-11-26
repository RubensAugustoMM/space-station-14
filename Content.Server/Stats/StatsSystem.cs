using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Stats;
using Content.Shared.Stats.Components;
using Robust.Shared.Console;

namespace Content.Server.Stats;

public sealed class StatsSystem : SharedStatsSystem
{
    [Dependency] private IConsoleHost _consoleHost = default!;
    public override void Initialize()
    {
        base.Initialize();

        _consoleHost.RegisterCommand("setstat",Loc.GetString("stat-command-set-stat"), "setstat <uid> <stat ID> <value>",
            SetStatCommand,
            SetStatCommandCompletions);
    }

    [AdminCommand(AdminFlags.Fun)]
    private CompletionResult SetStatCommandCompletions(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = new List<CompletionOption>();
            var query = EntityQueryEnumerator<StatsComponent>();
            while (query.MoveNext(out var ent,out _))
            {
                options.Add(new CompletionOption(ent.ToString(), ToPrettyString(ent)));
            }

            return CompletionResult.FromHintOptions(options,"<uid>");
        }

        if (args.Length == 2 && EntityUid.TryParse(args[0], out var uid) && TryComp<StatsComponent>(uid, out var stats))
        {
            return CompletionResult.FromHintOptions(stats.Stats.Keys, "<stat ID>");
        }

        return CompletionResult.Empty;
    }

    private void SetStatCommand(IConsoleShell shell, string argstr, string[] args)
    {
        if(args.Length != 3)
            shell.WriteError("Argument lengh must be 3");

        if (!EntityUid.TryParse(args[0], out var uid) || TryComp<StatsComponent>(uid, out var stat))
            return;

        if (!PrototypeManager.TryIndex<StatsPrototype>(args[1], out var statsPrototype))
            return;

        if (!int.TryParse(args[2], out var value))
            return;

        SetStatValue(uid, statsPrototype,value,stat);
    }
}
