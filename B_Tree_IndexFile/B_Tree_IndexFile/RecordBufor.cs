using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile
{
    public class RecordBufor
    {
        byte[] bufor = new byte[Record.BYTE_SIZE];
        int index;

        public int Index
        {
            get { return index; }
        }

        public byte this[int i]
        {
            get { return bufor[i]; }
            set
            {
                if (index >= Record.BYTE_SIZE)
                {
                    System.Console.Out.WriteLine("RecordBufor: writting to out of memory area!");
                }
                else
                {
                    bufor[i] = value;
                    index++;
                }
            }
        }

        public RecordBufor()
        {
            index = 0;
        }

        public void nullify()
        {
            index = 0;
            for (int i = 0; i < Record.BYTE_SIZE; i++)
            {
                bufor[i] = 0;
            }
        }           
        
    }
}
