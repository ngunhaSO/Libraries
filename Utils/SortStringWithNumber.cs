using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Common.Utils
{
    //=========================================================================================//
    //                                                                                         //
    //  Sort an collection of string that has mixing of text, special characters, and number   //
    //                                                                                         //
    //=========================================================================================//
    public class SortStringWithNumber
    {
        private Match match = null;
        public SortStringWithNumber(string raw)
        {
            match = Regex.Match(raw, @"^([A-Z]*)([^0-9]*)(\d+)(.*)$");
        }
        public string Prefix
        {
            get
            {
                return match.Groups[1].Value;
            }
        }

        public string Separator
        {
            get
            {
                return match.Groups[2].Value;
            }
        }

        public int Number
        {
            get
            {
                return int.Parse(match.Groups[3].Value);
            }
        }

        public string Suffix
        {
            get
            {
                return match.Groups[4].Value;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}/{1:00000}{2}", this.Prefix, this.Number, this.Suffix);
        }
    }
}
