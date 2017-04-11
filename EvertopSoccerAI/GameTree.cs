using Evertop.Soccer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvertopSoccerAI
{
    /// <summary>
    /// Tree of all (depth dependent) possible final moves of two players.
    /// </summary>
    public class GameTree
    {
        //How deep game tree is going to be created. Every +1 of depth means other player whole sequence of moves made. Value shouldn't be to high.
        public int MaxGameTreeDepth { get; set; }

        public int MaxMoveTreeDepth { get; set; }

        public Node Root { get; set; }

        public GameTree(MoveTree moveTree, Field field, int maxGameTreeDepth, int maxMoveTreeDepth)
        {
            MaxGameTreeDepth = maxGameTreeDepth;
            MaxMoveTreeDepth = maxMoveTreeDepth;

            Root = new Node(createTree(moveTree, field), field);
        }

        private Node[] createTree(MoveTree initialMoveTree, Field field, int depth = 0)
        {
            Node[] children = initialMoveTree.PossibleMoveNodes.ToArray().Select(x => (Node)x.Clone()).ToArray();

            if (depth >= MaxGameTreeDepth)
            {
                return children;
            }

            foreach (Node node in children)
            {
                Position nextPosition = NavigationHelper.ConvertPosition2ToPosition(NavigationHelper.GetNextPosition(node.Move.Start, node.Move.Direction));
                MoveTree moveTree = new MoveTree(nextPosition, node.Field, MaxMoveTreeDepth - depth);

                node.Children = new Node[moveTree.PossibleMoveNodes.Count];
                node.Children = createTree(moveTree, node.Field, depth + 1);

                foreach (Node child in node.Children)
                    child.GrandParent = node;
            }

            return children;
        }

        /// <summary>
        /// Climbs up the tree to get last grand parent (nearest to the root).
        /// </summary>
        public Node GetLastParent(Node node)
        {
            if (node.GrandParent != null)
                node = GetLastParent(node.GrandParent);

            return node;
        }
    }
}
