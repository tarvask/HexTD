using System;

namespace Plugins.OdinExtensions.Editor.ListAsString
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ListAsStringAttribute : Attribute
    {
        public string StringAction;

        public ListAsStringAttribute(string action = null)
        {
            StringAction = action;
        }
    }
}