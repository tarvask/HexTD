using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Tools
{
    public static class StringUtils
    {
        public static string ToString([CanBeNull]IEnumerable list, string delimiter = "\n")
        {
            if (list == null)
            {
                return "null";
            }
 
            var builder = new StringBuilder(500);
            builder.Append('{');

            foreach (var element in list)
            {
                Append(element, builder);
                builder.Append(delimiter);
            }
            
            if (builder.Length > 1)
                builder.Remove(builder.Length - delimiter.Length, delimiter.Length);
            
            builder.Append('}');
         
            return builder.ToString();
        }

        private static void Append(object target, StringBuilder toBuilder)
        {
            if(target == null)
            {
                toBuilder.Append("null");
            }
            else
            {
                toBuilder.Append("\"");
                toBuilder.Append(target);
                toBuilder.Append("\" (");
                toBuilder.Append(target.GetType().Name);
                toBuilder.Append(")");
            }
        }
    }
}