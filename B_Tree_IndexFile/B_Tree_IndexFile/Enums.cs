using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_Tree_IndexFile
{
    /// <summary>
    /// Contain constants describing results of:
    /// searching, inserting and compensation
    /// </summary>
    enum Result
    {
        // Searching results
        NotFound,
        Found,

        // Insertion results
        Added,
        AlreadyExist,

        // Compensation results
        Compensated,
        NotCompensated,

        // Deletion results
        Deleted
    }
    
}
