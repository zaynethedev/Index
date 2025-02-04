using System;

namespace Index.Scripts
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IndexPlugin : Attribute
    {
        public string name { get; }
        public string description { get; }
        public string guid { get; }

        public IndexPlugin(string Name, string Description, string GUID)
        {
            name = Name;
            description = Description;
            guid = GUID;
        }
    }
}
