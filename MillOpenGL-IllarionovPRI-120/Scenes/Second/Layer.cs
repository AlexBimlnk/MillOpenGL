using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace MillOpenGL_IllarionovPRI_120
{
    class Layer
    {

        private Color ActiveColor;
        private int ListNom;
        public bool Visability { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int[,,] DrawPlace { get; private set; }

        public Layer(int s_W, int s_H, int listNom)
        {
            Width = s_W;
            Height = s_H;
            DrawPlace = new int[Width, Height, 4];

            // Устанавливаем прозрачность
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    DrawPlace[i, j, 3] = 1;

            ListNom = listNom;
            Visability = true;

            ActiveColor = Color.Black;
        }

        public void SetColor(Color newColor)
        {
            ActiveColor = newColor;
        }

        public void Draw(MyBrush BR, int x, int y)
        {
            int readlPosDrawStartX = x - BR.myBrush.Width / 2;
            int readlPosDrawStartY = y - BR.myBrush.Height / 2;

            if (readlPosDrawStartX < 0)
                readlPosDrawStartX = 0;
            if (readlPosDrawStartY < 0)
                readlPosDrawStartY = 0;

            int boundaryX = readlPosDrawStartX + BR.myBrush.Width;
            int boundaryY = readlPosDrawStartY + BR.myBrush.Height;

            if (boundaryX > Width)
                boundaryX = Width;
            if (boundaryY > Height)
                boundaryY = Height;

            int countX = 0, countY = 0;

            for (int i = readlPosDrawStartX; i < boundaryX; i++, countX++)
            {
                countY = 0;
                for (int j = readlPosDrawStartY; j < boundaryY; j++, countY++)
                {
                    if (BR.IsBrushErase())
                    {
                        // данная кисть - ластик
                        // помечаем данный пиксель как незакрашенный
                        // получаем текущий цвет пикселя маски
                        Color ret = BR.myBrush.GetPixel(countX, countY);

                        // цвет не красный
                        if (!(ret.R == 255 && ret.G == 0 && ret.B == 0))
                        {
                            // заполняем данный пиксель активным цветом из маски
                            DrawPlace[i, j, 3] = 1;
                        }
                    }
                    else
                    {
                        Color ret = BR.myBrush.GetPixel(countX, countY);

                        if (!(ret.R == 255 && ret.G == 0 && ret.B == 0))
                        {
                            DrawPlace[i, j, 0] = ActiveColor.R;
                            DrawPlace[i, j, 1] = ActiveColor.G;
                            DrawPlace[i, j, 2] = ActiveColor.B;
                            DrawPlace[i, j, 3] = 0;
                        }
                    }

                }
            }
        }

        public void RenderImage(bool FromList)
        {
            if (FromList) // указана визуализация из дисплейного списка, следовательно данный слой не активен
            {
                // вызываем дисплейный список
                Gl.glCallList(ListNom);
            }
            else // данный слой активен и визуализацию необходимо делать на ходу
            {
                // счетчик номеров элементов, которые должны участвовать в визуализации
                int count = 0;

                // проходим по всем точкам рисунка
                for (int ax = 0; ax < Width; ax++)
                {
                    for (int bx = 0; bx < Height; bx++)
                    {
                        // если точка в координатах ax,bx не помечена флагом "прозрачная"
                        if (DrawPlace[ax, bx, 3] != 1)
                        {
                            // не лучшие способ, но так мы подсчитаем количество действительно значимых точек слоя,
                            // которые должны быть визуализированны
                            count++;
                        }
                    }
                }

                // данный массив будет заполнен, а затем передан для быстрой отрисовки геометрии (в нашем случае - точек)
                // колич. точек * 2 (для хранения координат x и y каждой точки, которая будет отрисована)
                int[] arr_date_vertex = new int[count * 2];

                // данный массив будет содержать значения цветов для всех отрисовываемых точек
                // колич. точек * 3 (для хранения координат R G B значений цветов каждой точки, которая будет отрисована)
                float[] arr_date_colors = new float[count * 3];

                // счетчик элементов для создания массивов, которые будут переданы в реализацию OpenGL c
                // помощью функции glDrawArrays
                int now_element = 0;

                // теперь, когда мы выделили массив необходимого размера,
                // заполним его необходимыми значениями
                for (int ax = 0; ax < Width; ax++)
                {
                    for (int bx = 0; bx < Height; bx++)
                    {
                        // если точка с координатами ax,bx не помечена флагом "прозрачная"
                        // если данная точка НЕ помечена флагом, сигнализирующим о том, что она не должна быть визуализированна
                        if (DrawPlace[ax, bx, 3] != 1)
                        {
                            // заносим координаты точки (ax , bx ) в массив, который будет передан для визуализации
                            arr_date_vertex[now_element * 2] = ax;
                            arr_date_vertex[now_element * 2 + 1] = bx;

                            // заносим значения составляющих цвета, сразу перенося их в формат float
                            arr_date_colors[now_element * 3] = (float)DrawPlace[ax, bx, 0] / 255.0f;
                            arr_date_colors[now_element * 3 + 1] = (float)DrawPlace[ax, bx, 1] / 255.0f;
                            arr_date_colors[now_element * 3 + 2] = (float)DrawPlace[ax, bx, 2] / 255.0f;

                            // подсчет добавленных элементов в массивы
                            now_element++;
                        }
                    }
                }

                // теперь, когда массивы с геометрическими данными и данными о цветах подготовлены,
                // включаем функцию использования массивов вершин и цветов

                Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
                Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);

                // передаем массивы вершин и цветов, указывая количество элементов массива, приходящихся
                // на один визуализируемый элемент (в случае точек - 2 координаты: х и у, в случае цветов - 3 составляющие цвета)

                Gl.glColorPointer(3, Gl.GL_FLOAT, 0, arr_date_colors);
                Gl.glVertexPointer(2, Gl.GL_INT, 0, arr_date_vertex);

                // вызываем функцию glDrawArrays, которая позволит нам визуализировать наши массивы, передав их целиком,
                // а не передавая в цикле каждую точку
                Gl.glDrawArrays(Gl.GL_POINTS, 0, count);

                // деактивируем режим использования массивов геометрии и цветов
                Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
                Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);

            }


            //Gl.glBegin(Gl.GL_POINTS);

            //for (int i = 0; i < Width; i++)
            //{
            //    for (int j = 0; j < Height; j++)
            //    {
            //        // Если не прозрачная
            //        if (DrawPlace[i, j, 3] != 1)
            //        {
            //            Gl.glColor3f(
            //                DrawPlace[i, j, 0],
            //                DrawPlace[i, j, 1],
            //                DrawPlace[i, j, 2]);

            //            Gl.glVertex2i(i, j);
            //        }
            //    }
            //}

            //Gl.glEnd();
        }

        // инвертирование цветов
        public void Invers()
        {
            // циклами переберам все пиксели изображения
            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    // и инвертируем цвет, установленный в RGB составляющих, на обратный (255-R) (255-G) (255-B)
                    DrawPlace[X, Y, 0] = 255 - DrawPlace[X, Y, 0];
                    DrawPlace[X, Y, 1] = 255 - DrawPlace[X, Y, 1];
                    DrawPlace[X, Y, 2] = 255 - DrawPlace[X, Y, 2];
                }
            }
        }

        // функция обработки слоя изображения на основе полученной матрицы и дополнительных параметров
        // corr - коррекция составляющей цвета - после обработки каждого пикселя к каждой его составляющей будет
        // прибавлено данное значение
        // COEFF - коэфицент, реализующий усиление работы фильтра
        // need_count_correction - необходимость корректировки значения полученного пикселя после прохода фильтра
        // если данный параметра установлен, то каждая составляющая цвета, перед тем как быть преведенной к виду 0-255
        // будет разделена на количество произошедших с ней преобразований
        // это необходимо для корректной работы некоторых фильтров
        public void PixelTransformation(float[] mat, int corr, float COEFF, bool need_count_correction)
        {
            // массив для получения результирующего пикселя
            float[] resault_RGB = new float[3];
            int count = 0;
            // проходим циклом по всем пикселям слоя
            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    // цикл по всем составляющим (0-2, т.е. R G B)
                    for (int c = 0, ax = 0, bx = 0; c < 3; c++)
                    {
                        // обнуление составляющей результата
                        resault_RGB[c] = 0;
                        // обнуление счетчика обработок
                        count = 0;

                        // два цикла для захвата области 3х3 вокруг обрабатываемого пикселя
                        for (bx = -1; bx < 2; bx++)
                        {
                            for (ax = -1; ax < 2; ax++)
                            {
                                // если мы не попали в рамки, просто используем центральный пиксель, и продолжаем цикл
                                if (X + ax < 0 || X + ax > Width - 1 || Y + bx < 0 || Y + bx > Height - 1)
                                {
                                    // считаем составляющую в одной из точек, используем коэфицент в матрице (под номером текущей итерации), коэфицент усиления (COEFF) и прибовляем коррекцию (corr)
                                    resault_RGB[c] += (float)(DrawPlace[X, Y, c]) * mat[count] * COEFF + corr;
                                    // счетчик обработок = ячейке матрицы с необходимым коэфицентом
                                    count++;
                                    // продолжаем цикл
                                    continue;
                                }

                                // иначе, если мы укладываемся в изображение (не пересекаем границы), используем соседние пиксели, корректируем ячейку массива параметрами ax, bx
                                resault_RGB[c] += (float)(DrawPlace[X + ax, Y + bx, c]) * mat[count] * COEFF + corr;
                                // счетчик обработок = ячейке матрицы с необходимым коэфицентом
                                count++;
                            }
                        }

                    }

                    // теперь для всех составляющих корректируем цвет
                    for (int c = 0; c < 3; c++)
                    {
                        // если требуется разделить результат до приведения к 0-255, разделив на количество проведенных операций
                        if (count != 0 && need_count_correction)
                        {
                            // выполняем данное деление
                            resault_RGB[c] /= count;
                        }

                        // если значение меньше нуля
                        if (resault_RGB[c] < 0)
                        {
                            // - приравниваем к нулю
                            resault_RGB[c] = 0;
                        }

                        // если больше 255
                        if (resault_RGB[c] > 255)
                        {
                            // приравниваем к 255
                            resault_RGB[c] = 255;
                        }
                        // записываем в массив цветов слоя новое значение
                        DrawPlace[X, Y, c] = (int)resault_RGB[c];
                    }
                }
            }

        }
    }
}
