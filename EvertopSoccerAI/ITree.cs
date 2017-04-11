using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvertopSoccerAI
{
    interface ITree
    {
        //How deep tree is going to be created.
        int MaxDepth { get; set; }

        //Head of the tree (beginning node).
        Node Root { get; }
    }
}
