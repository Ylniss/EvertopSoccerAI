using Evertop.Soccer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvertopSoccerAI
{
    /// <summary>
    /// Field of game. Holds: all positions and its edges, goal/gate positions, size of the field and on what side is player.
    /// </summary>
    public class Field
    {
        //All positions on the field with directions where edges are
        public Dictionary<Position2, List<Direction>> Edges;

        //Positions of gate. Goal of a game is to get ball to one of this positions.
        public Position2[] GoalPositions = new Position2[3];

        public int Length { get; }
        public int Width { get; }
        public FieldSide PlayerSide { get; }

        public Field(FieldData fieldData)
        {
            PlayerSide = fieldData.PlayerSide;
            Length = fieldData.Length;
            Width = fieldData.Width;

            Edges = new Dictionary<Position2, List<Direction>>();

            calculateFieldEdges();
            GoalPositions = calculateGoalPositions(PlayerSide);
        }

        public Field(Field field)
        {
            PlayerSide = field.PlayerSide;
            Length = field.Length;
            Width = field.Width;

            GoalPositions = field.GoalPositions;

            //create deep copy of dictionary
            Edges = field.Edges.ToDictionary(p => p.Key, p => p.Value.ToList());
        }

        /// <summary>
        /// Calculates edges made from recent moves made by players
        /// </summary>
        public void CalculateRecentMoveEdges(Move[] historyMoves)
        {
            List<Move> recentMoves = NavigationHelper.GetRecentMoves(historyMoves);

            foreach (Move move in recentMoves)
            {
                Position2 nextPosition = NavigationHelper.GetNextPosition(move.Start, move.Direction);

                //Add new direction to existing list in dictionary
                List<Direction> directions;
                if (Edges.TryGetValue(NavigationHelper.ConvertPositionToPosition2(move.Start), out directions))
                {
                    directions.Add(move.Direction);
                    if (Edges.TryGetValue(nextPosition, out directions))
                        directions.Add(NavigationHelper.OppositeDirection(move.Direction));
                    else
                        Edges.Add(nextPosition, new List<Direction> { NavigationHelper.OppositeDirection(move.Direction) });
                }
                else
                {
                    Edges.Add(NavigationHelper.ConvertPositionToPosition2(move.Start), new List<Direction> { move.Direction });
                    Edges.Add(nextPosition, new List<Direction> { NavigationHelper.OppositeDirection(move.Direction) });
                }
            }
        }

        /// <summary>
        /// Checks if there was already some move made on given position.
        /// </summary>
        public bool IsPositionOccupied(Position2 position)
        {
            if (Edges.ContainsKey(position))
                return true;

            return false;
        }

        /// <summary>
        /// Calculates field edges
        /// </summary>
        private void calculateFieldEdges()
        {
            //add upper left corner
            Edges.Add(new Position2(0, 0), new List<Direction> { Direction.Down, Direction.Right, Direction.UpAndRight, Direction.Up, Direction.LeftAndUp, Direction.Left, Direction.DownAndLeft });
            //add upper right corner
            Edges.Add(new Position2(Length, 0), new List<Direction> { Direction.Down, Direction.Left, Direction.UpAndRight, Direction.Up, Direction.LeftAndUp, Direction.Right, Direction.RightAndDown });
            //add upper field edge
            for (int i = 1; i < Length; ++i)
                Edges.Add(new Position2(i, 0), new List<Direction> { Direction.Left, Direction.Right, Direction.UpAndRight, Direction.Up, Direction.LeftAndUp });

            //add lower left corner
            Edges.Add(new Position2(0, Width), new List<Direction> { Direction.Up, Direction.Right, Direction.RightAndDown, Direction.Down, Direction.DownAndLeft, Direction.Left, Direction.LeftAndUp });
            //add lower right corner
            Edges.Add(new Position2(Length, Width), new List<Direction> { Direction.Up, Direction.Left, Direction.DownAndLeft, Direction.Down, Direction.RightAndDown, Direction.Right, Direction.UpAndRight });
            //add lower field edge
            for (int i = 1; i < Length; ++i)
                Edges.Add(new Position2(i, Width), new List<Direction> { Direction.Left, Direction.Right, Direction.RightAndDown, Direction.Down, Direction.DownAndLeft });

            //add left field edge
            for (int i = 1; i < Width; ++i)
            {
                if (i == Width / 2 - 1) //upper gatepost
                    Edges.Add(new Position2(0, i), new List<Direction> { Direction.Left, Direction.Up, Direction.LeftAndUp });
                else if (i == Width / 2 + 1) //lower gatepost
                    Edges.Add(new Position2(0, i), new List<Direction> { Direction.Left, Direction.Down, Direction.DownAndLeft });
                else if (i != Width / 2)
                    Edges.Add(new Position2(0, i), new List<Direction> { Direction.Up, Direction.LeftAndUp, Direction.Left, Direction.DownAndLeft, Direction.Down });
            }

            //add right field edge
            for (int i = 1; i < Width; ++i)
            {
                if (i == Width / 2 - 1) //upper gatepost
                    Edges.Add(new Position2(Length, i), new List<Direction> { Direction.Right, Direction.Up, Direction.UpAndRight });
                else if (i == Width / 2 + 1) //lower gatepost
                    Edges.Add(new Position2(Length, i), new List<Direction> { Direction.Right, Direction.Down, Direction.RightAndDown });
                else if (i != Width / 2)
                    Edges.Add(new Position2(Length, i), new List<Direction> { Direction.Up, Direction.UpAndRight, Direction.Right, Direction.RightAndDown, Direction.Down });
            }
        }

        /// <summary>
        /// Calculates positions that ball needs to go towards (goal positions).
        /// </summary>
        private Position2[] calculateGoalPositions(FieldSide playerSide)
        {
            Position2[] goalPositions = new Position2[3];

            if (playerSide == FieldSide.Right)
            {
                goalPositions[0] = new Position2(-1, Width / 2 - 1);
                goalPositions[1] = new Position2(-1, Width / 2);
                goalPositions[2] = new Position2(-1, Width / 2 + 1);
            }
            else
            {
                goalPositions[0] = new Position2(Length + 1, Width / 2 - 1);
                goalPositions[1] = new Position2(Length + 1, Width / 2);
                goalPositions[2] = new Position2(Length + 1, Width / 2 + 1);
            }

            return goalPositions;
        }

    }
}