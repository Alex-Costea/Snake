using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static Form1 CurrentForm;
        public static Random rnd = new Random();
        public static int CellSize = 16;
        public static int CellNumber = 20;//>=8
        public static int WindowSize = CellSize * CellNumber;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CurrentForm = new Form1();
            Application.Run(CurrentForm);
        }
    }
}
