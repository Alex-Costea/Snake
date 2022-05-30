using System;
using Snake;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Snake.Properties;
using System.Timers;

namespace Snake
{
    enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3,
        None=6
    }

    class Snake
    {
        Point origin;
        Point head;
        Point fruit;
        bool IsGameOver = false;
        Timer MyTimer = new Timer(75);
        Direction LastPressed;
        LinkedList<Direction> SnakeDirections = new LinkedList<Direction>();
        bool[,] SnakePieces = new bool[Program.CellNumber,Program.CellNumber];
        private int speed;
        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                MyTimer.Interval = value;
            }
        }

        private void PlaceFruit()
        {
            int MaxNumber = Program.CellNumber*Program.CellNumber - SnakeDirections.Count() - 1;
            if(MaxNumber==0)
            {
                IsGameOver = true;
                System.Windows.Forms.Application.Run(new HighScores());
                return;
            }
            int RandomNumber = Program.rnd.Next(MaxNumber);
            int Count=0;
            for (int i = 0; i <= Program.CellNumber-1; i++)
                for (int j = 0; j <= Program.CellNumber-1; j++)
                    if (!SnakePieces[i, j]) 
                    {
                        if (Count == RandomNumber)
                        {
                            fruit = new Point(i, j);
                            Program.CurrentForm.PaintFruit(fruit.X, fruit.Y);
                            return;
                        }
                        Count++;
                    }
            throw new Exception("Fruit not placed for some reason!");
        }

        private Point MovePoint(Point p,Direction d)
        {
            switch (d)
            {
                case Direction.Up:
                    return new Point(p.X, p.Y == 0 ? Program.CellNumber-1 : (p.Y - 1) % Program.CellNumber);
                case Direction.Down:
                    return new Point(p.X, (p.Y + 1) % Program.CellNumber);
                case Direction.Left:
                    return new Point(p.X == 0 ? Program.CellNumber-1 : (p.X - 1) % Program.CellNumber, p.Y);
                case Direction.Right:
                    return new Point((p.X + 1) % Program.CellNumber, p.Y);
                default:
                    return p;
            }
        }

        public Snake()
        {
            Direction moveDirection = (Direction)Program.rnd.Next(4);

            //origin
            origin = new Point(Program.rnd.Next(Program.CellNumber), Program.rnd.Next(Program.CellNumber));
            PaintBody(origin.X, origin.Y, moveDirection, moveDirection);
            SnakePieces[origin.X, origin.Y] = true;

            //body
            Point aux = MovePoint(origin, moveDirection);
            for (int i = 1; i <= Program.rnd.Next(4,7); i++)
            {
                PaintBody(aux.X, aux.Y, moveDirection, moveDirection);
                SnakePieces[aux.X, aux.Y] = true;
                SnakeDirections.AddLast(moveDirection);
                aux = MovePoint(aux, moveDirection);
            }

            //head
            head = aux;
            PaintHead(head.X, head.Y, moveDirection);
            SnakePieces[head.X, head.Y] = true;
            SnakeDirections.AddLast(moveDirection);

            LastPressed = moveDirection;
            PlaceFruit();
            MyTimer.Elapsed += TimerUp;
            MyTimer.Start();
        }

        private void PaintBody(int x,int y,Direction LastDirection,Direction CurrentDirection)
        {
            int direction = 0;
            if (LastDirection == CurrentDirection)
            {
                Program.CurrentForm.PaintBody1(x,y, CurrentDirection == Direction.Up || CurrentDirection == Direction.Down ? 1 : 0);
            }
            else
            {
                //choose the correct orientation
                if (LastDirection == Direction.Up && CurrentDirection == Direction.Left)
                    direction = 3;
                if (LastDirection == Direction.Up && CurrentDirection == Direction.Right)
                    direction = 2;
                if (LastDirection == Direction.Down && CurrentDirection == Direction.Left)
                    direction = 5;
                if (LastDirection == Direction.Down && CurrentDirection == Direction.Right)
                    direction = 4;
                if (LastDirection == Direction.Left && CurrentDirection == Direction.Up)
                    direction = 1;
                if (LastDirection == Direction.Left && CurrentDirection == Direction.Down)
                    direction = 7;
                if (LastDirection == Direction.Right && CurrentDirection == Direction.Up)
                    direction = 0;
                if (LastDirection == Direction.Right && CurrentDirection == Direction.Down)
                    direction = 6;
                Program.CurrentForm.PaintBody2(x,y, direction);
            }
        }

        private void PaintHead(int x,int y, Direction CurrentDirection)
        {
            //choose the correct orientation
            int direction = 0;
            if (CurrentDirection == Direction.Up)
                direction = 1;
            if (CurrentDirection == Direction.Down)
                direction = 3;
            if (CurrentDirection == Direction.Left)
                direction = 0;
            if (CurrentDirection == Direction.Right)
                direction = 2;
            Program.CurrentForm.PaintHead(x,y,direction);
        }

        private bool ContradictoryDirection(Direction d1,Direction d2)
        {
            return Math.Abs((int)d1-(int)d2)==2;
        }

        private void MoveSnake(Direction d)
        {
            Point futureHead = MovePoint(head, d);
            if (futureHead != fruit)
            {
                //remove last piece
                SnakePieces[origin.X, origin.Y] = false;
                Program.CurrentForm.PaintGrass(origin.X, origin.Y);
                origin = MovePoint(origin, SnakeDirections.First());
                SnakeDirections.RemoveFirst();
            }
            else
            {
                SnakePieces[futureHead.X, futureHead.Y] = true;
                SnakeDirections.AddLast(d);

                //place fruit
                Program.CurrentForm.Points += Program.rnd.Next(8, 11);
                PlaceFruit();

                SnakePieces[futureHead.X, futureHead.Y] = false;
                SnakeDirections.RemoveLast();
            }

            //paint body
            PaintBody(head.X, head.Y, SnakeDirections.Last(), d);

            //add head
            head = futureHead;
            PaintHead(head.X, head.Y, d);
            if (SnakePieces[head.X, head.Y])
            {
                //game was lost
                IsGameOver = true;
                System.Windows.Forms.Application.Run(new HighScores());
            }
            SnakePieces[head.X, head.Y] = true;
            SnakeDirections.AddLast(d);
        }

        private void TimerUp(object sender, ElapsedEventArgs e)
        {
            MyTimer.Stop();
            if (ContradictoryDirection(LastPressed, SnakeDirections.Last()))
            {
                MoveSnake(SnakeDirections.Last());
            }
            else MoveSnake(LastPressed);
            if(!IsGameOver)
                MyTimer.Start();
        }

        internal void ChangeSnakeDirection(Direction NewDirection)
        {
            LastPressed = NewDirection;
        }
    }
}
