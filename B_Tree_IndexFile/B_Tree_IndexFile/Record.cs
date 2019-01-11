using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile
{
    public class Record
    {
        char[] name;
        char[] surname;
        ulong PESEL;
        uint key;
        ulong position;
        public const uint BYTE_SIZE = 63;

        public char[] Name
        {
            get { return name; }
            set { name = value; }
        }

        public char[] Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        public ulong Pesel
        {
            get { return PESEL; }
            set { PESEL = value; }
        }

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
