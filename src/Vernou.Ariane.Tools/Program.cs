using System.CommandLine;
using System.IO;
using Vernou.Ariane.Tools.Commands;

namespace Vernou.Ariane.Tools;

public static class Program
{
    static async Task<int> Main(string[] args)
    {
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
            await new GraphCommand().RunAsync();
        }, slnOrProjectArgument);

        var auditCommand = new Command("audit", "Audit dependencies");
        rootCommand.AddCommand(auditCommand);
        auditCommand.SetHandler(async () =>
        {
            await new AuditCommand().RunAsync();
        });

        //return await rootCommand.InvokeAsync(args);
        //return await rootCommand.InvokeAsync(["graph"]);
        //return await rootCommand.InvokeAsync(["audit"]);
        //return await rootCommand.InvokeAsync(["graph", "-h"]);
        return await rootCommand.InvokeAsync(["""C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""", "graph"]);
        //return await rootCommand.InvokeAsync(["""C:\repos\efcore\artifacts\obj\EFCore.SqlServer\project.assets.json""", "graph", "-h"]);
    }
}
