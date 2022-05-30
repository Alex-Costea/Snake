using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Snake.Properties;
namespace Snake
{
    public partial class Form1 : Form
    {
        int points = 0;
        Image[,] Images = new Image[Program.CellNumber,Program.CellNumber];
        Snake snake;

        //Change the number of points, but with multithreading support
        delegate void TextDel(int p);
        private void UpdateText(int p)
        {
            if (this.label1.InvokeRequired)
            {
                TextDel d = new TextDel(UpdateText);
                try
                {
                    this.Invoke(d, p);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
            }
            else
            {
                label1.Text = String.Format("{0} points", points);
            }
        }

        public int Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
                UpdateText(value);
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(Program.WindowSize, Program.WindowSize);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            label1.ForeColor = Color.White;
            label1.BackColor = Color.DarkBlue;
            label1.Location = new Point(Program.WindowSize-72, Program.WindowSize-22);
        }

        //Invalidate, and then Update, a certain cell, but with multithreading support
        delegate void IAUDel(int x,int y);
        private void InvalidateAndUpdate(int x,int y)
        {
            if (this.InvokeRequired)
            {
                IAUDel d = new IAUDel(InvalidateAndUpdate);
                try
                {
                    this.Invoke(d, x,y);
                }
                catch(ObjectDisposedException)
                {
                    return;
                }
            }
            else
            {
                Invalidate(new Rectangle(x,y, Program.CellSize,Program.CellSize));
                Update();
            }
        }

        public void PaintGrass(int x,int y)
        {
            Image curImage;
            curImage = Resources.Grass;
            curImage.RotateFlip((RotateFlipType)Program.rnd.Next(0, 8));
            Images[x, y] = curImage;
            InvalidateAndUpdate(x * Program.CellSize, y * Program.CellSize);
        }

        public void PaintBody1(int x,int y, int rotation = 0)
        {
            Image curImage;
            curImage = Resources.SnakeBody1;
            curImage.RotateFlip((RotateFlipType)rotation);
            Images[x, y] = curImage;
            InvalidateAndUpdate(x * Program.CellSize, y * Program.CellSize);
        }

        public void PaintBody2(int x, int y, int rotation = 0)
        {
            Image curImage;
            curImage = Resources.SnakeBody2;
            curImage.RotateFlip((RotateFlipType)rotation);
            Images[x, y] = curImage;
            InvalidateAndUpdate(x * Program.CellSize, y * Program.CellSize);
        }

        public void PaintHead(int x,int y, int rotation = 0)
        {
            Image curImage;
            curImage = new Image[] { Resources.SnakeHead, Resources.SnakeHead2 }[Program.rnd.Next(2)];
            curImage.RotateFlip((RotateFlipType)rotation);
            Images[x, y] = curImage;
            InvalidateAndUpdate(x * Program.CellSize, y * Program.CellSize);
        }

        public void PaintFruit(int x, int y)
        {
            Image curImage;
            curImage = new Image[] { Resources.Fruit, Resources.Fruit2 }[Program.rnd.Next(2)];
            curImage.RotateFlip((RotateFlipType)Program.rnd.Next(0, 8));
            Images[x, y] = curImage;
            InvalidateAndUpdate(x * Program.CellSize, y * Program.CellSize);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            for(int i=0;i<Program.CellNumber;i++)
                for(int j=0;j< Program.CellNumber; j++)
                {
                    e.Graphics.DrawImage(Images[i,j], new Point(i * Program.CellSize, j * Program.CellSize));
                }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    snake.ChangeSnakeDirection(Direction.Up);
                    break;
                case Keys.Down:
                case Keys.S:
                    snake.ChangeSnakeDirection(Direction.Down);
                    break;
                case Keys.Left:
                case Keys.A:
                    snake.ChangeSnakeDirection(Direction.Left);
                    break;
                case Keys.Right:
                case Keys.D:
                    snake.ChangeSnakeDirection(Direction.Right);
                    break;
                case Keys.D1:
                    snake.Speed = 10;
                    break;
                case Keys.D2:
                    snake.Speed = 15;
                    break;
                case Keys.D3:
                    snake.Speed = 20;
                    break;
                case Keys.D4:
                    snake.Speed = 40;
                    break;
                case Keys.D5:
                    snake.Speed = 75;
                    break;
                case Keys.D6:
                    snake.Speed = 100;
                    break;
                case Keys.D7:
                    snake.Speed = 125;
                    break;
                case Keys.D8:
                    snake.Speed = 150;
                    break;
                case Keys.D9:
                    snake.Speed = 200;
                    break;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //not using PaintGrass in order to avoid flickering
            for (int i = 0; i < Program.CellNumber; i++)
                for (int j = 0; j < Program.CellNumber; j++)
                {
                    Images[i, j] = Resources.Grass;
                    Images[i,j].RotateFlip((RotateFlipType)Program.rnd.Next(0, 8));
                }
            Refresh();
            snake = new Snake();
        }
    }
}
