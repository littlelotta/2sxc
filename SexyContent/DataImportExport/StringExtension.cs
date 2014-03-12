﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public static class StringExtension
    {
        public static string GetValueReferenceLanguage(this string valueString)
        {
            var match = Regex.Match(valueString, @"=ref\((?<language>.+),(?<readOnly>.+)\)");
            if (match.Success)
            {
                return match.Groups["language"].Value;
            }
            return null;
        }

        public static bool GetValueReadOnly(this string valueString)
        {
            var match = Regex.Match(valueString, @"=ref\((?<language>.+),(?<readOnly>.+)\)");
            if (match.Success)
            {
                return match.Groups["readOnly"].Value == "ro";
            }
            return true;
        }

        public static bool IsValueDefault(this string valueString)
        {
            return valueString == "=default()";
        } 
    }
}