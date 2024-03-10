using MillOpenGL_IllarionovPRI_120.Scenes;
using MillOpenGL_IllarionovPRI_120.Scenes.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace MillOpenGL_IllarionovPRI_120
{
    public sealed class Mill : IOpenGLDrawable
    {
        public const int DEFAULT_MILL_ROTATION = 6;

        private readonly ConeOnCurve _coneOnCurve = new ConeOnCurve();

        private int _rotateAngle = DEFAULT_MILL_ROTATION;
        private int _sheduledRotationAngle = DEFAULT_MILL_ROTATION;

        private int _rotateIteration = 0;

        public bool IsNeedCurveDraw { get; set; }
        public bool IsNeedDefaultConeDraw { get; set; }

        public int ActualRatotationSpeed => _rotateAngle;

        public int SheduledRotationSpeed 
        { 
            get { return _sheduledRotationAngle; }
            set
            {
                if (value < 0)
                    _sheduledRotationAngle = 0;
                else if (value > 10)
                    _sheduledRotationAngle = 10;
                else
                    _sheduledRotationAngle = value;

                Debug.WriteLine($"Sheduled rotation speed changed: {_sheduledRotationAngle}");
            }
        }

        public void Draw(Action globalSceneMove = null)
        {
            if (_rotateAngle == 0)
            {
                _rotateIteration = 0;
                _rotateAngle = _sheduledRotationAngle;
            }
            else if (_rotateIteration == 90 / _rotateAngle)
            {
                _rotateIteration = 0;
                _rotateAngle = _sheduledRotationAngle;
            }

            DrawMillCone(globalSceneMove);
            DrawMillCube(globalSceneMove);
            DrawMillCube2(globalSceneMove);

            _rotateIteration++;
        }

        private void DrawMillCone(Action globalSceneMove)
        {
            if (IsNeedDefaultConeDraw)
                DrawOpenGlCone(globalSceneMove);
            else
                _coneOnCurve.Draw(globalSceneMove);

            if (IsNeedCurveDraw)
            {
                _coneOnCurve.Curve.Draw(globalSceneMove);
            }

            //DrawOpenGlCone(globalSceneMove);
            //DrawInterpolatedLine(globalSceneMove);
        }

        private void DrawMillCube(Action globalSceneMove)
        {
            Gl.glPushMatrix();

            Gl.glColor3ub(205, 154, 123);

            Gl.glRotated(_rotateAngle * _rotateIteration, 0, 0, 1);

            Gl.glScaled(0.3, 2, 1);

            Gl.glTranslated(0, 0, -8);

            globalSceneMove?.Invoke();

            Glut.glutSolidCube(1);
            

            Gl.glPopMatrix();
            Gl.glFlush();
        }

        private void DrawMillCube2(Action globalSceneMove)
        {
            Gl.glPushMatrix();

            Gl.glColor3ub(205, 154, 123);

            Gl.glRotated(_rotateAngle * _rotateIteration + 90, 0, 0, 1);

            Gl.glScaled(0.3, 2, 1);

            Gl.glTranslated(0, 0, -8);

            globalSceneMove?.Invoke();

            Glut.glutSolidCube(1);


            Gl.glPopMatrix();
            Gl.glFlush();
        }

        private void DrawOpenGlCone(Action globalSceneMove)
        {
            Gl.glPushMatrix();

            Gl.glColor3ub(150, 75, 0);

            Gl.glTranslated(0, -3, -15);
            Gl.glRotated(-90, 1, 0, 0);

            Glut.glutSolidCone(2, 3, 10, 10);

            globalSceneMove?.Invoke();

            Gl.glPopMatrix();
            Gl.glFlush();
        }

        private void DrawInterpolatedLine(Action globalSceneMove)
        {
            var control = new List<(double, double)>()
            {
                //(-3.5, -4),
                (-2, -3),
                (-1.5, -2),
                (-1, -1),
                (0, 0),
                (1, -1),
                (1.5, -2),
                (2, -3),
                //(3.5, -4),
            };

            var ermit = new ErmitCurve(control);

            ermit.Draw(globalSceneMove);
        }
    }
}
