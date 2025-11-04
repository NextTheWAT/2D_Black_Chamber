# 🎮 2D_Black_Chamber
## 탑다운 스텔스 잠입 액션 게임

![Unity](https://img.shields.io/badge/Unity-2022.3-000000?logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-11-239120?logo=csharp&logoColor=white)

> 완전한 스텔스 액션 게임 프로젝트입니다. 443개 스크립트와 60개 시스템 모듈로 구성된 상업적 품질의 게임입니다.

---

## 🎯 게임 소개

**STEALTH OPERATION**은 위에서 내려다보는 시점의 스텔스 액션 게임입니다. 플레이어는 암살 미션을 수행하고 안전하게 탈출해야 하며, 정교한 스텔스 메커니즘과 지능형 AI로 구성된 전술적 액션 게임입니다.

### 🎮 게임 플레이
- **미션 시스템**: 암살 → 탈출 → 성공/실패
- **스텔스 메커니즘**: 소음 기반 탐지, CCTV 감시망
- **AI 시스템**: 16개 상태 머신 기반 지능형 적 행동
- **무기 시스템**: 스텔스/전투 상황별 무기 전환

---

## 🏗️ 시스템 아키텍처

### 핵심 시스템
- **🎮 Player System** (17개 파일) - 정밀 컨트롤과 전투
- **🤖 Enemy AI** (20개 파일) - 16-State 머신 지능형 AI
- **🔫 Weapon System** (21개 파일) - 정교한 무기 메커니즘
- **🖥️ UI System** (32개 파일) - 완전한 사용자 인터페이스
- **🏪 Store System** (9개 파일) - 경제와 상점 시스템

### 기술 스택
- **Engine**: Unity 3D Engine
- **Language**: C# Scripting
- **Architecture**: Event-Driven, State Machine, Factory, Singleton
- **Patterns**: ScriptableObject, Object Pooling, Observer

---

## 🎬 게임 씬

### 메인 씬 구조
```
├── TitleScene       # 메인 메뉴
├── LobbyScene       # 로비 및 메뉴
├── TutorialScene    # 튜토리얼
├── GameScene        # 메인 게임플레이
├── StoreScene       # 상점 시스템
├── GameOverScene    # 게임 오버
└── ClearScene       # 미션 성공
```

### 게임 화면
- **🎮 메인 메뉴**: 게임 시작, 설정, 로딩
- **🎯 게임플레이**: 스텔스 미션 진행 화면
- **🛒 상점**: 무기 구매 및 업그레이드
- **📖 튜토리얼**: 게임 조작법 안내
- **🏆 미션 완료**: 성공 결과 화면

---

## 🛠️ 개발 정보

### 통계
- **총 스크립트**: 443개 파일
- **시스템 모듈**: 60개
- **주요 파일**: 111개 핵심 시스템
- **코드 라인**: 50,000+ 줄

### 주요 기술
- **Event-Driven Architecture** - 시스템 간 통신
- **Finite State Machine (FSM)** - AI 상태 관리
- **ScriptableObject** - 데이터 중심 설계
- **Object Pooling** - 성능 최적화
- **Coroutines** - 비동기 처리

### 성능 최적화
- Object Pooling으로 메모리 효율성 확보
- 코루틴을 활용한 비동기 처리
- Unity Profiler를 통한 지속적 최적화
- 60fps 목표 성능 달성

---

## 🚀 빠르게 시작하기

### 요구사항
- Unity 2022.3 이상
- C# 11.0 이상

### 설치 및 실행
1. 프로젝트 클론
```bash
git clone [repository-url]
```

2. Unity에서 프로젝트 열기
3. Scene 폴더에서 `TitleScene` 실행

### 프로젝트 구조
```
Scripts/
├── Core/          # 핵심 시스템
├── Player/        # 플레이어 시스템
├── Enemy/         # 적 AI 시스템
├── Weapon/        # 무기 시스템
├── Manager/       # 매니저 클래스
├── UI/           # 사용자 인터페이스
└── ScriptableObject/ # 데이터 오브젝트
```

---

## 🎯 핵심 기능

### 🧠 지능형 AI
- **16개 상태 머신**: Patrol, Investigate, Attack, Cover, Flee 등
- **성격별 AI**: Hiding, Bravery, Tactics, Coward 등 6가지 성격
- **협력적 AI**: 팀워크와 전술적 행동

### 🔍 스텔스 메커니즘
- **3단계 경계**: None → Suspicious → Alert
- **소음 기반 탐지**: 실시간 소음 추적 및 감지
- **CCTV 시스템**: 시각적 감시망 구현

### 🎮 정밀 컨트롤
- **이벤트 기반 입력**: 확장 가능한 입력 시스템
- **물리 기반 이동**: 자연스러운 움직임
- **정밀 조준**: 실시간 조준 시스템

### 🛡️ 무기 시스템
- **무기 슬롯**: 스텔스/전투 상황별 자동 전환
- **탄약 관리**: 탄창/예비탄 시스템
- **반동 시스템**: 현실적인 발사 피드백

---

## 📁 문서

자세한 정보는 다음 문서들을 참고하세요:
- **[게임 브로셔](게임_브로셔.pdf)** - 전체 프로젝트 개요
- **[사용 기술 리스트](사용_기술_리스트.pdf)** - 기술 스택 상세 정보
- **[개발 문서](docs/)** - 내부 개발 문서

---

## 🏆 프로젝트 성과

✅ **443개 스크립트** - 체계적이고 확장 가능한 코드베이스  
✅ **60개 모듈** - 모듈식 아키텍처 설계  
✅ **16-State AI** - 업계 수준의 지능형 인공지능  
✅ **최적화된 성능** - Object Pooling과 효율적 메모리 관리  
✅ **완성도 높은 UI** - 32개 파일의 포괄적 인터페이스  

**이 프로젝트는 게임 개발에서 요구되는 모든 핵심 기술을 포괄하는 포트폴리오입니다.**

---

## 📞 연락처

프로젝트 관련 문의나 피드백을 환영합니다!

---

*📅 개발 완료일: 2025년 11월*  
*🎯 Unity 스텔스 액션 게임 프로젝트*
