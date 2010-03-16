namespace dropkick.Configuration.Dsl
{
    public interface DeploymentInspector
    {
        void Inspect(object obj);
        void Inspect(object obj, ExposeMoreInspectionSites additionalInspections);
    }

    public delegate void ExposeMoreInspectionSites();
}