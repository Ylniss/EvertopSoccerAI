using Evertop.Soccer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvertopSoccerAI
{
    public class PlayerAi : IPlayer
    {
        private const double infinity = 100;
        private const int maxMoveTreeDepth = 5;
        private const int maxGameTreeDepth = 1;

        private Field field;

        public string Name
        {
            get
            {
                return "Porobieniec";
            }
        }

        public Direction GetNextMove(Move[] historyMoves, Position ballPosition, Direction[] possibleMoves)
        {
            field.CalculateRecentMoveEdges(historyMoves);

            MoveTree moveTree = new MoveTree(ballPosition, new Field(field), maxMoveTreeDepth);

            GameTree gameTree = new GameTree(moveTree, new Field(field), maxGameTreeDepth, maxMoveTreeDepth);

            Node bestNode = alphaBeta(gameTree.Root, -infinity, infinity, true);

            bestNode = gameTree.GetLastParent(bestNode);

            Move choosenMove = moveTree.GetLastParent(bestNode).Move;

            return choosenMove.Direction;
        }

        public void StartMatch(FieldData fieldData)
        {
            field = new Field(fieldData);
        }


        private Node alphaBeta(Node startNode, double alpha, double beta, bool isMaximizing)
        {
            if (startNode.Children == null || startNode.Score >= 1)
            {
                return startNode;
            }

            if (isMaximizing)
            {
                double bestScore = -infinity;
                Node bestNode = startNode;
                foreach (Node child in startNode.Children)
                {
                    bestScore = Math.Max(bestScore, alphaBeta(child, alpha, beta, false).Score);
                    if (alpha != Math.Max(alpha, bestScore))
                        bestNode = child;
                    alpha = Math.Max(alpha, bestScore);
                    if (beta <= alpha)
                        break;
                }
                return bestNode;
            }
            else
            {
                double bestScore = infinity;
                Node bestNode = startNode;
                foreach (Node child in startNode.Children)
                {
                    bestScore = Math.Min(bestScore, alphaBeta(child, alpha, beta, true).Score);
                    if (beta != Math.Max(alpha, bestScore))
                        bestNode = child;
                    beta = Math.Min(beta, bestScore);
                    if (beta <= alpha)
                        break;
                }
                return bestNode;
            }
        }
    }
}