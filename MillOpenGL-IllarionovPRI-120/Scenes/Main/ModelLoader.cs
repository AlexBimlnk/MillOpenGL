using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace MillOpenGL_IllarionovPRI_120.Scenes.Main
{
    public class ModelLoader : IOpenGLDrawable
    {
        private const double TELEGA_TRANSLATED_KOEF = 0.025;

        // загружен ли (флаг)
        private bool isLoad = false;
        // счетчик подобъектов
        private int count_ExportedObjects;
        // переменная для хранения номера текстуры
        private int mat_nom = 0;

        // номер дисплейного списка с данной моделью
        private int thisList = 0;

        // данная переменная будет указывать на количество прочитанных символов в строке при чтении информации из файла
        private int GlobalStringFrom = 0;

        // массив подобъектов
        ExportedObject[] ExportedObjects = null;

        // массив для хранения текстур
        TextureLoader[] text_objects = null;

        // описание ориентации модели
        //Model_Prop coord = new Model_Prop();

        private int _telegaShag = 0;

        public int MaxShag => 100;

        public int TelegaShag
        {
            get { return _telegaShag; }
            set
            {
                if (value <= 0)
                    _telegaShag = 0;
                else if (value > MaxShag)
                    _telegaShag = MaxShag;
                else
                    _telegaShag = value;
            }
        }

        // загрузка модели
        public int LoadModel(string modelFileName)
        {
            // модель может содержать до 2048 подобъектов
            ExportedObjects = new ExportedObject[204800];
            // счетчик скинут
            int ExportedObject_ = -1;

            // начинаем чтение файла
            StreamReader sw = File.OpenText($"models\\{modelFileName}");

            // временные буферы
            string a_buff = "";
            string b_buff = "";
            string c_buff = "";

            // счетчики вершин и полигонов
            int ver = 0, fac = 0;

            // если строка успешно прочитана
            while ((a_buff = sw.ReadLine()) != null)
            {
                // получаем первое слово
                b_buff = GetFirstWord(a_buff, 0);
                if (b_buff[0] == '*') // определеям, является ли первый символ звездочкой
                {
                    switch (b_buff) // если да, то проверяем какое управляющее слово содержится в первом прочитаном слове
                    {
                        case "*MATERIAL_COUNT": // счетчик материалов
                            {
                                // получаем первое слово от символа, указанного в GlobalStringFrom
                                c_buff = GetFirstWord(a_buff, GlobalStringFrom);
                                int mat = System.Convert.ToInt32(c_buff);

                                // создаем объект для текстуры в памяти
                                text_objects = new TextureLoader[mat];
                                continue;
                            }

                        case "*MATERIAL_REF": // номер текстуры
                            {
                                // записываем для текущего подобъекта номер текстуры
                                c_buff = GetFirstWord(a_buff, GlobalStringFrom);
                                int mat_ref = System.Convert.ToInt32(c_buff);

                                // устанавливаем номер материала, соответствующий данной модели
                                ExportedObjects[ExportedObject_].SetMaterialNom(mat_ref);
                                continue;
                            }

                        case "*MATERIAL": // указание на материал
                            {
                                c_buff = GetFirstWord(a_buff, GlobalStringFrom);
                                mat_nom = System.Convert.ToInt32(c_buff);
                                continue;
                            }

                        case "*GEOMOBJECT": // начинается описание геометрии подобъекта
                            {
                                ExportedObject_++; // записываем в счетчик подобъектов
                                continue;
                            }

                        case "*MESH_NUMVERTEX": // количесвто вершин в подобъекте
                            {
                                c_buff = GetFirstWord(a_buff, GlobalStringFrom);
                                ver = System.Convert.ToInt32(c_buff);
                                continue;
                            }

                        case "*BITMAP": // имя текстуры
                            {
                                c_buff = ""; // обнуляем временный буффер

                                for (int ax = GlobalStringFrom + 2; ax < a_buff.Length - 1; ax++)
                                    c_buff += a_buff[ax]; // считываем имя текстуры

                                text_objects[mat_nom] = new TextureLoader(); // новый объект для текстуры

                                text_objects[mat_nom].LoadTextureForModel(c_buff); // загружаем текстуру

                                continue;
                            }

                        case "*MESH_NUMTVERTEX": // количество текстурных координат; данное слово говорит о наличии текстурных координат, следовательно мы должны выделить память для них
                            {

                                c_buff = GetFirstWord(a_buff, GlobalStringFrom);
                                if (ExportedObjects[ExportedObject_] != null)
                                {
                                    ExportedObjects[ExportedObject_].createTextureVertexMem(System.Convert.ToInt32(c_buff));
                                }
                                continue;
                            }

                        case "*MESH_NUMTVFACES": // память для текстурных координат (faces)
                            {
                                c_buff = GetFirstWord(a_buff, GlobalStringFrom);

                                if (ExportedObjects[ExportedObject_] != null)
                                {
                                    // выделяем память для текстурных координат
                                    ExportedObjects[ExportedObject_].createTextureFaceMem(System.Convert.ToInt32(c_buff));
                                }
                                continue;
                            }

                        case "*MESH_NUMFACES": // количество полигонов в подобъекте
                            {
                                c_buff = GetFirstWord(a_buff, GlobalStringFrom);
                                fac = System.Convert.ToInt32(c_buff);

                                // если было объвляющее слово *GEOMOBJECT (гарантия выполнения условия ExportedObject_ > -1) и было указано количство вершин
                                if (ExportedObject_ > -1 && ver > -1 && fac > -1)
                                {
                                    // создаем новый подобъект в памяти
                                    ExportedObjects[ExportedObject_] = new ExportedObject(ver, fac);
                                }
                                else
                                {
                                    // иначе завершаем неудачей
                                    return -1;
                                }
                                continue;
                            }

                        case "*MESH_VERTEX": // информация о вершине
                            {
                                // подобъект создан в памяти
                                if (ExportedObject_ == -1)
                                    return -2;
                                if (ExportedObjects[ExportedObject_] == null)
                                    return -3;

                                string a1 = "", a2 = "", a3 = "", a4 = "";

                                // получаем информацию о кооринатах и номере вершины
                                // (получаем все слова в строке)
                                a1 = GetFirstWord(a_buff, GlobalStringFrom);
                                a2 = GetFirstWord(a_buff, GlobalStringFrom);
                                a3 = GetFirstWord(a_buff, GlobalStringFrom);
                                a4 = GetFirstWord(a_buff, GlobalStringFrom);

                                // преобразовываем в целое число
                                int NomVertex = System.Convert.ToInt32(a1);

                                // заменяем точки в представлении числа с плавающей точкой на запятые, чтобы правильно выполнилась функция
                                // преобразования строки в дробное число
                                a2 = a2.Replace('.', ',');
                                a3 = a3.Replace('.', ',');
                                a4 = a4.Replace('.', ',');

                                // записываем информацию о вершине
                                ExportedObjects[ExportedObject_].vert[0, NomVertex] = (float)System.Convert.ToDouble(a2); // x
                                ExportedObjects[ExportedObject_].vert[1, NomVertex] = (float)System.Convert.ToDouble(a3); // y
                                ExportedObjects[ExportedObject_].vert[2, NomVertex] = (float)System.Convert.ToDouble(a4); // z

                                continue;
                            }

                        case "*MESH_FACE": // информация о полигоне
                            {
                                // подобъект создан в памяти
                                if (ExportedObject_ == -1)
                                    return -2;
                                if (ExportedObjects[ExportedObject_] == null)
                                    return -3;

                                // временные перменные
                                string a1 = "", a2 = "", a3 = "", a4 = "", a5 = "", a6 = "", a7 = "";

                                // получаем все слова в строке
                                a1 = GetFirstWord(a_buff, GlobalStringFrom);
                                a2 = GetFirstWord(a_buff, GlobalStringFrom);
                                a3 = GetFirstWord(a_buff, GlobalStringFrom);
                                a4 = GetFirstWord(a_buff, GlobalStringFrom);
                                a5 = GetFirstWord(a_buff, GlobalStringFrom);
                                a6 = GetFirstWord(a_buff, GlobalStringFrom);
                                a7 = GetFirstWord(a_buff, GlobalStringFrom);

                                // получаем номер полигона из первого слова в строке, заменив последний символ ":" после номера на флаг окончания строки
                                int NomFace = System.Convert.ToInt32(a1.Replace(':', '\0'));

                                // записываем номера вершин, которые нас интересуют
                                ExportedObjects[ExportedObject_].face[0, NomFace] = System.Convert.ToInt32(a3);
                                ExportedObjects[ExportedObject_].face[1, NomFace] = System.Convert.ToInt32(a5);
                                ExportedObjects[ExportedObject_].face[2, NomFace] = System.Convert.ToInt32(a7);

                                continue;

                            }

                        // текстурые координаты
                        case "*MESH_TVERT":
                            {
                                // подобъект создан в памяти
                                if (ExportedObject_ == -1)
                                    return -2;
                                if (ExportedObjects[ExportedObject_] == null)
                                    return -3;

                                // временные перменные
                                string a1 = "", a2 = "", a3 = "", a4 = "";

                                // получаем все слова в строке
                                a1 = GetFirstWord(a_buff, GlobalStringFrom);
                                a2 = GetFirstWord(a_buff, GlobalStringFrom);
                                a3 = GetFirstWord(a_buff, GlobalStringFrom);
                                a4 = GetFirstWord(a_buff, GlobalStringFrom);

                                // преобразуем первое слово в номер вершины
                                int NomVertex = System.Convert.ToInt32(a1);

                                // заменяем точки в представлении числа с плавающей точкой на запятые, чтобы правильно выполнилась функция
                                // преобразование строки в дробное число
                                a2 = a2.Replace('.', ',');
                                a3 = a3.Replace('.', ',');
                                a4 = a4.Replace('.', ',');

                                // записываем значение вершины
                                ExportedObjects[ExportedObject_].t_vert[0, NomVertex] = (float)System.Convert.ToDouble(a2); // x
                                ExportedObjects[ExportedObject_].t_vert[1, NomVertex] = (float)System.Convert.ToDouble(a3); // y
                                ExportedObjects[ExportedObject_].t_vert[2, NomVertex] = (float)System.Convert.ToDouble(a4); // z

                                continue;

                            }

                        // привязка текстурных координат к полигонам
                        case "*MESH_TFACE":
                            {
                                // подобъект создан в памяти
                                if (ExportedObject_ == -1)
                                    return -2;
                                if (ExportedObjects[ExportedObject_] == null)
                                    return -3;

                                // временные перменные
                                string a1 = "", a2 = "", a3 = "", a4 = "";

                                // получаем все слова в строке
                                a1 = GetFirstWord(a_buff, GlobalStringFrom);
                                a2 = GetFirstWord(a_buff, GlobalStringFrom);
                                a3 = GetFirstWord(a_buff, GlobalStringFrom);
                                a4 = GetFirstWord(a_buff, GlobalStringFrom);

                                // преобразуем первое слово в номер полигона
                                int NomFace = System.Convert.ToInt32(a1);

                                // записываем номера вершин, которые опиывают полигон
                                ExportedObjects[ExportedObject_].t_face[0, NomFace] = System.Convert.ToInt32(a2);
                                ExportedObjects[ExportedObject_].t_face[1, NomFace] = System.Convert.ToInt32(a3);
                                ExportedObjects[ExportedObject_].t_face[2, NomFace] = System.Convert.ToInt32(a4);

                                continue;

                            }

                    }

                }

            }
            // пересохраняем количество полигонов
            count_ExportedObjects = ExportedObject_;


            // получаем ID для создаваемого дисплейного списка
            int nom_l = Gl.glGenLists(1);
            thisList = nom_l;
            // генерируем новый дисплейный список
            Gl.glNewList(nom_l, Gl.GL_COMPILE);
            // отрисовываем геометрию
            CreateList();
            // завершаем дисплейный список
            Gl.glEndList();

            // загрузка завершена
            isLoad = true;

            return 0;
        }

        // функция отрисовки
        private void CreateList()
        {
            // сохраняем текущую матрицу

            Gl.glPushMatrix();

            // проходим циклом по всем подобъектам
            for (int l = 0; l <= count_ExportedObjects; l++)
            {

                if (ExportedObjects[l] is null)
                {
                    continue;
                }

                // если текстура необходима
                if (ExportedObjects[l].NeedTexture())
                {
                    if (ExportedObjects[l].GetTextureNom() < 1)
                    {
                        continue;
                    }
                    if (text_objects[ExportedObjects[l].GetTextureNom()] != null) // текстурный объект существует
                    {
                        Gl.glEnable(Gl.GL_TEXTURE_2D); // включаем режим текстурирования

                        // ID текстуры в памяти
                        uint nn = text_objects[ExportedObjects[l].GetTextureNom()].GetTextureObj();
                        // активируем (привязываем) эту текстуру
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, nn);
                    }

                }

                Gl.glEnable(Gl.GL_NORMALIZE);

                // начинаем отрисовку полигонов
                Gl.glBegin(Gl.GL_TRIANGLES);

                // по всем полигонам
                for (int i = 0; i < ExportedObjects[l].VandF[1]; i++)
                {
                    // временные переменные, чтобы код был более понятен
                    float x1, x2, x3, y1, y2, y3, z1, z2, z3 = 0;

                    // вытаскиваем координаты треугольника (полигона)
                    x1 = ExportedObjects[l].vert[0, ExportedObjects[l].face[0, i]];
                    x2 = ExportedObjects[l].vert[0, ExportedObjects[l].face[1, i]];
                    x3 = ExportedObjects[l].vert[0, ExportedObjects[l].face[2, i]];
                    y1 = ExportedObjects[l].vert[1, ExportedObjects[l].face[0, i]];
                    y2 = ExportedObjects[l].vert[1, ExportedObjects[l].face[1, i]];
                    y3 = ExportedObjects[l].vert[1, ExportedObjects[l].face[2, i]];
                    z1 = ExportedObjects[l].vert[2, ExportedObjects[l].face[0, i]];
                    z2 = ExportedObjects[l].vert[2, ExportedObjects[l].face[1, i]];
                    z3 = ExportedObjects[l].vert[2, ExportedObjects[l].face[2, i]];

                    // рассчитываем номраль
                    float n1 = (y2 - y1) * (z3 - z1) - (y3 - y1) * (z2 - z1);
                    float n2 = (z2 - z1) * (x3 - x1) - (z3 - z1) * (x2 - x1);
                    float n3 = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);

                    // устанавливаем номраль
                    Gl.glNormal3f(n1, n2, n3);

                    // если установлена текстура
                    if (ExportedObjects[l].NeedTexture() && (ExportedObjects[l].t_vert != null) && (ExportedObjects[l].t_face != null))
                    {
                        // устанавливаем текстурные координаты для каждой вершины и сами вершины
                        Gl.glTexCoord2f(ExportedObjects[l].t_vert[0, ExportedObjects[l].t_face[0, i]], ExportedObjects[l].t_vert[1, ExportedObjects[l].t_face[0, i]]);
                        Gl.glVertex3f(x1, y1, z1);

                        Gl.glTexCoord2f(ExportedObjects[l].t_vert[0, ExportedObjects[l].t_face[1, i]], ExportedObjects[l].t_vert[1, ExportedObjects[l].t_face[1, i]]);
                        Gl.glVertex3f(x2, y2, z2);

                        Gl.glTexCoord2f(ExportedObjects[l].t_vert[0, ExportedObjects[l].t_face[2, i]], ExportedObjects[l].t_vert[1, ExportedObjects[l].t_face[2, i]]);
                        Gl.glVertex3f(x3, y3, z3);

                    }
                    else // иначе - отрисовка только вершин
                    {
                        Gl.glVertex3f(x1, y1, z1);
                        Gl.glVertex3f(x2, y2, z2);
                        Gl.glVertex3f(x3, y3, z3);
                    }

                }

                // завершаем отрисовку
                Gl.glEnd();
                Gl.glDisable(Gl.GL_NORMALIZE);

                // отключаем текстурирование
                Gl.glDisable(Gl.GL_TEXTURE_2D);

            }

            // возвращаем сохраненную ранее матрицу
            Gl.glPopMatrix();
        }

        // функция получения первого слова строки
        private string GetFirstWord(string word, int from)
        {
            // from указывает на позицию, начиная с которой будет выполнятся чтение файла
            char a = word[from]; // первый символ
            string res_buff = ""; // временный буффер
            int L = word.Length; // длина слова

            if (word[from] == ' ' || word[from] == '\t') // если первый символ, с которого предстоит искать слово, является пробелом или знаком табуляции
            {
                // необходимо вычислить наличие секции пробелов или знаков табуляции и исключить их
                int ax = 0;
                // проходим до конца слова
                for (ax = from; ax < L; ax++)
                {
                    a = word[ax];
                    if (a != ' ' && a != '\t') // если встречаем символ пробела или табуляции
                        break; // выходим из цикла
                               // таким образом, мы исключаем все последовательности пробелов или знаков табуляции, с которых могла начинатся переданная строка
                }

                if (ax == L) // если вся представленная строка является набором пробелов или знаков табуляции - возвращаем res_buff
                    return res_buff;
                else
                    from = ax; // иначе сохраняем значение ax
            }
            int bx = 0;

            // теперь, когда пробелы и табуляция исключены, мы непосредственно вычисляем слово
            for (bx = from; bx < L; bx++)
            {
                // если встретили знак пробела или табуляции, завершаем чтение слова
                if (word[bx] == ' ' || word[bx] == '\t')
                    break;
                // записываем символ во временный буффер, постепенно получая таким образом слово
                res_buff += word[bx];
            }

            // если дошли до конца строки
            if (bx == L)
                bx--; // убираем последнее значение

            GlobalStringFrom = bx; // позиция в данной строке для чтения следующего слова в данной строке

            return res_buff; // возвращаем слово
        }

        // функция отрисовки 3D-модели
        public void Draw(Action globalActionMove)
        {
            // если модель не загружена, возврат из функции
            if (!isLoad)
                return;

            // сохраняем матрицу
            Gl.glPushMatrix();

            Gl.glTranslated(-1.5, -2.2, -6);

            Gl.glRotated(-45, 0, 1, 0);

            // масштабирование по умолчанию
            Gl.glScalef(0.3f, 0.3f, 0.3f);

            MoveTelega();

            // вызов дисплейного списка

            Gl.glCallList(thisList);

            globalActionMove?.Invoke();

            // возврат матрицы
            Gl.glPopMatrix();
            Gl.glFlush();
        }

        private void MoveTelega()
        {
            Gl.glTranslated(
                TELEGA_TRANSLATED_KOEF * TelegaShag,
                TELEGA_TRANSLATED_KOEF * TelegaShag,
                -TELEGA_TRANSLATED_KOEF * TelegaShag);

            if (TelegaShag > 0)
            {
                var newScale = 1 - TelegaShag / 120d;

                // Восстанавливаем предыдущий размер
                if (TelegaShag > 1)
                {
                    var previosScaleKof = 1 - (TelegaShag - 1) / 120d;

                    Gl.glScaled(
                        1 + 1 - previosScaleKof,
                        1 + 1 - previosScaleKof,
                        1 + 1 - previosScaleKof);
                }

                Debug.WriteLine($"Scale kof: {1 - TelegaShag / 120d}");
                Gl.glScaled(
                    newScale,
                    newScale,
                    newScale);
            }
        }
    }

}
