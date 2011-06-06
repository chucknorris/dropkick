namespace dropkick
{
    public interface Log
    {
        void Note(string format, object[] args);
    }
}