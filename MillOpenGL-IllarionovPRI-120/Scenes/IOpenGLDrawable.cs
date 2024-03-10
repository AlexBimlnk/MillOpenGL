using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillOpenGL_IllarionovPRI_120.Scenes
{
    public interface IOpenGLDrawable
    {
        void Draw(Action globalSceneMove = null);
    }
}
