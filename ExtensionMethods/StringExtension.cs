using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    //===============================================================================//
    //                                                                               //
    //     Desc: collection of extension methods for string                          //                                 
    //                                                                               //
    //===============================================================================//
    public static class StringExtension
    {
        //usage: var s = "The co-ordinate is ({0}, {1})".StrFmt(point.X, point.Y);
        public static string StrFmt(this string s, params object[] args)
        {
            return string.Format(s, args);
        }


        public static bool IsLike(this string s, string wildcardPattern)
        {
            if (s == null || String.IsNullOrEmpty(wildcardPattern))
                return false;
            var regexPattern = "^" + Regex.Escape(wildcardPattern) + "$";

            regexPattern = regexPattern.Replace(@"\[!", "[^")
                                        .Replace(@"\[", "[")
                                        .Replace(@"\]", "]")
                                        .Replace(@"\?", ".")
                                        .Replace(@"\*", ".*")
                                        .Replace(@"\#", @"\d");
            var result = false;
            try
            {
                result = Regex.IsMatch(s, regexPattern);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid pattern: {0}".StrFmt(wildcardPattern));
            }
            return result;
        }
    }
}
