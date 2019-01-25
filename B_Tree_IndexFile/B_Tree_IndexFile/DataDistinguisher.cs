using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile
{
    static public class DataDistinguisher
    {
        static public Record setRecord(RecordBufor recordBufor)
        {
            Record record = new Record();

            byte letter = recordBufor[0];
            int i = 0;
            int position;
            long number = 0;            
            while (letter != ' ')
            {
                i++;
                letter = recordBufor[i];
            }
            for (int j = 0; j < i; j++)
            {
                record.Name[j] = (char)recordBufor[j];
            }
            i++;
            position = i;
            letter = recordBufor[i];
            while (letter != ' ')
            {
                i++;
                letter = recordBufor[i];
            }
            for (int j = position; j < i; j++)
            {
                record.Surname[j] = (char)recordBufor[j];
            }
            i++;
            position = i;
            letter = recordBufor[i];
            while (letter != ' ')
            {
                i++;
                letter = recordBufor[i];
            }
            for (int j = position; j < i; j++)
            {
                number = number*10 + recordBufor[j];
            }
            record.Pesel = (ulong)number;

            return record;
        }
        
    }    
}
