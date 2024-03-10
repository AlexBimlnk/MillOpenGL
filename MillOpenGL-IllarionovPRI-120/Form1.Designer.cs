namespace MillOpenGL_IllarionovPRI_120
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openGLControl = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.renderTimer = new System.Windows.Forms.Timer(this.components);
            this.btnSwitchScene = new System.Windows.Forms.Button();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.btnAddParticals = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnResetGlobalRotation = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkErmitDraw = new System.Windows.Forms.CheckBox();
            this.checkConeMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // openGLControl
            // 
            this.openGLControl.AccumBits = ((byte)(0));
            this.openGLControl.AutoCheckErrors = false;
            this.openGLControl.AutoFinish = false;
            this.openGLControl.AutoMakeCurrent = true;
            this.openGLControl.AutoSwapBuffers = true;
            this.openGLControl.BackColor = System.Drawing.Color.Black;
            this.openGLControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("openGLControl.BackgroundImage")));
            this.openGLControl.ColorBits = ((byte)(32));
            this.openGLControl.DepthBits = ((byte)(16));
            this.openGLControl.Location = new System.Drawing.Point(12, 12);
            this.openGLControl.Name = "openGLControl";
            this.openGLControl.Size = new System.Drawing.Size(700, 500);
            this.openGLControl.StencilBits = ((byte)(0));
            this.openGLControl.TabIndex = 0;
            this.openGLControl.VSync = false;
            this.openGLControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.openGLControl_KeyDown);
            // 
            // renderTimer
            // 
            this.renderTimer.Interval = 50;
            this.renderTimer.Tick += new System.EventHandler(this.renderTimer_Tick);
            // 
            // btnSwitchScene
            // 
            this.btnSwitchScene.BackColor = System.Drawing.Color.White;
            this.btnSwitchScene.Location = new System.Drawing.Point(718, 12);
            this.btnSwitchScene.Name = "btnSwitchScene";
            this.btnSwitchScene.Size = new System.Drawing.Size(122, 38);
            this.btnSwitchScene.TabIndex = 1;
            this.btnSwitchScene.Text = "Нарисовать фрактал";
            this.btnSwitchScene.UseVisualStyleBackColor = false;
            this.btnSwitchScene.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Enabled = false;
            this.btnApplyFilter.Location = new System.Drawing.Point(718, 56);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(122, 29);
            this.btnApplyFilter.TabIndex = 2;
            this.btnApplyFilter.Text = "Применить тиснение";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApplyFilter_Click);
            // 
            // btnAddParticals
            // 
            this.btnAddParticals.Location = new System.Drawing.Point(718, 91);
            this.btnAddParticals.Name = "btnAddParticals";
            this.btnAddParticals.Size = new System.Drawing.Size(122, 29);
            this.btnAddParticals.TabIndex = 3;
            this.btnAddParticals.Text = "Включить дождь";
            this.btnAddParticals.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(718, 217);
            this.progressBar1.Maximum = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(230, 23);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Value = 1000;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(718, 189);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "Наполнение мельницы:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnResetGlobalRotation
            // 
            this.btnResetGlobalRotation.Location = new System.Drawing.Point(722, 384);
            this.btnResetGlobalRotation.Name = "btnResetGlobalRotation";
            this.btnResetGlobalRotation.Size = new System.Drawing.Size(122, 36);
            this.btnResetGlobalRotation.TabIndex = 6;
            this.btnResetGlobalRotation.Text = "Централизовать";
            this.btnResetGlobalRotation.UseVisualStyleBackColor = true;
            this.btnResetGlobalRotation.Click += new System.EventHandler(this.btnResetGlobalRotation_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(718, 243);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(177, 54);
            this.label2.TabIndex = 7;
            this.label2.Text = "Используйте Q/E для поворта";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(718, 306);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(177, 54);
            this.label3.TabIndex = 8;
            this.label3.Text = "Используйте W/S для лопастей";
            // 
            // checkErmitDraw
            // 
            this.checkErmitDraw.AutoSize = true;
            this.checkErmitDraw.Location = new System.Drawing.Point(722, 437);
            this.checkErmitDraw.Name = "checkErmitDraw";
            this.checkErmitDraw.Size = new System.Drawing.Size(126, 17);
            this.checkErmitDraw.TabIndex = 9;
            this.checkErmitDraw.Text = "Отобразить кривую";
            this.checkErmitDraw.UseVisualStyleBackColor = true;
            this.checkErmitDraw.CheckedChanged += new System.EventHandler(this.checkErmitDraw_CheckedChanged);
            // 
            // checkConeMode
            // 
            this.checkConeMode.AutoSize = true;
            this.checkConeMode.Location = new System.Drawing.Point(722, 460);
            this.checkConeMode.Name = "checkConeMode";
            this.checkConeMode.Size = new System.Drawing.Size(99, 17);
            this.checkConeMode.TabIndex = 10;
            this.checkConeMode.Text = "Конус OpenGL";
            this.checkConeMode.UseVisualStyleBackColor = true;
            this.checkConeMode.CheckedChanged += new System.EventHandler(this.checkConeMode_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 521);
            this.Controls.Add(this.checkConeMode);
            this.Controls.Add(this.checkErmitDraw);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnResetGlobalRotation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnAddParticals);
            this.Controls.Add(this.btnApplyFilter);
            this.Controls.Add(this.btnSwitchScene);
            this.Controls.Add(this.openGLControl);
            this.Name = "Form1";
            this.Text = "КП \"Мельница\" Илларионов АМ ПРИ-120";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl openGLControl;
        private System.Windows.Forms.Timer renderTimer;
        private System.Windows.Forms.Button btnSwitchScene;
        private System.Windows.Forms.Button btnApplyFilter;
        private System.Windows.Forms.Button btnAddParticals;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnResetGlobalRotation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkErmitDraw;
        private System.Windows.Forms.CheckBox checkConeMode;
    }
}

