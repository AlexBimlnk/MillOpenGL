using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillOpenGL_IllarionovPRI_120.Scenes.Main
{
    // класс ExportedObject отвечает за логические единицы 3D объектов в загружаемой сцене
    class ExportedObject
    {
        // при инициализации мы должны указать количество вершин (vertex) и полигонов (face), которые описывают геометрию подобъекта
        public ExportedObject(int a, int b)
        {
            if (temp[0] == 0)
                temp[0] = 1;

            // записываем количество вершин и полигонов
            VandF[0] = a;
            VandF[1] = b;

            // выделяем память
            memcompl();

        }

        public int Itog; // флаг успешности

        // массивы для хранения данных (геометрии и текстурных координат)
        public float[,] vert;
        public int[,] face;
        public float[,] t_vert;
        public int[,] t_face;

        // номер материала (текстуры) данного подобъекта
        public int MaterialNom = -1;

        // временное хранение информации
        public int[] VandF = new int[4];
        private int[] temp = new int[2];

        // флаг, говорящий о том, что модель использует текстуру
        private bool ModelHasTexture = false;

        // функция для определения значения флага (наличие текстуры)
        public bool NeedTexture()
        {
            // возвращаем значение флага
            return ModelHasTexture;
        }

        // массивы для текстурных координат
        public void createTextureVertexMem(int a)
        {
            VandF[2] = a;
            t_vert = new float[3, VandF[2]];
        }

        // привязка значений текстурных координат к полигонам
        public void createTextureFaceMem(int b)
        {
            VandF[3] = b;
            t_face = new int[3, VandF[3]];

            // отмечаем флаг наличия текстуры
            ModelHasTexture = true;
        }

        // память для геометрии
        private void memcompl()
        {
            vert = new float[3, VandF[0]];
            face = new int[3, VandF[1]];
        }

        // номер текстуры
        public int GetTextureNom()
        {
            return MaterialNom;
        }

        internal void SetMaterialNom(int mat_ref)
        {
            ModelHasTexture = true;
            MaterialNom = mat_ref;
        }
    }

}
