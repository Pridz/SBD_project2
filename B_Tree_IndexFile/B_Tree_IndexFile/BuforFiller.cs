using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace B_Tree_IndexFile
{
    class BuforFiller
    {
        Bufor bufor;
        FileStream fileStream;
        RecordBufor recordBufor;
        /// <summary>
        ///     Position in the opened file using file stream.
        /// </summary>        
        long position;
        string path;

        public BuforFiller(Bufor bufor)
        {
            this.bufor = bufor;
            fileStream = null;
            recordBufor = new RecordBufor();
            position = 0;
            path = null;
        }

        void readBytes()
        {
            using (fileStream = File.Open(path, FileMode.Open))
            {
                int letter;                    
                fileStream.Seek(position, SeekOrigin.Begin);

                for (int i = 0; i < 3; i++)
                {
                    letter = fileStream.ReadByte();
                    while (letter != ' ')
                    {                        
                        recordBufor[recordBufor.Index] = (byte)letter;                        
                        letter = fileStream.ReadByte();
                    }                    
                    recordBufor[recordBufor.Index] = (byte)letter;
                    position = fileStream.Position;                    
                }
            }            
        }

        Record readRecord()
        {
            Record record;
            readBytes();
            record = DataDistinguisher.setRecord(recordBufor);
            recordBufor.nullify();
            return record;
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">Page number.</param>
        /// <param name="j">Record number in page.</param>
        void writeRecord(int i, int j)        
        {
            Record record = readRecord();
            bufor[i][j] = record;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">Number of page</param>
        void writePage(int i)            
        {
            int pageSize = bufor.PageSize;
            if (i >= bufor.AmountOfPages)
            {
                System.Console.Out.WriteLine("BuforFiller: writting to out of memory area! Too high page number!");
                return;
            }
            for (int j = 0; j < pageSize; j++)
            {
                writeRecord(i, j);
            }
        }
                
        Page readPage()
        {
            int pageSize = bufor.PageSize;
            Page page = new Page(pageSize);
            for (int j = 0; j < pageSize; j++)
            {
                page[j] = readRecord();
            }
            return page;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">Number of page</param>
        void writePageToBufor(int i)
        {
            bufor[i] = readPage();
        }
    }
}
