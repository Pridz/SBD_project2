using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile
{
    class Record
    {
        char[] name { get; set; } 
        char[] surname { get; set; }
        ulong PESEL { get; set; }
        uint key { get; set; }
        ulong position { get; set; }

        public Record()
        {
            name = null;
            surname = null;
            PESEL = 0;
            key = 0;
            position = 0;
        }

        bool isNull()
        {
            bool state = true;
            if (name != null)
            {
                return false;
            }
            if (surname != null)
            {
                return false;
            }
            if (PESEL != 0)
            {
                return false;
            }
            if (key != 0)
            {
                return false;
            }
            if (position != 0)
            {
                return false;
            }
            return state;
        }

        uint calculateKeyValue()
        {
            return KeyValueCalculator.calculateNameValue(name) +
                    KeyValueCalculator.calculateSurnameValue(surname) +
                    KeyValueCalculator.calculatePeselValue(PESEL);
        }
    }
}
