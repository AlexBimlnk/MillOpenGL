using MillOpenGL_IllarionovPRI_120.Scenes;
using MillOpenGL_IllarionovPRI_120.Scenes.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace MillOpenGL_IllarionovPRI_120
{
    public sealed class ErmitCurve : IOpenGLDrawable
    {
        private readonly List<Point> _corePoints;
        private readonly int _pointSize;

        private const decimal INTERPOLATION_STEP = 0.2m;

        public const int FIXED_Z_COORDINATE = 1;

        public IReadOnlyList<Point> InterpolatedPoints { get; }

        public ErmitCurve(
            IReadOnlyCollection<(double X, double Y)> corePoints,
            int pointSize = 5)
        {
            _corePoints = new List<Point>(corePoints.Select(t => new Point(t.X, t.Y)));
            InterpolatedPoints = Interpolation();
            _pointSize = pointSize;
        }

        public void Draw(Action globalSceneMove = null)
        {
            Gl.glPushMatrix();

            // устанавливаем размер точек равный 5
            Gl.glPointSize(_pointSize);

            Gl.glColor3f(255, 0, 0);

            Gl.glTranslated(0, 0, -15);
            //Gl.glScaled(0.8, 0.8, 1);

            // режим вывода геометрии - точки
            Gl.glBegin(Gl.GL_POINTS);


            foreach (var point in _corePoints)
            {
                Gl.glVertex3d(point.X, point.Y, FIXED_Z_COORDINATE);
            }

            Gl.glColor3f(0, 0, 0);

            foreach (var point in InterpolatedPoints)
            {
                Gl.glVertex3d(point.X, point.Y, FIXED_Z_COORDINATE);
            }

            Gl.glEnd();

            globalSceneMove?.Invoke();

            Gl.glPopMatrix();
            Gl.glFlush();
        }

        private List<Point> Interpolation()
        {
            var points = new List<Point>();

            for(int i = 0; i < _corePoints.Count - 1; i++)
            {
                Point? point1 = default;
                Point point2 = default;
                Point point3 = default;
                Point? point4 = default;

                if (i == 0)
                {
                    point1 = null;
                    point2 = _corePoints[i];
                    point3 = _corePoints[i + 1];
                    point4 = _corePoints[i + 2];
                }
                else if (i == _corePoints.Count - 2)
                {
                    point1 = _corePoints[i - 1];
                    point2 = _corePoints[i];
                    point3 = _corePoints[i + 1];
                    point4 = null;
                }
                else
                {
                    point1 = _corePoints[i - 1];
                    point2 = _corePoints[i];
                    point3 = _corePoints[i + 1];
                    point4 = _corePoints[i + 2];
                }

                points.AddRange(InterpolationOnInterval(point1, point2, point3, point4));
            }

            return points;
        }

        private List<Point> InterpolationOnInterval(
            Point? point1, Point point2, Point point3, Point? point4)
        {
            var points = new List<Point>();
            for (decimal i = 0; i <= 1; i += INTERPOLATION_STEP)
            {
                var x = InterpolatedP((double)i, point1, point2, point3, point4, true);
                var y = InterpolatedP((double)i, point1, point2, point3, point4, false);

                points.Add(new Point(x, y));
            }

            return points;
        }

        // pointOs means X or Y
        private double InterpolatedP(
            double t, 
            Point? point0, Point point1, Point point2, Point? point3,
            bool forX)
        {
            return  H00(t) * GetValueFromPoint(point1, forX) +
                    H10(t) * (point2.X - point1.X) * CalculateM(point0, point1, point2, forX) +
                    H01(t) * GetValueFromPoint(point2, forX) +
                    H11(t) * (point2.X - point1.X) * CalculateM(point1, point2, point3, forX);
        }
            
        private double CalculateM(Point? point1, Point point2, Point? point3, bool forX)
        {
            if (point1 == null && point3 == null)
                throw new InvalidOperationException();

            if (point1 == null)
            {
                return 0.5 *
                    (GetValueFromPoint(point3.Value, forX) - GetValueFromPoint(point2, forX)) /
                    (point3.Value.X - point2.X);
            }

            if (point3 == null)
            {
                return 0.5 *
                    (GetValueFromPoint(point2, forX) - GetValueFromPoint(point1.Value, forX)) /
                    (point2.X - point1.Value.X);
            }

            return 0.5 *
                    (
                        (GetValueFromPoint(point3.Value, forX) - GetValueFromPoint(point2, forX)) /
                        (point3.Value.X - point2.X)
                        +
                        (GetValueFromPoint(point2, forX) - GetValueFromPoint(point1.Value, forX)) /
                        (point2.X - point1.Value.X)
                    );
        }

        private double GetValueFromPoint(Point point, bool forX) =>
            forX ? point.X : point.Y;

        private double H00(double t) => 2 * Math.Pow(t, 3) - 3 * Math.Pow(t, 2) + 1;
        
        private double H10(double t) => Math.Pow(t, 3) - 2 * Math.Pow(t, 2) + t;

        private double H01(double t) => -2 * Math.Pow(t, 3) + 3 * Math.Pow(t, 2);

        private double H11(double t) => Math.Pow(t, 3) - Math.Pow(t, 2);
    }
}
