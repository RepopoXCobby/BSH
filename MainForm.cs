using System;
using System.Drawing;
using System.Windows.Forms;
using Tekla.Structures.Model;

namespace TeklaPlugin
{
    public partial class MainForm : Form
    {
        private ModelConnector _modelConnector;
        private Model _model;

        public MainForm()
        {
            InitializeComponent();
            _modelConnector = new ModelConnector();
        }

        private void InitializeComponent()
        {
            this.btnConnect = new Button();
            this.btnDisconnect = new Button();
            this.btnModelInfo = new Button();
            this.lblStatus = new Label();
            this.groupBox1 = new GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();

            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.btnModelInfo);
            this.groupBox1.Controls.Add(this.btnDisconnect);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Location = new Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(300, 200);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Tekla 모델 연결";
            this.groupBox1.TabStop = false;

            // 
            // btnConnect
            // 
            this.btnConnect.Location = new Point(20, 30);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new Size(120, 30);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "모델 연결";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new EventHandler(this.btnConnect_Click);

            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new Point(160, 30);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new Size(120, 30);
            this.btnDisconnect.TabIndex = 1;
            this.btnDisconnect.Text = "연결 해제";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new EventHandler(this.btnDisconnect_Click);
            this.btnDisconnect.Enabled = false;

            // 
            // btnModelInfo
            // 
            this.btnModelInfo.Location = new Point(20, 80);
            this.btnModelInfo.Name = "btnModelInfo";
            this.btnModelInfo.Size = new Size(260, 30);
            this.btnModelInfo.TabIndex = 2;
            this.btnModelInfo.Text = "모델 정보 보기";
            this.btnModelInfo.UseVisualStyleBackColor = true;
            this.btnModelInfo.Click += new EventHandler(this.btnModelInfo_Click);
            this.btnModelInfo.Enabled = false;

            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(20, 130);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(100, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "상태: 연결 안됨";
            this.lblStatus.ForeColor = Color.Red;

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(324, 224);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Tekla 모델 연결기";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                
                if (_modelConnector.Run())
                {
                    _model = _modelConnector.GetModel();
                    UpdateUI(true);
                    lblStatus.Text = "상태: 연결됨";
                    lblStatus.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"연결 중 오류가 발생했습니다: {ex.Message}", "오류", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                _modelConnector.DisconnectFromModel();
                _model = null;
                UpdateUI(false);
                lblStatus.Text = "상태: 연결 안됨";
                lblStatus.ForeColor = Color.Red;
                
                MessageBox.Show("모델 연결이 해제되었습니다.", "연결 해제", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"연결 해제 중 오류가 발생했습니다: {ex.Message}", "오류", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModelInfo_Click(object sender, EventArgs e)
        {
            if (_model != null && _modelConnector.IsConnected())
            {
                try
                {
                    // 모델 정보 가져오기
                    string modelPath = _model.GetInfo().ModelPath;
                    string modelName = _model.GetInfo().Name;
                    string currentUser = _model.GetCurrentUser().Name;
                    
                    // 모델 통계 정보
                    int partCount = _model.GetObjects(new Type[] { typeof(Part) }).GetSize();
                    int beamCount = _model.GetObjects(new Type[] { typeof(Beam) }).GetSize();
                    int columnCount = _model.GetObjects(new Type[] { typeof(Column) }).GetSize();
                    
                    string info = $"모델 정보:\n" +
                                $"이름: {modelName}\n" +
                                $"경로: {modelPath}\n" +
                                $"현재 사용자: {currentUser}\n" +
                                $"부재 수: {partCount}\n" +
                                $"보 수: {beamCount}\n" +
                                $"기둥 수: {columnCount}";
                    
                    MessageBox.Show(info, "모델 정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"모델 정보를 가져올 수 없습니다: {ex.Message}", "오류", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateUI(bool isConnected)
        {
            btnConnect.Enabled = !isConnected;
            btnDisconnect.Enabled = isConnected;
            btnModelInfo.Enabled = isConnected;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_modelConnector != null && _modelConnector.IsConnected())
            {
                _modelConnector.DisconnectFromModel();
            }
            base.OnFormClosing(e);
        }
    }
}