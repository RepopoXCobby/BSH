using System;
using System.Windows.Forms;
using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Plugins;
using Tekla.Structures.Geometry3d;

namespace TeklaPlugin
{
    [Plugin("ModelConnector")]
    [PluginUserInterface("ModelConnector.MainForm")]
    public class ModelConnector : PluginBase
    {
        private Model _model;
        private bool _isConnected = false;

        public ModelConnector()
        {
            _model = new Model();
        }

        public override bool Run(params string[] args)
        {
            try
            {
                // 모델 연결 시도
                if (ConnectToModel())
                {
                    MessageBox.Show("Tekla 모델에 성공적으로 연결되었습니다!", "연결 성공", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // 모델 정보 출력
                    DisplayModelInfo();
                    
                    return true;
                }
                else
                {
                    MessageBox.Show("Tekla 모델에 연결할 수 없습니다.", "연결 실패", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류가 발생했습니다: {ex.Message}", "오류", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Tekla 모델에 연결
        /// </summary>
        /// <returns>연결 성공 여부</returns>
        private bool ConnectToModel()
        {
            try
            {
                // 모델이 이미 열려있는지 확인
                if (_model.GetConnectionStatus())
                {
                    _isConnected = true;
                    return true;
                }

                // 모델 연결 시도
                if (_model.Connect())
                {
                    _isConnected = true;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"모델 연결 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 모델 정보 표시
        /// </summary>
        private void DisplayModelInfo()
        {
            if (!_isConnected) return;

            try
            {
                // 모델 정보 가져오기
                string modelPath = _model.GetInfo().ModelPath;
                string modelName = _model.GetInfo().Name;
                
                // 현재 사용자 정보
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
                System.Diagnostics.Debug.WriteLine($"모델 정보 가져오기 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 모델 연결 해제
        /// </summary>
        public void DisconnectFromModel()
        {
            if (_isConnected)
            {
                _model.Disconnect();
                _isConnected = false;
            }
        }

        /// <summary>
        /// 연결 상태 확인
        /// </summary>
        /// <returns>연결 상태</returns>
        public bool IsConnected()
        {
            return _isConnected && _model.GetConnectionStatus();
        }

        /// <summary>
        /// 모델 객체 반환
        /// </summary>
        /// <returns>연결된 모델 객체</returns>
        public Model GetModel()
        {
            return _isConnected ? _model : null;
        }
    }
}