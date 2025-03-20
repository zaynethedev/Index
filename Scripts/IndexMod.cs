using System;
namespace Index.Resources
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IndexMod : Attribute
    {
        public string ModName { get; }
        public string ModDescription { get; }
        public string ModGUID { get; }
        public int ModID { get; }

        public IndexMod(string modName, string modDescription, string modGUID, int modID)
        {
            ModName = modName;
            ModDescription = modDescription;
            ModGUID = modGUID;
            ModID = modID;
        }
    }
}