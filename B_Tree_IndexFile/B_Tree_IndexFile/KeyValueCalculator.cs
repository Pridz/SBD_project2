using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile
{
    static class KeyValueCalculator
    {
        static public ushort nameDivider = 20;
        static public ushort surnameDivider = 30;
        static public ushort peselDivider= 40;

        static public uint sumStringValues(char[] letters)
        {
            uint sum = 0;
            foreach (char let in letters)
            {
                sum += (uint)Convert.ToInt32(let);
            }
            return sum;
        }

        static public uint calculateNameValue(char[] name)
        {           
            return sumStringValues(name) / nameDivider;
        }

        static public uint calculateSurnameValue(char[] surname)
        {
            return sumStringValues(surname) / surnameDivider;
        }
        static public uint calculatePeselValue(ulong pesel)
        {
            uint values = (uint)pesel / 100000;
            values = values % 10000;
            return values / peselDivider;
        }
    }
}
