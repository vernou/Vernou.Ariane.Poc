namespace Vernou.Ariane.Tools.Models;

internal class Project : PackageDependency
{
    public IEnumerable<Project> ProjectReferences => _projectReferences;
    private List<Project> _projectReferences = [];

    public void AddProjectReference(Project project)
    {
        _projectReferences.Add(project);
    }
}
