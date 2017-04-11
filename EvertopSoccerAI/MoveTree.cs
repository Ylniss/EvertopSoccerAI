using Evertop.Soccer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvertopSoccerAI
{
    /// <summary>
    /// Tree of all possible single moves in one player turn. Represents one player whole sequence of moves.
    /// </summary>
    public class MoveTree
    {
        //How deep tree of single move sequence is going to be created.
        public int MaxDepth { get; set; }

        public Node Root { get; private set; }

        //All possible nodes that whole sequence of single moves can end with.
        public List<Node> PossibleMoveNodes { get; private set; } = new List<Node>();

        public MoveTree(Position position, Field field, int maxDepth)
        {
            MaxDepth = maxDepth;

            Root = new Node(createTree(position, field), field);
            depthFirstTraverse(Root);
        }

        /// <summary>
        /// Creates tree of all possible moves from given position on the field
        /// </summary>
        /// <param name="depth"> Depth of the tree, represents length of movement path. </param>
        private Node[] createTree(Position ballPosition, Field currentField, Node parent = null, int depth = 1)
        {
            Position2 ballPosition2 = NavigationHelper.ConvertPositionToPosition2(ballPosition);
            List<Direction> possibleDirections = NavigationHelper.CalculatePossibleMoves(ballPosition2, currentField);

            Node[] children = new Node[possibleDirections.Count];
            for (int i = 0; i < children.Length; ++i)
            {
                Move move = new Move(false, ballPosition, possibleDirections[i]);

                children[i] = new Node(move, depth, new Field(currentField), parent);

                Position2 nextPosition = NavigationHelper.GetNextPosition(move.Start, move.Direction);

                //if its possible to bounce...
                if (children[i].Field.IsPositionOccupied(nextPosition) && depth <= MaxDepth)
                {
                    updateField(children[i].Field, move);
                    children[i].Children = createTree(NavigationHelper.ConvertPosition2ToPosition(nextPosition), children[i].Field, children[i], depth + 1);
                }
                else
                    children[i].Score = evaluateFunction(move, children[i].Field, depth);

                updateField(children[i].Field, move);
            }

            return children;
        }

        ///// <summary>
        ///// Returns first move that will be made from given node by climbing through parents.
        ///// </summary>
        //public Move GetFirstMoveFromGivenNode(Node node)
        //{
        //    Node grandParent = getLastParent(node);

        //    if (grandParent != null)
        //        return grandParent.Move;
        //    else
        //        return null;
        //}

        ///// <summary>
        ///// Returns best scored node by climbing through parents.
        ///// </summary>
        //public Node GetBestScoredNode()
        //{
        //    Node grandParent = getLastParent(GetHighetScoreNode());

        //    if (grandParent != null)
        //        return grandParent;
        //    else
        //        return null;
        //}


        /// <summary>
        /// Climbs up the tree to get last grand parent (nearest to the root).
        /// </summary>
        public Node GetLastParent(Node node)
        {
            if (node.Parent != null)
                node = GetLastParent(node.Parent);

            return node;
        }

        /// <summary>
        /// Returns node with given score from list of possible move nodes.
        /// </summary>
        public Node GetNodeWithGivenScore(double score)
        {
            return PossibleMoveNodes.First(node => node.Score == score);
        }

        /// <summary>
        /// Searches through list of all nodes and gets one with the highest score.
        /// </summary>
        public Node GetHighetScoreNode()
        {
            return PossibleMoveNodes.Aggregate((node1, node2) => node1.Score > node2.Score ? node1 : node2);
        }

        /// <summary>
        /// Traverses whole tree and fills list with all possible nodes that move may be finnished with.
        /// </summary>
        private void depthFirstTraverse(Node startNode)
        {
            if (startNode.Children == null)
                PossibleMoveNodes.Add(startNode);

            else
                foreach (Node child in startNode.Children)
                {
                    if (!PossibleMoveNodes.Contains(child))
                        depthFirstTraverse(child);
                }
        }

        /// <summary>
        /// Evaluates how strong given path is
        /// </summary>
        private double evaluateFunction(Move move, Field field, int depthBonus)
        {
            double score = 0;

            Position2 lastPosition = NavigationHelper.GetNextPosition(move.Start, move.Direction);

            if(field.PlayerSide == FieldSide.Right)
                score += (1 - ((double)(lastPosition.X + 1) / (double)(field.Length + 1)));
            else
                score += ((double)(lastPosition.X + 1) / (double)(field.Length + 1));

            score += depthBonus * 0.005;

            return score;
        }

        /// <summary>
        /// Updates field with given move
        /// </summary>
        private void updateField(Field field, Move move)
        {
            field.CalculateRecentMoveEdges(new Move[] { move });
        }

    }
}