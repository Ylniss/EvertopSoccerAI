using Evertop.Soccer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvertopSoccerAI
{
    /// <summary>
    /// Helper methods associated with navigation.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Converts Position into Position2 that has overriden methods Equals and GetHashCode needed for TryGetValue method in Dictionary.
        /// </summary>
        public static Position2 ConvertPositionToPosition2(Position position)
        {
            return new Position2(position.X, position.Y);
        }

        /// <summary>
        /// Converts Position2 into Position.
        /// </summary>
        public static Position ConvertPosition2ToPosition(Position2 position)
        {
            return new Position(position.X, position.Y);
        }

        /// <summary>
        /// Returns position where ball will be from position and towards given direction.
        /// </summary>
        public static Position2 GetNextPosition(Position start, Direction direction)
        {
            switch (direction)
            {
                case Direction.Down: return new Position2(start.X, start.Y + 1);
                case Direction.DownAndLeft: return new Position2(start.X - 1, start.Y + 1);
                case Direction.Left: return new Position2(start.X - 1, start.Y);
                case Direction.LeftAndUp: return new Position2(start.X - 1, start.Y - 1);
                case Direction.Right: return new Position2(start.X + 1, start.Y);
                case Direction.RightAndDown: return new Position2(start.X + 1, start.Y + 1);
                case Direction.Up: return new Position2(start.X, start.Y - 1); ;
                case Direction.UpAndRight: return new Position2(start.X + 1, start.Y - 1);
                default: return new Position2(start.X, start.Y); ;
            }
        }

        /// <summary>
        /// Returns opposite direction to given direction.
        /// </summary>
        public static Direction OppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down: return Direction.Up;
                case Direction.DownAndLeft: return Direction.UpAndRight;
                case Direction.Left: return Direction.Right;
                case Direction.LeftAndUp: return Direction.RightAndDown;
                case Direction.Right: return Direction.Left;
                case Direction.RightAndDown: return Direction.LeftAndUp;
                case Direction.Up: return Direction.Down;
                case Direction.UpAndRight: return Direction.DownAndLeft;
                default: return direction;
            }
        }

        /// <summary>
        /// Cuts out last two player moves (whole moves, not only single actions) unless there was only single action in history of moves.
        /// </summary>
        public static List<Move> GetRecentMoves(Move[] historyMoves)
        {
            List<Move> recentMoves = new List<Move>();
            bool playerChanged = false;
            for (int i = historyMoves.Length - 1; playerChanged == false && i > -1; --i)
            {
                recentMoves.Add(historyMoves[i]);
                if ((i > 0 && i < historyMoves.Length - 1 && historyMoves[i].IsOpponentMove != historyMoves[i + 1].IsOpponentMove) || historyMoves.Length == 1)
                    playerChanged = true;
            }
            recentMoves.Reverse();
            return recentMoves;
        }

        /// <summary>
        /// Calculates possible moves from given position.
        /// </summary>
        public static List<Direction> CalculatePossibleMoves(Position2 position, Field field)
        {
            List<Direction> possibleMoves = new List<Direction> { Direction.Up, Direction.UpAndRight, Direction.Right, Direction.RightAndDown,
                                                            Direction.Down, Direction.DownAndLeft, Direction.Left, Direction.LeftAndUp };

            List<Direction> occupiedDirections;
            if (field.Edges.TryGetValue(position, out occupiedDirections))
            {
                possibleMoves = possibleMoves.Except(occupiedDirections).ToList();
            }

            return possibleMoves;
        }
    }
}