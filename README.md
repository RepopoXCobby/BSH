# Tekla Model Connector Plugin

Tekla Structures에서 모델을 연결하고 관리하는 플러그인입니다.

## 주요 기능

- **모델 연결**: Tekla Structures 모델에 자동 연결
- **연결 상태 확인**: 현재 모델 연결 상태 표시
- **모델 정보 조회**: 모델 경로, 이름, 사용자, 부재 수 등 정보 표시
- **연결 해제**: 모델 연결 안전하게 해제

## 파일 구조

```
TeklaPlugin/
├── TeklaPlugin.cs          # 메인 플러그인 클래스
├── MainForm.cs             # 사용자 인터페이스 폼
├── TeklaPlugin.csproj      # 프로젝트 파일
├── ModelConnector.manifest # 플러그인 매니페스트
└── README.md               # 이 파일
```

## 핵심 코드 설명

### 1. 모델 연결 (TeklaPlugin.cs)

```csharp
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
```

### 2. 모델 정보 조회

```csharp
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
        
        // 정보 표시...
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"모델 정보 가져오기 오류: {ex.Message}");
    }
}
```

## 설치 및 사용법

### 1. 빌드

```bash
dotnet build TeklaPlugin.csproj
```

### 2. Tekla에 플러그인 등록

1. Tekla Structures 실행
2. 도구 > 매크로 > 매크로 관리자
3. 새 매크로 추가
4. 빌드된 DLL 파일 선택
5. 플러그인 활성화

### 3. 플러그인 실행

- Tekla에서 매크로 메뉴를 통해 실행
- 또는 단축키 설정 후 실행

## 요구사항

- .NET Framework 4.8
- Tekla Structures 2020 이상
- Windows Forms 지원

## 주의사항

1. **Tekla 경로 설정**: 프로젝트 파일에서 `TEKLA_STRUCTURES_PATH` 환경변수를 올바르게 설정해야 합니다.
2. **권한**: Tekla가 실행 중일 때만 플러그인이 작동합니다.
3. **모델 열기**: 플러그인 실행 전에 Tekla에서 모델을 열어야 합니다.

## 문제 해결

### 연결 실패 시
- Tekla Structures가 실행 중인지 확인
- 모델이 열려있는지 확인
- 플러그인 DLL이 올바른 위치에 있는지 확인

### 빌드 오류 시
- Tekla 참조 DLL 경로 확인
- .NET Framework 버전 확인
- Visual Studio 또는 .NET SDK 설치 확인

## 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다.

## 기여

버그 리포트나 기능 제안은 이슈로 등록해 주세요.
