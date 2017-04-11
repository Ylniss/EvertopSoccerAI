using Evertop.Soccer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvertopSoccerAI
{
    public class Node : ICloneable
    {
        //All possible nodes that player can move.
        public Node[] Children { get; set; }

        //Parent of node, one higher in a tree hierarchy. Used in MoveTree.
        public Node Parent { get; set; }

        //Grand parent is last position of previous player, whole move tree higher in a tree hierarchy. Used in GameTree.
        public Node GrandParent { get; set; }

        //Move that represents node.
        public Move Move { get; set; }

        //Whole field of the game, holds all positions and edges.
        public Field Field { get; set; }

        //How good move is, evaluated by function.
        public double Score { get; set; }

        //Depth in tree. Indicates what number of move it is in whole sequence of moves.
        public int Depth { get; set; }

        //String representation of node (depth + move direction).
        public string Id { get; private set; }

        public Node(Move move, int depth, Field field, Node parent)
        {
            Move = move;
            Depth = depth;
            Field = field;
            Parent = parent;
            Id = Depth + Move.Direction.ToString();
        }

        public Node(Node[] children, Field field)
        {
            Children = children;
            Field = new Field(field);
        }

        public object Clone()
        {
            Node node = new Node(Move, Depth, Field, Parent);
            node.Children = Children;
            node.Score = Score;
            node.GrandParent = GrandParent;

            return node;
        }
    }
}