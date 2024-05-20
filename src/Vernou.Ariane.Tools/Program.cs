using System.CommandLine;
using Vernou.Ariane.Tools.Commands;
using Vernou.Ariane.Tools.Core.Output;

namespace Vernou.Ariane.Tools;

public class Program
{
    private static Task<int> Main(string[] args)
    {
        return Run(
            args,
            new ConsoleOutput()
        );
    }

    public static async Task<int> Run(string[] args, IOutput output)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        var rootCommand = new RootCommand("Ariane to navigate through the dependency hell.");

        Argument<IEnumerable<string>> slnOrProjectArgument = new("PROJECT | SOLUTION") {
            Description = "The project or solution file to operate on. If a file is not specified, the command will search the current directory for one.",
            Arity = ArgumentArity.ZeroOrMore
        };

        var graphCommand = new Command("graph", "Print dependencies");
        rootCommand.AddCommand(graphCommand);
        rootCommand.Add(slnOrProjectArgument);
        graphCommand.SetHandler(async (slnOrProjectArgumentValue) =>
        {
            var p = slnOrProjectArgumentValue.FirstOrDefault() ?? "";
            await new GraphCommand(p, output).RunAsync();
        }, slnOrProjectArgument);

        var auditCommand = new Command("audit", "Audit dependencies");
        rootCommand.AddCommand(auditCommand);
        auditCommand.SetHandler(async (slnOrProjectArgumentValue) =>
        {
            var p = slnOrProjectArgumentValue.FirstOrDefault() ?? "";
            await new AuditCommand(p, output).RunAsync();
        }, slnOrProjectArgument);

        //return await rootCommand.InvokeAsync(args);

        // graph
        //return await rootCommand.InvokeAsync(["graph"]);
        //return await rootCommand.InvokeAsync(["graph", "-h"]);
        //return await rootCommand.InvokeAsync(["""C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""", "graph", "-h"]);
        return await rootCommand.InvokeAsync(["""C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""", "graph"]);

        // audti
        //return await rootCommand.InvokeAsync(["audit"]);
        //return await rootCommand.InvokeAsync(["""C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""", "audit", "-h"]);
        return await rootCommand.InvokeAsync(["""C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""", "audit"]);

        //var projectName = """HowFix.Demo""";
        //var assetsPath = """C:\t\HowFix.Demo\HowFix.Demo\obj\project.assets.json""";
        //var assetsPath = """C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""";
    }
}
