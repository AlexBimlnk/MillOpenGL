using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillOpenGL_IllarionovPRI_120
{
    class PifagorFractal
    {
        private int pictureSizeX, pictureSizeY;

        private List<Layer> Layers = new List<Layer>();

        private MyBrush standartBrush;

        public int ActiveLayerNom { private get; set; }

        public PifagorFractal(int sizeX, int sizeY)
        {
            pictureSizeX = sizeX;
            pictureSizeY = sizeY;

            Layers.Add(new Layer(pictureSizeX, pictureSizeY, Layers.Count));

            ActiveLayerNom = 0;

            standartBrush = new MyBrush(3, false);
        }

        public void SetColor(Color newColor)
        {
            Layers[ActiveLayerNom].SetColor(newColor);
        }

        public void Drawing(int x, int y)
        {
            Layers[ActiveLayerNom].Draw(standartBrush, x, y);
        }

        // визуализация
        public void SwapImage()
        {
            // вызываем функцию визуализации в нашем слое
            for (int ax = 0; ax < Layers.Count; ax++)
            {
                // если этот слой является активным в данный момент
                if (ax == ActiveLayerNom)
                {
                    // вызываем визуализацию данного слоя напрямую
                    Layers[ax].RenderImage(false);
                }
                else
                {
                    // вызываем визуализацию слоя из дисплейного списка
                    Layers[ax].RenderImage(true);
                }
            }
        }

        // получение изображения для главного слоя
        public void SetImageToMainLayer(Bitmap layer)
        {
            // поворачиваем изображение (чтобы оно корректно отображалось в области редактирования)
            layer.RotateFlip(RotateFlipType.Rotate180FlipX);

            // проходим двумя циклами по всем пикселям изображения, подгруженного в класс Bitmap
            // получая цвет пикселя, устанавливаем его в текущий слой с помощью функции Drawing
            // данный алгоритм является медленным, но простым
            // оптимальным решением здесь было бы написание собственного загрузчика файлов изображений,
            // что дало бы возможность без "посредников" получать массив значений пикселей изображений,
            // но данная задача является намного более сложной и выходит за рамки обучающей программы

            for (int ax = 0; ax < layer.Width; ax++)
            {
                for (int bx = 0; bx < layer.Height; bx++)
                {
                    // получения цвета пикселя изображения
                    SetColor(layer.GetPixel(ax, bx));
                    // отрисовка данного пикселя в слое
                    Drawing(ax, bx);
                }
            }
        }

        // фильтр для инвертирования цветов
        public void Filter_0()
        {
            // вызываем функцию инвертирования класса anLayer
            Layers[ActiveLayerNom].Invers();
        }

        // увеличить резкость
        public void Filter_1()
        {
            // собираем матрицу
            float[] mat = new float[9]; mat[0] = -0.1f;
            mat[1] = -0.1f;
            mat[2] = -0.1f;
            mat[3] = -0.1f;
            mat[4] = 1.8f;
            mat[5] = -0.1f;
            mat[6] = -0.1f;
            mat[7] = -0.1f;
            mat[8] = -0.1f;

            //вызываем функцию обработки, передавая туда матрицу и дополнительные параметры
            Layers[ActiveLayerNom].PixelTransformation(mat, 0, 1, false);
        }

        // Размытие
        public void Filter_2()
        {
            // собираем матрицу
            float[] mat = new float[9];

            mat[0] = 0.05f;
            mat[1] = 0.05f;
            mat[2] = 0.05f;
            mat[3] = 0.05f;
            mat[4] = 0.6f;
            mat[5] = 0.05f;
            mat[6] = 0.05f;
            mat[7] = 0.05f;
            mat[8] = 0.05f;

            //вызываем функцию обработки , передавая туда матрицу и дополнительные параметры
            Layers[ActiveLayerNom].PixelTransformation(mat, 0, 1, false);
        }

        // Тиснение
        public void Tisnenie()
        {
            // собираем матрицу
            float[] mat = new float[9];

            mat[0] = -1.0f;
            mat[1] = -1.0f;
            mat[2] = -1.0f;
            mat[3] = -1.0f;
            mat[4] = 8.0f;
            mat[5] = -1.0f;
            mat[6] = -1.0f;
            mat[7] = -1.0f;
            mat[8] = -1.0f;

            //вызываем функцию обработки, передавая туда матрицу и дополнительные параметры
            Layers[ActiveLayerNom].PixelTransformation(mat, 0, 2, true);
        }

        // Акварелизация
        public void Filter_4()
        {
            // собираем матрицу
            // для данного фильтра нам необзодимо будет произвести два преобразования

            float[] mat = new float[9];

            mat[0] = 0.50f;
            mat[1] = 1.0f;
            mat[2] = 0.50f;
            mat[3] = 1.0f;
            mat[4] = 2.0f;
            mat[5] = 1.0f;
            mat[6] = 0.50f;
            mat[7] = 1.0f;
            mat[8] = 0.50f;

            //вызываем функцию обработки, передавая туда матрицу и дополнительные параметры
            Layers[ActiveLayerNom].PixelTransformation(mat, 0, 2, true);

            mat[0] = -0.5f;
            mat[1] = -0.5f;
            mat[2] = -0.5f;
            mat[3] = -0.5f;
            mat[4] = 6.0f;
            mat[5] = -0.5f;
            mat[6] = -0.5f;
            mat[7] = -0.5f;
            mat[8] = -0.5f;

            //вызываем функцию обработки, передавая туда матрицу и дополнительные параметры
            ((Layer)Layers[ActiveLayerNom]).PixelTransformation(mat, 0, 1, false);
        }
    }
}
