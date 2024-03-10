using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillOpenGL_IllarionovPRI_120
{
    class MyBrush
    {
        public Bitmap myBrush;
        // флаг, сигнализирующий о том, что установленная кисть является ластиком
        private bool IsErase = false;

        public MyBrush(int value, bool special)
        {
            if (!special)
            {
                myBrush = new Bitmap(value, value);

                for (int i = 0; i < value; i++)
                    for (int j = 0; j < value; j++)
                        myBrush.SetPixel(i, j, Color.Black);

                IsErase = false;
            }
            else
            {
                switch (value)
                {
                    default:
                        {
                            myBrush = new Bitmap(5, 5);

                            for (int i = 0; i < 5; i++)
                                for (int j = 0; j < 5; j++)
                                    myBrush.SetPixel(i, j, Color.Red);

                            myBrush.SetPixel(0, 2, Color.Black);
                            myBrush.SetPixel(1, 2, Color.Black);

                            myBrush.SetPixel(2, 0, Color.Black);
                            myBrush.SetPixel(2, 1, Color.Black);
                            myBrush.SetPixel(2, 2, Color.Black);
                            myBrush.SetPixel(2, 3, Color.Black);
                            myBrush.SetPixel(2, 4, Color.Black);

                            myBrush.SetPixel(3, 2, Color.Black);
                            myBrush.SetPixel(4, 2, Color.Black);

                            break;
                        }
                    case 1: // стерка
                        {
                            // создается так же, как и обычная кисть,
                            // но имеет флаг IsErase равный true
                            myBrush = new Bitmap(5, 5);

                            for (int ax = 0; ax < value; ax++)
                                for (int bx = 0; bx < value; bx++)
                                    myBrush.SetPixel(0, 0, Color.Black);

                            // является ластиком
                            IsErase = true;
                            break;
                        }
                    case 2: // Инициалы и группа
                        {

                            int size = 3;
                            // создается так же, как и обычная кисть,
                            // но имеет флаг IsErase равный true
                            myBrush = new Bitmap(171, 41);

                            Init();
                            PrintA(size);
                            PrintM(size);
                            Print1(size);
                            Print2(size);
                            Print0(size);

                            IsErase = false;
                            break;
                        }
                }
            }
        }

        void Init()
        {
            for (int i = 0; i < myBrush.Height; i++)
            {
                for (int j = 0; j < myBrush.Width; j++)
                {
                    myBrush.SetPixel(j, i, Color.Red);
                }
            }
        }
        void DrawHorizontal(int startX, int startY, int count)
        {
            for (int x = startX; x <= startX + count; x++)
            {
                myBrush.SetPixel(x, startY, Color.Black);
            }
        }
        void DrawVeritcal(int startX, int startY, int count)
        {
            for (int y = startY; y <= startY + count; y++)
            {
                myBrush.SetPixel(startX, y, Color.Black);
            }
        }
        void DrawDigDownToRight(int startX, int startY, int count)
        {
            int pixSet = 0;
            for (int y = startY; y <= startY + count; y++, pixSet++)
            {
                myBrush.SetPixel(startX + pixSet, y, Color.Black);
            }
        }
        void DrawDigDownToLeft(int startX, int startY, int count)
        {
            int pixSet = 0;
            for (int y = startY; y <= startY + count; y++, pixSet++)
            {
                myBrush.SetPixel(startX - pixSet, y, Color.Black);
            }
        }

        void PrintA(int size)
        {
            DrawDigDownToLeft(40, 0, 20);
            DrawDigDownToRight(0, 0, 20);
            DrawHorizontal(10, 10, 20);
        }
        void PrintM(int size)
        {
            DrawVeritcal(50, 0, 20);
            DrawVeritcal(90, 0, 20);
            DrawDigDownToLeft(70, 0, 20);
            DrawDigDownToRight(70, 0, 20);
        }

        void Print1(int size)
        {
            DrawVeritcal(110, 0, 20);
        }
        void Print2(int size)
        {
            DrawHorizontal(120, 0, 20);
            DrawHorizontal(120, 20, 20);
            DrawDigDownToRight(120, 0, 20);
        }
        void Print0(int size)
        {
            DrawHorizontal(150, 0, 20);
            DrawHorizontal(150, 20, 20);
            DrawVeritcal(150, 0, 20);
            DrawVeritcal(170, 0, 20);
        }

        // функция, которая будет использоваться для получения информации
        // о том, является ли данная кисть ластиком
        public bool IsBrushErase()
        {
            return IsErase;
        }
    }
}
