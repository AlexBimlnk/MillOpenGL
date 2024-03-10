using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace MillOpenGL_IllarionovPRI_120.Scenes.Main
{
    public sealed class ConeOnCurve : IOpenGLDrawable
    {
        private const int SLICES = 64;
        private double _angleRotation = 2 * Math.PI / SLICES;
        
        private double[,,] _resultGeometry;

        public ErmitCurve Curve { get; private set; }

        private bool _isInit = false;

        public void Draw(Action globalSceneMove = null)
        {
            InitIfNeed();

            //DrawAsPoints(globalSceneMove);
            //DrawAsLines(globalSceneMove);
            DrawAsPolygons(globalSceneMove);
            //Curve.Draw(globalSceneMove);
        }

        private void DrawAsPoints(Action globalSceneMove)
        {
            Gl.glPushMatrix();

            Gl.glPointSize(5);

            Gl.glTranslated(0, 0, -15);

            // режим вывода геометрии - точки
            Gl.glBegin(Gl.GL_POINTS);

            Gl.glColor3f(0, 0, 0);

            // выводим всю ранее просчитанную геометрию объекта
            for (int ax = 0; ax < Curve.InterpolatedPoints.Count; ax++)
            {
                for (int bx = 0; bx < SLICES; bx++)
                {
                    // отрисовка точки
                    Gl.glVertex3d(
                        _resultGeometry[ax, bx, 0],
                        _resultGeometry[ax, bx, 1],
                        _resultGeometry[ax, bx, 2]);
                }

            }
            // завершаем режим рисования
            Gl.glEnd();

            globalSceneMove?.Invoke();

            Gl.glPopMatrix();
            Gl.glFlush();
        }

        private void DrawAsLines(Action globalSceneMove)
        {
            Gl.glPushMatrix();

            Gl.glPointSize(5);

            Gl.glTranslated(0, 0, -15);

            // устанавливаем режим отрисвки линиями (последовательность линий)
            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int ax = 0; ax < Curve.InterpolatedPoints.Count - 1; ax++)
            {

                for (int bx = 0; bx < SLICES; bx++)
                {


                    Gl.glVertex3d(
                        _resultGeometry[ax, bx, 0],
                        _resultGeometry[ax, bx, 1], 
                        _resultGeometry[ax, bx, 2]);
                    Gl.glVertex3d(
                        _resultGeometry[ax + 1, bx, 0],
                        _resultGeometry[ax + 1, bx, 1],
                        _resultGeometry[ax + 1, bx, 2]);

                    if (bx + 1 < SLICES - 1)
                    {

                        Gl.glVertex3d(
                            _resultGeometry[ax + 1, bx + 1, 0],
                            _resultGeometry[ax + 1, bx + 1, 1],
                            _resultGeometry[ax + 1, bx + 1, 2]);

                    }
                    else
                    {

                        Gl.glVertex3d(
                            _resultGeometry[ax + 1, 0, 0],
                            _resultGeometry[ax + 1, 0, 1],
                            _resultGeometry[ax + 1, 0, 2]);

                    }

                }

            }
            Gl.glEnd();

            globalSceneMove?.Invoke();

            Gl.glPopMatrix();
            Gl.glFlush();
        }

        private void DrawAsPolygons(Action globalSceneMove)
        {
            Gl.glPushMatrix();

            Gl.glPointSize(5);

            Gl.glColor3ub(150, 75, 0);

            Gl.glTranslated(0, 0, -15);

            Gl.glBegin(Gl.GL_QUADS); // режим отрисовки полигонов, состоящих из 4 вершин
            for (int ax = 0; ax < Curve.InterpolatedPoints.Count; ax++)
            {

                for (int bx = 0; bx < SLICES; bx++)
                {

                    // вспомогательные переменные для более наглядного использования кода при расчете нормалей
                    double
                        x1 = 0, x2 = 0, x3 = 0, x4 = 0,
                        y1 = 0, y2 = 0, y3 = 0, y4 = 0,
                        z1 = 0, z2 = 0, z3 = 0, z4 = 0;

                    // первая вершина
                    x1 = _resultGeometry[ax, bx, 0];
                    y1 = _resultGeometry[ax, bx, 1];
                    z1 = _resultGeometry[ax, bx, 2];

                    if (ax + 1 < Curve.InterpolatedPoints.Count) // если текущий ax не последний
                    {

                        // берем следующую точку последовательности
                        x2 = _resultGeometry[ax + 1, bx, 0];
                        y2 = _resultGeometry[ax + 1, bx, 1];
                        z2 = _resultGeometry[ax + 1, bx, 2];

                        if (bx + 1 < SLICES - 1) // если текущий bx не последний
                        {

                            // берем следующую точку последовательности и следующий меридиан
                            x3 = _resultGeometry[ax + 1, bx + 1, 0];
                            y3 = _resultGeometry[ax + 1, bx + 1, 1];
                            z3 = _resultGeometry[ax + 1, bx + 1, 2];

                            // точка, соотвествующуя по номеру только на соседнем меридиане
                            x4 = _resultGeometry[ax, bx + 1, 0];
                            y4 = _resultGeometry[ax, bx + 1, 1];
                            z4 = _resultGeometry[ax, bx + 1, 2];

                        }
                        else
                        {

                            // если это последний меридиан, то в качестве следующего мы берем начальный (замыкаем геометрию фигуры)
                            x3 = _resultGeometry[ax + 1, 0, 0];
                            y3 = _resultGeometry[ax + 1, 0, 1];
                            z3 = _resultGeometry[ax + 1, 0, 2];

                            x4 = _resultGeometry[ax, 0, 0];
                            y4 = _resultGeometry[ax, 0, 1];
                            z4 = _resultGeometry[ax, 0, 2];

                        }

                    }
                    else // данный элемент ax последний, следовательно мы будем использовать начальный (нулевой) вместо данного ax
                    {

                        // слудуещей точкой будет нулевая ax
                        x2 = _resultGeometry[0, bx, 0];
                        y2 = _resultGeometry[0, bx, 1];
                        z2 = _resultGeometry[0, bx, 2];


                        if (bx + 1 < SLICES - 1)
                        {

                            x3 = _resultGeometry[0, bx + 1, 0];
                            y3 = _resultGeometry[0, bx + 1, 1];
                            z3 = _resultGeometry[0, bx + 1, 2];

                            x4 = _resultGeometry[ax, bx + 1, 0];
                            y4 = _resultGeometry[ax, bx + 1, 1];
                            z4 = _resultGeometry[ax, bx + 1, 2];

                        }
                        else
                        {

                            x3 = _resultGeometry[0, 0, 0];
                            y3 = _resultGeometry[0, 0, 1];
                            z3 = _resultGeometry[0, 0, 2];

                            x4 = _resultGeometry[ax, 0, 0];
                            y4 = _resultGeometry[ax, 0, 1];
                            z4 = _resultGeometry[ax, 0, 2];

                        }

                    }


                    // переменные для расчета нормали
                    double n1 = 0, n2 = 0, n3 = 0;

                    // нормаль будем расчитывать как векторное произведение граней полигона
                    // для нулевого элемента нормаль мы будем считать немного по-другому

                    // на самом деле разница в расчете нормали актуальна только для последнего и первого полигона на меридиане

                    if (ax == 0) // при расчете нормали для ax мы будем использовать точки 1,2,3
                    {

                        n1 = (y2 - y1) * (z3 - z1) - (y3 - y1) * (z2 - z1);
                        n2 = (z2 - z1) * (x3 - x1) - (z3 - z1) * (x2 - x1);
                        n3 = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);

                    }
                    else // для остальных - 1,3,4
                    {

                        n1 = (y4 - y3) * (z1 - z3) - (y1 - y3) * (z4 - z3);
                        n2 = (z4 - z3) * (x1 - x3) - (z1 - z3) * (x4 - x3);
                        n3 = (x4 - x3) * (y1 - y3) - (x1 - x3) * (y4 - y3);

                    }


                    // если не включен режим GL_NORMILIZE, то мы должны в обязательном порядке
                    // произвести нормализацию вектора нормали, перед тем как передать информацию о нормали
                    double n5 = (double)Math.Sqrt(n1 * n1 + n2 * n2 + n3 * n3);
                    n1 /= (n5 + 0.01);
                    n2 /= (n5 + 0.01);
                    n3 /= (n5 + 0.01);

                    // передаем информацию о нормали
                    Gl.glNormal3d(n1, n2, n3);

                    // передаем 4 вершины для отрисовки полигона
                    Gl.glVertex3d(x1, y1, z1);
                    Gl.glVertex3d(x2, y2, z2);
                    Gl.glVertex3d(x3, y3, z3);
                    Gl.glVertex3d(x4, y4, z4);

                }

            }

            // завершаем выбранный режим рисования полигонов
            Gl.glEnd();

            globalSceneMove?.Invoke();

            Gl.glPopMatrix();
            Gl.glFlush();
        }

        private void InitIfNeed()
        {
            if (_isInit)
            {
                return;
            }

            var control = new List<(double, double)>()
            {
                //(-3.5, -4),
                //(-2, -3),
                //(-1.5, -2),
                //(-1, -1),
                (0, 0),
                (1, -1),
                (1.5, -2),
                (2, -3),
                //(3.5, -4),
            };

            Curve = new ErmitCurve(control);

            _resultGeometry = new double[Curve.InterpolatedPoints.Count, SLICES, 3];

            // цикл по последовательности точек кривой, на основе которой будет построено тело вращения
            for (int ax = 0; ax < Curve.InterpolatedPoints.Count; ax++)
            {

                // цикл по меридианам объекта, заранее определенным в программе
                for (int bx = 0; bx < SLICES; bx++)
                {

                    // для всех (bx > 0) элементов алгоритма используются предыдушая построенная последовательность
                    // для ее поворота на установленный угол
                    if (bx > 0)
                    {
                        double new_x =
                            _resultGeometry[ax, bx - 1, 0] * Math.Cos(_angleRotation) -
                            _resultGeometry[ax, bx - 1, 2] * Math.Sin(_angleRotation);

                        double new_y = _resultGeometry[ax, bx - 1, 1];

                        double new_z =
                            _resultGeometry[ax, bx - 1, 0] * Math.Sin(_angleRotation) +
                            _resultGeometry[ax, bx - 1, 2] * Math.Cos(_angleRotation);

                        _resultGeometry[ax, bx, 0] = new_x;
                        _resultGeometry[ax, bx, 1] = new_y;
                        _resultGeometry[ax, bx, 2] = new_z;

                    }
                    else // для построения первого меридиана мы используем начальную кривую, описывая ее нулевым значением угла поворота
                    {

                        double new_x = 
                            Curve.InterpolatedPoints[ax].X * Math.Cos(0) - 
                            ErmitCurve.FIXED_Z_COORDINATE * Math.Sin(0);

                        double new_y = Curve.InterpolatedPoints[ax].Y;

                        double new_z =
                            Curve.InterpolatedPoints[ax].X * Math.Sin(0) +
                            ErmitCurve.FIXED_Z_COORDINATE * Math.Cos(0);
                        
                        _resultGeometry[ax, bx, 0] = new_x;
                        _resultGeometry[ax, bx, 1] = new_y;
                        _resultGeometry[ax, bx, 2] = new_z;

                    }

                }

            }
        }
    }
}
