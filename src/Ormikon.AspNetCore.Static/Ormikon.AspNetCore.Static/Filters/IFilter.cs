namespace Ormikon.AspNetCore.Static.Filters
{
    internal interface IFilter
    {
        bool Contains(string test);

        bool IsActive();
    }
}

