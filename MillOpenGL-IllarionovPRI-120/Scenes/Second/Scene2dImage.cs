using MillOpenGL_IllarionovPRI_120.Properties;
using System;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace MillOpenGL_IllarionovPRI_120
{
    public sealed class Scene2dImage
    {
        private PifagorFractal _fractal;

        private readonly int _width;
        private readonly int _height;

        private bool _isInit;

        public Scene2dImage(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void Init()
        {
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            // очитка окна
            Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода в соотвествии с размерами элемента openGLControl
            Gl.glViewport(0, 0, _width, _height);

            // настройка проекции
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Glu.gluOrtho2D(0, _width, 0, _height);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);

            Gl.glDisable(Gl.GL_COLOR_MATERIAL);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_LIGHT0);

            // Загружаем фрактал заново
            LoadFractal();

            _isInit = true;
        }

        public void Draw()
        {
            if (!_isInit)
            {
                throw new InvalidOperationException();
            }

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glLoadIdentity();

            Gl.glColor3f(0, 0, 0);

            _fractal.SwapImage();

            Gl.glFlush();
        }

        public void ApplyFilter()
        {
            if (!_isInit)
            {
                throw new InvalidOperationException();
            }

            _fractal.Tisnenie();
        }

        private void LoadFractal()
        {
            // создаем новый экземпляр класса anEngine
            _fractal = new PifagorFractal(_width, _height);

            // копируем изображение в нижний левый угол рабочей области
            _fractal.SetImageToMainLayer(Resources.fractalMin_fotor);
        }
    }
}
