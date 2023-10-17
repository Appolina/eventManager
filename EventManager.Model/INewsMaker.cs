namespace EventManager.Model
{
    public delegate void NewsHappen(INewsMaker sender, News news);

    public interface INewsMaker
    {
        event NewsHappen OnNewsHappen;
    }
}