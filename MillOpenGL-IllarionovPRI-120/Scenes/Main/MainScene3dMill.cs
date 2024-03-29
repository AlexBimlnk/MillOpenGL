﻿using MillOpenGL_IllarionovPRI_120.ParticalSystem;
using MillOpenGL_IllarionovPRI_120.Scenes;
using MillOpenGL_IllarionovPRI_120.Scenes.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.DevIl;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MillOpenGL_IllarionovPRI_120
{
    public sealed class MainScene3dMill : IOpenGLDrawable
    {
        private readonly TextureLoader _textureLoader = new TextureLoader();
        private readonly ModelLoader _modelLoader = new ModelLoader();
        
        private readonly Dictionary<int, TextureLoader> _clouds = Enumerable.Range(1, 6)
            .Select(x => (x, new TextureLoader()))
            .ToDictionary(k => k.x, v => v.Item2);

        private IEnumerator<uint> _cloudsEnumerator;

        private readonly int _width;
        private readonly int _height;

        private bool _isInit;

        public float GlobalTime {  get; set; }

        public Mill Mill { get; } = new Mill();

        public Explosion Explosion { get; } = new Explosion(0, 0, 0, 300, 500);

        public ModelLoader Telega => _modelLoader;

        public MainScene3dMill(int w, int h)
        {
            _width = w;
            _height = h;
        }

        public void Init()
        {
            // инициализация режима экрана
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);

            // установка цвета очистки экрана (RGBA)
            Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода
            Gl.glViewport(0, 0, _width, _height);

            // активация проекционной матрицы
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // очистка матрицы
            Gl.glLoadIdentity();

            // установка перспективы
            Glu.gluPerspective(45, _width / _height, 0.1, 200);

            // установка объектно-видовой матрицы
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            // начальные настройки OpenGL
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);

            _textureLoader.LoadTextureForModel("background.jpg");

            foreach(var cloud in _clouds)
            {
                cloud.Value.LoadTextureForModel($"cloud{cloud.Key}.png");
            }

            _cloudsEnumerator = CloudIterator().GetEnumerator();

            _modelLoader.LoadModel("telega.ase");

            _isInit = true;
        }

        public void Draw(Action globalSceneMove = null)
        {
            if (!_isInit)
            {
                throw new InvalidOperationException();
            }

            // очистка буфера цвета и буфера глубины
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glClearColor(255, 255, 255, 1);
            // очищение текущей матрицы
            Gl.glLoadIdentity();

            DrawExplosion();

            DrawBackground();

            DrawClouds();

            globalSceneMove?.Invoke();

            Mill.Draw(globalSceneMove);

            _modelLoader.Draw(globalSceneMove);
        }

        private void DrawExplosion()
        {
            Gl.glPushMatrix();
            Gl.glTranslated(0, 0, -15);

            Explosion.Calculate(GlobalTime);

            // возвращаем состояние матрицы
            Gl.glPopMatrix();

            // завершаем рисование
            Gl.glFlush();
        }

        private void DrawBackground()
        {
            Gl.glPushMatrix();
            // включаем режим текстурирования
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            // включаем режим текстурирования, указывая идентификатор mGlTextureObject
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, _textureLoader.GetTextureObj());

            // сохраняем состояние матрицы
            Gl.glPushMatrix();

            Gl.glTranslated(0, 0, -55);
            Gl.glRotated(-90, 0, 0, 1);

            Gl.glClear(Gl.GL_4D_COLOR_TEXTURE);

            // отрисовываем полигон
            Gl.glBegin(Gl.GL_QUADS);

            // указываем поочередно вершины и текстурные координаты
            Gl.glVertex3d(25, 25, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(25, -25, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(-25, -25, 0);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(-25, 25, 0);
            Gl.glTexCoord2f(1, 0);

            // завершаем отрисовку
            Gl.glEnd();

            // возвращаем состояние матрицы
            Gl.glPopMatrix();

            // завершаем рисование
            Gl.glFlush();

            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        private void DrawClouds()
        {
            Gl.glPushMatrix();
            // включаем режим текстурирования
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            // включаем режим текстурирования, указывая идентификатор mGlTextureObject
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, _cloudsEnumerator.Current);

            _cloudsEnumerator.MoveNext();

            // сохраняем состояние матрицы
            Gl.glPushMatrix();

            Gl.glTranslated(15, 15, -50);
            Gl.glRotated(-90, 0, 0, 1);
            Gl.glScaled(0.1, 0.1, 0.1);

            Gl.glClear(Gl.GL_4D_COLOR_TEXTURE);

            // отрисовываем полигон
            Gl.glBegin(Gl.GL_QUADS);

            // указываем поочередно вершины и текстурные координаты
            Gl.glVertex3d(25, 25, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(25, -25, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(-25, -25, 0);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(-25, 25, 0);
            Gl.glTexCoord2f(1, 0);

            // завершаем отрисовку
            Gl.glEnd();

            // возвращаем состояние матрицы
            Gl.glPopMatrix();

            // завершаем рисование
            Gl.glFlush();

            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        private IEnumerable<uint> CloudIterator()
        {
            while (true)
            {
                foreach (var cloud in _clouds)
                {
                    yield return cloud.Value.GetTextureObj();
                }
            }
        }
    }
}
