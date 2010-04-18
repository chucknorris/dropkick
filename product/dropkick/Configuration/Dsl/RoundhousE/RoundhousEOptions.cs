namespace dropkick.Configuration.Dsl.RoundhousE
{
    public interface RoundhousEOptions
    {
        RoundhousEOptions Environment(string name);
        RoundhousEOptions OnInstance(string name);
        RoundhousEOptions OnDatabase(string name);
        RoundhousEOptions UseMsSqlServer2005();
        RoundhousEOptions WithRecoveryMode(string type);

    }
}