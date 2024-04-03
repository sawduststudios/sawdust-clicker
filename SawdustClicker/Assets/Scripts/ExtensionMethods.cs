using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static string ToFormattedStr(this double number)
    {
        string[] suffixes = new string[] { "", "k", "M", "B", "T", "Q", "QQ", "S", "SS", "O", "N", "D", "UN", "DD", "TR", "QT", "QN", "SD", "SPD", "OD", "ND", "VG" };
        int suffixIndex = 0;
        while (number >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            number /= 1000;
            suffixIndex++;

            if (suffixIndex >= suffixes.Length - 1 && number >= 1000)
            {
                break;
            }
        }

        if (suffixIndex == 0)
            return number.ToString("F0");

        return number.ToString("F1") + suffixes[suffixIndex];
    }
}
