namespace Vernou.Ariane.Tools;

public interface IProjectResolver
{
    Models.Project Resolve(string projectPath);
}
