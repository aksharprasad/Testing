using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProseTutorial
{
    public static class Semantics
    {
        public static uint? Add(Dictionary<uint?, uint?> v, uint? op1, uint? op2)
        {
            return op1 + op2;
        }

        public static uint? Element(Dictionary<uint?, uint?> v, int k)
        {
            if (k >= v.Count)
                return null;
            return v.Keys.ElementAt(k);
        }

        public static uint? Multiply(Dictionary<uint?, uint?> v, uint? op1, uint? op2)
        {
            return op1 * op2;
        }
    }
}