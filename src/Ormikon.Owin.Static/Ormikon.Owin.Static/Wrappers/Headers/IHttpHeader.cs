namespace Ormikon.Owin.Static.Wrappers.Headers
{
    internal interface IHttpHeader
    {
        void Clear();

        bool Available { get; }

        string[] Values { get; }
    }
}

