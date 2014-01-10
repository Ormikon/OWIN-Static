namespace Ormikon.Owin.Static.Filters
{
    internal interface IFilter
    {
        bool Contains(string test);

        bool IsActive();
    }
}

