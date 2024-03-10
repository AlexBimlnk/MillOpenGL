using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.DevIl;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace MillOpenGL_IllarionovPRI_120
{
    public partial class Form1 : Form
    {
        private const string WAIT_ROLLBACK = "Ждите возрат телеги";
        private const string CAN_MOVE_TELEGA = "Вы можете передвигать тележку";

        private MainScene3dMill _mainScene3dMill;
        private Scene2dImage _scene2dImage;

        private System.Windows.Media.MediaPlayer _millAudioPlayer = new System.Windows.Media.MediaPlayer();
        private System.Windows.Media.MediaPlayer _refreshAudioPlayer = new System.Windows.Media.MediaPlayer();

        private readonly SoundPlayer _millSoundPlayer = new SoundPlayer("sounds\\millloop.wav");
        private readonly SoundPlayer _refreshSoundPlayer = new SoundPlayer("sounds\\millrefresh.wav");
        private readonly SoundPlayer _bgAudio = new SoundPlayer("sounds\\Terrain-Grass.wav");

        private bool _isMainScene = true;
        private bool _isCollision;

        private int _globalRotation = 0;

        public Form1()
        {
            InitializeComponent();
            openGLControl.InitializeContexts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация бибилиотеки glut
            Glut.glutInit();
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);

            _mainScene3dMill = new MainScene3dMill(openGLControl.Width, openGLControl.Height);
            _scene2dImage = new Scene2dImage(openGLControl.Width, openGLControl.Height);

            // По умолчанию запускаем главную сцену
            _mainScene3dMill.Init();
            renderTimer.Start();


            _refreshAudioPlayer.Open(new Uri("sounds\\millrefresh.wav", UriKind.Relative));
            _refreshAudioPlayer.MediaEnded += AudioResetHandler;
            _refreshAudioPlayer.Volume = 0.3;

            _millAudioPlayer.Open(new Uri("sounds\\millloop.wav", UriKind.Relative));
            _millAudioPlayer.MediaEnded += AudioLoopHandler;
            _millAudioPlayer.Volume = 0.2;
            _millAudioPlayer.Play();
            
            //_millSoundPlayer.PlayLooping();
        }

        private void AudioLoopHandler(object sender, EventArgs e)
        {
            var audioPlayer = sender as System.Windows.Media.MediaPlayer;

            audioPlayer.Position = TimeSpan.FromMilliseconds(1);
        }

        private void AudioResetHandler(object sender, EventArgs e)
        {
            var audioPlayer = sender as System.Windows.Media.MediaPlayer;
            audioPlayer.Stop();
            audioPlayer.Position = TimeSpan.FromMilliseconds(1);
        }

        private void renderTimer_Tick(object sender, EventArgs e)
        {
            if (_isMainScene)
            {
                _mainScene3dMill.Draw(GlobalSceneActionMove);
                UpdateProgressBar();
                CalculateCollision();
            }
            else
                _scene2dImage.Draw();

            openGLControl.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_isMainScene)
            {
                _isMainScene = true;
                _mainScene3dMill.Init();

                btnSwitchScene.BackColor = Color.White;

                btnResetGlobalRotation.Enabled = true;
                btnAddParticals.Enabled = true;
                checkErmitDraw.Enabled = true;
                checkConeMode.Enabled = true;

                btnApplyFilter.Enabled = false;
            }
            else
            {
                _isMainScene = false;
                _scene2dImage.Init();

                btnSwitchScene.BackColor = Color.Green;

                btnApplyFilter.Enabled = true;

                btnResetGlobalRotation.Enabled = false;
                btnAddParticals.Enabled = false;
                checkErmitDraw.Enabled = false;
                checkConeMode.Enabled = false;
            }

            openGLControl.Focus();
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            _scene2dImage.ApplyFilter();
            openGLControl.Focus();
        }

        private void UpdateProgressBar()
        {
            if (progressBar1.Value - 2 <= progressBar1.Minimum)
            {
                progressBar1.Value = progressBar1.Minimum;

                if (_mainScene3dMill.Mill.ActualRatotationSpeed > 0)
                {
                    _mainScene3dMill.Mill.SheduledRotationSpeed = 0;
                    //_millSoundPlayer.Stop();
                    _millAudioPlayer.Stop();
                }

                return;
            }

            if (_mainScene3dMill.Mill.ActualRatotationSpeed > 0)
            {
                progressBar1.Value -= 2;
                if (progressBar1.Value % 200 == 0 && progressBar1.Value != progressBar1.Maximum)
                {
                    _mainScene3dMill.Mill.SheduledRotationSpeed -= 1;
                }
            }
        }

        private void CalculateCollision()
        {
            var telega = _mainScene3dMill.Telega;
            if (telega.TelegaShag == telega.MaxShag)
            {
                Debug.WriteLine("COLLISION");
                _isCollision = true;
                labelTelegaStatus.Text = WAIT_ROLLBACK;
                labelTelegaStatus.ForeColor = Color.Red;

                _mainScene3dMill.Mill.SheduledRotationSpeed = Mill.DEFAULT_MILL_ROTATION;
                progressBar1.Value = progressBar1.Maximum;
                //_millSoundPlayer.PlayLooping();
                _millAudioPlayer.Play();
                _refreshAudioPlayer.Play();
            }

            if (_isCollision)
                telega.TelegaShag -= 1;

            if (telega.TelegaShag == 0)
            {
                Debug.WriteLine("ROLLBACK IS DONE");
                _isCollision = false;
                labelTelegaStatus.Text = CAN_MOVE_TELEGA;
                labelTelegaStatus.ForeColor = Color.Green;
            }
        }

        private void GlobalSceneActionMove()
        {
            Gl.glRotated(_globalRotation, 0, 1, 0);
            //Gl.glTranslated(0, 0.5, 0);
            //Gl.glTranslated(5, 0, 0);
            //Gl.glTranslated(0, 0, -20);
        }

        private void openGLControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {
                _globalRotation += 2;
                if (_globalRotation > 16)
                    _globalRotation = 16;
            }
            else if (e.KeyCode == Keys.E)
            {
                _globalRotation -= 2;
                if (_globalRotation < -16)
                    _globalRotation = -16;
            }
            else if (e.KeyCode == Keys.W && progressBar1.Value != progressBar1.Minimum)
            {
                _mainScene3dMill.Mill.SheduledRotationSpeed += 1;
            }
            else if (e.KeyCode == Keys.S)
            {
                _mainScene3dMill.Mill.SheduledRotationSpeed -= 1;
            }
            else if (e.KeyCode == Keys.A && !_isCollision)
            {
                _mainScene3dMill.Telega.TelegaShag -= 1;
            }
            else if (e.KeyCode == Keys.D && !_isCollision)
            {
                _mainScene3dMill.Telega.TelegaShag += 1;
            }
        }

        private void btnResetGlobalRotation_Click(object sender, EventArgs e)
        {
            _globalRotation = 0;
            openGLControl.Focus();
        }

        private void checkErmitDraw_CheckedChanged(object sender, EventArgs e)
        {
            _mainScene3dMill.Mill.IsNeedCurveDraw = ((CheckBox)sender).Checked;

            openGLControl.Focus();
        }

        private void checkConeMode_CheckedChanged(object sender, EventArgs e)
        {
            _mainScene3dMill.Mill.IsNeedDefaultConeDraw = ((CheckBox)sender).Checked;

            openGLControl.Focus();
        }
    }
}
