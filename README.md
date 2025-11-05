# 🎮 2D_Black_Chamber
<img width="2560" height="1440" alt="BlackChamberTitle" src="https://github.com/user-attachments/assets/6a1607a9-fa5e-4d19-9b57-0aaf6e22c934" />

## 🎮 게임소개
2D 탑다운 시점의 스텔스 액션 게임입니다.  
플레이어는 적들에게 들키지 않고 미션을 수행하거나, 상황에 따라 전투 모드로 전환하여 정면 돌파할 수 있습니다.  
스텔스와 액션의 균형을 통해 다양한 플레이 스타일을 지원하며, **Unity Analytics**로 플레이어 행동 패턴을 분석하여 게임 디자인을 개선했습니다.

**핵심 메커니즘**:  
🕵️ **스텔스 모드**: 적의 시야를 피해 조용히 이동, 백어택으로 무력화  
⚔️ **전투 모드**: 발각 시 또는 의도적으로 정면 교전  
🔄 **모드 전환**: 상황에 따른 전략적 플레이스타일 변경  
📊 **데이터 분석**: Unity Analytics로 플레이어 행동 패턴 실시간 추적

## ▶️ 플레이영상
https://youtu.be/OVt4j6ZTV6E

## 🕹️ 플레이방법  
게임 시작 → 튜토리얼 진행 → 스테이지 1/2/3 순차 도전  

### 입력 (Unity Input System)  
**이동**: WASD 키 또는 방향키  
**스텔스 모드**: 기본 상태, 조용한 이동과 은신  
**전투 모드**: 자동 또는 수동 전환, 빠른 이동과 공격  
**상호작용**: E키 (문 열기, 아이템 획득 등)  
**공격**: 마우스 클릭 (전투 모드) 또는 백어택 (스텔스 모드)  

## ✨ Features   

### 🎯 스텔스 시스템  
적 시야각(FOV) 기반 탐지 시스템    
소음 레벨에 따른 적 반응 변화    
은신 지점 활용 및 백어택 메커니즘    
스텔스 게이지를 통한 시각적 피드백    

### ⚔️ 전투 시스템    
실시간 모드 전환으로 플레이스타일 다양화  
무기별 차별화된 공격 패턴  
적 AI의 스텔스/전투 모드별 행동 변화  
전투 시 동적 UI 전환  

### 🏗️ 스테이지 시스템    
3개의 차별화된 스테이지 (튜토리얼 + 메인 스테이지)  
각 스테이지별 고유한 레벨 디자인과 적 배치  
진행률 추적 및 체크포인트 시스템  

### 🎵 사운드 시스템   
환경음, 발걸음 소리 등 몰입도 향상 사운드  
스텔스/전투 모드별 BGM 전환  
적 탐지 상태에 따른 동적 사운드 변화  

### 🎨 시각적 피드백   
적 시야각 시각화 (디버그 모드)  
모드 전환 시 화면 효과 및 UI 변화  
스텔스 상태 표시를 위한 투명도 조절  

## 🎯 시스템 기획

### 미션 시스템
- **Assassination Phase**: 목표 제거 단계 - 스텔스 침투와 정밀 타격
- **Escape Phase**: 도주 단계 - 현장 이탈과 안전 지대 도달  
- **Mission Complete**: 성공적인 미션 완료와 보상 획득

### 스텔스 메커니즘
- 🔇 **소음 기반 탐지 시스템**: NoiseManager를 통한 실시간 소음 추적
- 👁️ **CCTV 감시망**: 239줄의 정교한 CCTV 시스템 구현
- ⚠️ **적 경고 상태**: Alert(경계) → Suspicious(의심) → Investigation(조사) 단계별 반응
- 🎭 **은신/전투 모드**: 상황에 따른 스텔스/전투 무기 자동 전환

### 🤖 인공지능 시스템
- **Patrol**: 정찰 순찰 패턴
- **Investigate**: 의심스러운 상황 조사
- **Attack**: 공격적 교전 모드
- **Cover**: 엄폐 전술 수행
- **Retreat**: 전술적 후퇴
- **Flee**: 도주 행동

### 🧠 AI 성격 시스템
- **Hiding (은신형)**: 조심스럽고 은밀한 행동 패턴
- **Tactics (전술형)**: 계획적이고 체계적인 대응
- **Bravery (용감형)**: 적극적이고 대담한 추격
- **Coward (겁쟁이형)**: 소극적이고 방어적인 반응

## 🛠️ 핵심 기술 스택

## ⚙️ 아키텍처 패턴

### **Event-Driven Architecture (이벤트 기반 아키텍처)**

**선택 이유:**
- **시스템 분리**: 각 시스템이 독립적으로 동작하여 유지보수성 향상
- **확장성**: 새로운 기능 추가 시 기존 코드 수정 최소화
- **테스트 용이성**: 각 컴포넌트를 독립적으로 테스트 가능

```csharp
// Player/Controller/TopDownController.cs
public class TopDownController : MonoBehaviour
{
    public event Action<Vector2> OnMoveEvent;
    public event Action<Vector2> OnLookEvent;

    public void CallMoveEvent(Vector2 direction)
        => OnMoveEvent?.Invoke(direction);
    public void CallLookEvent(Vector2 direction)
        => OnLookEvent?.Invoke(direction);
}
```

- 시스템 간 통신에 이벤트 기반 아키텍처 적용
- OnMoveEvent, OnLookEvent 등을 통한 느슨한 결합 구현
- 시스템 분리와 확장성 확보로 유지보수성 향상

### **State Machine Pattern (상태 기계 패턴)**

**선택 이유:**
- **복잡한 AI 관리**: 16가지 적 행동 상태를 체계적으로 관리
- **디버깅 용이성**: 현재 상태를 명확히 추적 가능
- **행동 예측성**: 각 상태의 전이 조건이 명확하여 게임 밸런스 조정 용이

```csharp
// Enemy/State/FSM/StateMachineFactory.cs 일부
public StateMachine CreateStateMachine(StateMachineType type, Enemy enemy)
{
    switch(type)
    {
        case StateMachineType.NonCombat:
            return new NonCombatStateMachine(enemy,
                CreateNonCombatStates(enemy));
        case StateMachineType.Combat:
            return new CombatStateMachine(enemy,
                CreateCombatStates(enemy));
    }
}
```

- AI 상태 관리에 유한 상태 머신 패턴 적용
- 16개 상태 기반 적 AI 행동 제어 (Patrol, Investigate, Attack, Cover 등)
- NonCombat ↔ Combat 상태 머신 동적 전환

### **ScriptableObject 패턴**

**선택 이유:**
- **데이터 중심 설계**: 코드와 데이터 분리로 기획자도 쉽게 조정 가능
- **메모리 효율성**: 같은 데이터를 여러 객체가 공유
- **모듈식 콘텐츠**: 무기, 아이템 등을 독립된 에셋으로 관리

```csharp
// ScriptableObject/Weapon/GunData.cs
[CreateAssetMenu(fileName = "New Gun Data", menuName = "Weapon/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName;
    public int damage;
    public float fireRate;

    [Header("탄약 시스템")]
    public int magazineSize;
    public int reserveAmmo;

    [Header("정확도")]
    public float baseSpread;
    public float recoilForce;
}
```

- 게임 데이터 관리의 핵심 패턴
- GunData, WeaponCatalog 등 무기 데이터 모듈화
- 데이터 중심 설계로 기획자도 쉽게 조정 가능

### **Singleton Pattern**

**선택 이유:**
- **전역 접근**: 매니저 클래스들에 어디서든 접근 가능
- **상태 보존**: 게임 전체에서 하나의 인스턴스로 상태 일관성 유지
- **리소스 절약**: 중복 인스턴스 생성 방지

```csharp
// Core/Singleton.cs
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    instance = singleton.AddComponent<T>();
                }
            }
            return instance;
        }
    }
}
```

- 매니저 클래스 설계에 싱글톤 패턴 적용
- GameManager, ObjectPoolingManager 등 전역 접근 제공

### **Factory Pattern**

**선택 이유:**
- **객체 생성 캡슐화**: 복잡한 객체 생성 로직을 한 곳에 집중
- **확장성**: 새로운 타입 추가 시 팩토리만 수정하면 됨
- **타입 안전성**: 컴파일 시점에 타입 검증 가능
- **코드 재사용**: 동일한 생성 패턴을 여러 곳에서 활용

```csharp
// Enemy/State/FSM/StateMachineFactory.cs
public class StateMachineFactory
{
    public StateMachine CreateStateMachine(StateMachineType type, Enemy enemy)
    {
        switch(type)
        {
            case StateMachineType.NonCombat:
                return new NonCombatStateMachine(enemy,
                    CreateNonCombatStates(enemy));

            case StateMachineType.Combat:
                return new CombatStateMachine(enemy,
                    CreateCombatStates(enemy));

            default:
                throw new ArgumentException($"Unknown StateMachine type: {type}");
        }
    }

    private Dictionary<StateType, State> CreateNonCombatStates(Enemy enemy)
    {
        return new Dictionary<StateType, State>
        {
            { StateType.Patrol, new PatrolState(enemy) },
            { StateType.Investigate, new InvestigateState(enemy) },
            { StateType.Suspect, new SuspectState(enemy) },
            { StateType.Return, new ReturnState(enemy) }
        };
    }

    private Dictionary<StateType, State> CreateCombatStates(Enemy enemy)
    {
        // 적의 전투 성격에 따른 상태 생성
        CombatStateType combatType = enemy.enemyData.combatStateType;

        var states = new Dictionary<StateType, State>
        {
            { StateType.Attack, new AttackState(enemy) },
            { StateType.Cover, new CoverState(enemy) },
            { StateType.Retreat, new RetreatState(enemy) },
            { StateType.Flee, new FleeState(enemy) }
        };

        // 성격별 특수 상태 추가
        switch(combatType)
        {
            case CombatStateType.Hiding:
                states.Add(StateType.Hide, new HideState(enemy));
                break;
            case CombatStateType.Bravery:
                states.Add(StateType.Assault, new AssaultState(enemy));
                break;
            case CombatStateType.Tactics:
                states.Add(StateType.Flank, new FlankState(enemy));
                break;
        }

        return states;
    }
}
```

- 객체 생성 관리를 위한 팩토리 패턴 적용
- StateMachineFactory를 통한 동적 상태 머신 생성

### **Observer Pattern**
- 이벤트 구독/발행 시스템 구현
- 컴포넌트 간 느슨한 결합과 확장 가능한 이벤트 시스템 제공

## ⚙️ 게임 시스템

### **Unity Input System**
- 입력 처리 시스템
- 이벤트 기반 입력 처리로 확장성과 유지보수성 향상

### **Unity Audio System**
- 사운드 관리 시스템
- NoiseManager를 통한 정교한 소음 기반 스텔스 메커니즘 구현

## ⚙️ AI & 게임플레이 시스템

### **Finite State Machine (FSM)**
- 적 AI 행동 제어의 핵심 시스템
- NonCombat ↔ Combat 상태 머신 동적 전환
- CombatStateType에 따른 성격별 차별화된 행동 패턴
  - Hiding(은신), Discretion(신중), Tactics(전술), Bravery(용감), Temerity(무모), Coward(겁쟁이)

### **Behavior Trees**
- 복잡한 AI 의사결정 시스템
- 상황별 최적 판단과 적응적 행동 구현

### **Pathfinding**
- 적 이동 경로 계산 시스템
- 지능적인 순찰과 추적 경로 생성

### **Detection System**
- 시각/청각 기반 감지 시스템
- 소음 임계값(investigateThreshold = 20f) 기반 탐지
- 거리 기반 확률적 감지로 자연스러운 AI 반응 구현

## ⚙️ 성능 최적화

### **Object Pooling**
- 메모리 관리 최적화의 핵심 기술
- 가비지 컬렉션 최소화와 프레임 드롭 방지

### **Coroutines**
- 비동기 처리를 위한 Unity 코루틴 활용
- DelayedInvestigation 등 시간 기반 AI 행동 제어
- UI 애니메이션과 타이머 기반 시스템 구현

### **Culling System**
- 렌더링 최적화 시스템
- 불필요한 렌더링 제거로 성능 향상

## ⚙️ 데이터 관리

### **Firebase Integration**
- 클라우드 연동 시스템
- 유저 데이터 동기화와 온라인 기능 확장성

## ⚙️ UI & UX

### **Unity UI (uGUI)**
- 사용자 인터페이스 구현
- 32개 파일로 구성된 포괄적 UI 시스템
- 게임 HUD, 상점, 로비, 튜토리얼 등 전체 UI 담당

### **Canvas System**
- UI 렌더링 시스템
- 상황별 UI 전환 (스텔스 ↔ 전투 모드)
- 3초 전투 알림 후 자동 복귀 등 동적 UI 관리

### **Animation System**
- UI 애니메이션 구현
- 부드러운 화면 전환과 사용자 피드백 제공

### **Event System**
- UI 상호작용 처리
- 버튼 클릭, 호버 이벤트 등 사용자 입력 관리

## ⚙️ 특수 시스템

### **Mission Phase System**
- 단계별 미션 진행 관리
- Assassination → Escape → Completed/Failed 구조화된 게임플레이
- 페이즈별 UI 전환과 게임 상태 관리

### **Stealth Mechanics**
- 정교한 스텔스 게임플레이 구현
- CCTV 감시망, 소음 기반 탐지, 3단계 경계 시스템
- AlertIconState (None → Suspicious → Alert) 시각적 피드백

### **Weapon Management**
- 스텔스/전투 상황별 무기 자동 전환
- 탄창/예비탄 관리, 반동 시스템, 확산도 계산
- 252줄의 WeaponManager와 274줄의 Shooter 시스템

### **Sound System**
- 221줄의 NoiseManager를 통한 소음 기반 게임플레이
- investigateThreshold 기반 적 반응 시스템
- BGM, 캐릭터, UI, 무기별 사운드 분류 관리

## 📊 Unity Analytics: 데이터 기반 게임 디자인

### 기술적 선택 이유

**Unity 생태계 완벽 통합**: Unity Gaming Services의 일부로 별도 SDK 없이 즉시 사용 가능하여 개발 시간 단축

**실시간 이벤트 추적**: CustomEvent 시스템으로 스텔스/전투 모드 전환 등 핵심 게임 메커니즘을 정밀 모니터링

**WebGL 플랫폼 최적화**: 브라우저 환경의 데이터 손실 방지를 위한 수동 플러시 시스템 구현

### 핵심 구현 기술

#### 중앙화된 Analytics 관리 (GA.cs)
모든 이벤트를 단일 정적 클래스로 관리하여 코드 일관성 확보

```csharp
public static class GA
{
    public static void Tutorial_Start() => Send("tutorial_start", new Dictionary<string, object>());
    public static void Tutorial_Complete() => Send("tutorial_complete", new Dictionary<string, object>());
    public static void StageComplete(string stage_id, int attempt_count, float play_time, int kill_count, string mode)
    {
        // 스테이지 완료 데이터 전송
    }
    public static void PlayerDeath(Vector3 position, string killer, string mode)
    {
        // 플레이어 사망 위치 및 원인 추적
    }
    public static void ModeSwitch(string from_mode, string to_mode, Vector3 position)
    {
        // 스텔스↔전투 모드 전환 패턴 분석
    }
}
```

#### 스테이지별 진행 추적 (StageRunTracker.cs)
시도 횟수, 플레이 시간, 킬 수를 자동 추적하는 컴포넌트 기반 시스템

#### 위치 기반 사망 분석
플레이어 사망 위치를 0.5f 단위로 스냅하여 히트맵 생성에 최적화된 데이터 수집

### 게임 퍼널 분석 결과
<img width="1229" height="782" alt="image" src="https://github.com/user-attachments/assets/6c209c5f-5d0b-4bdf-87fa-4388c0f62128" />

**5단계 퍼널**: Tutorial Start → Tutorial Complete → Stage 1/2/3 Complete

**주요 인사이트**:
- 튜토리얼 완료율: **85.7%** (높은 초기 유지율)
- Stage 1 완료율: **44.9%** (실제 게임플레이 전환점에서 드롭오프)
- Stage 2 완료율: **55.3%** (스테이지 1 완주자의 높은 지속률)
- Stage 3 완료율: **8.3%** (최종 난이도 조정 필요)
- 전체 완주율: **9.2%** (하드코어 게이머 타겟 확인)

### 기술적 성과

✅ **확장 가능한 이벤트 시스템**: 모듈화된 구조로 새 이벤트 추가 용이  
✅ **성능 최적화**: WebGL 환경 특화 데이터 플러시 전략  
✅ **데이터 정확성**: 중복 방지 로직과 플랫폼별 최적화  

Unity Analytics를 통해 **실제 플레이어 행동 패턴 기반의 게임 디자인 개선**을 달성하였으며, 특히 스텔스-전투 모드 전환 패턴 분석으로 핵심 메커니즘을 최적화했습니다.

## 🛠️ 사용기술 & 시스템  

**Unity Engine**: Unity 2022.3 LTS, C#  
**Unity Input System**: PlayerInput 액션 맵 기반 입력 처리  
**Unity Analytics**: Unity Gaming Services 통합 데이터 수집  
**Unity NavMesh**: 적 AI 경로 탐색 및 순찰 패턴  
**Unity Tilemap**: 2D 레벨 디자인 및 충돌 처리  
**Unity Audio**: AudioMixer 기반 동적 사운드 제어  
**Unity UI (UGUI)**: Canvas 기반 게임 UI 시스템  
**Unity Cinemachine**: 카메라 추적 및 화면 전환 효과  
**Unity Timeline**: 컷신 및 연출 시퀀스  
**DOTween**: UI 애니메이션 및 오브젝트 트윈 효과  

## 🧱 실제 프로젝트 구조 (Assets/Scripts)   

```  
Scripts
├─ Analytics                        # Unity Analytics 구현
│  ├─ GA.cs                        # 중앙화된 이벤트 관리 클래스
│  ├─ StageRunTracker.cs          # 스테이지별 진행 추적
│  ├─ UGSInitializer.cs           # Unity Gaming Services 초기화
│  ├─ ModeSwitchTracker.cs        # 모드 전환 패턴 추적
│  ├─ AnalyticsAutoFlusher.cs     # WebGL 자동 플러시
│  └─ Player
│     └─ PlayerDeathHook.cs       # 플레이어 사망 이벤트 추적
│
├─ Player
│  ├─ Movement
│  │  ├─ PlayerMovement.cs        # 플레이어 이동 제어
│  │  ├─ StealthController.cs     # 스텔스 모드 로직
│  │  └─ CombatController.cs      # 전투 모드 로직
│  ├─ Combat
│  │  ├─ PlayerAttack.cs          # 공격 시스템
│  │  ├─ WeaponController.cs      # 무기 관리
│  │  └─ HealthSystem.cs          # 체력 관리
│  └─ StateMachine
│     ├─ PlayerStateMachine.cs    # 플레이어 상태머신
│     ├─ StealthState.cs          # 스텔스 상태
│     └─ CombatState.cs           # 전투 상태
│
├─ Enemy
│  ├─ AI
│  │  ├─ EnemyAI.cs              # 적 기본 AI
│  │  ├─ PatrolAI.cs             # 순찰 패턴 AI
│  │  ├─ ChaseAI.cs              # 추적 AI
│  │  └─ GuardAI.cs              # 경비 AI
│  ├─ Detection
│  │  ├─ VisionCone.cs           # 시야각 감지 시스템
│  │  ├─ HearingSystem.cs        # 청각 감지 시스템
│  │  └─ AlertSystem.cs          # 경보 시스템
│  └─ StateMachine
│     ├─ EnemyStateMachine.cs    # 적 상태머신
│     ├─ PatrolState.cs          # 순찰 상태
│     ├─ AlertState.cs           # 경계 상태
│     ├─ ChaseState.cs           # 추적 상태
│     └─ AttackState.cs          # 공격 상태
│
├─ Manager
│  ├─ GameManager.cs             # 게임 전체 흐름 관리
│  ├─ StageManager.cs            # 스테이지 관리
│  ├─ UIManager.cs               # UI 시스템 관리
│  ├─ SoundManager.cs            # 사운드 시스템 관리
│  ├─ ModeManager.cs             # 스텔스/전투 모드 관리
│  └─ SaveManager.cs             # 세이브/로드 시스템
│
├─ UI
│  ├─ HUD
│  │  ├─ HealthUI.cs             # 체력 UI
│  │  ├─ StealthUI.cs            # 스텔스 게이지 UI
│  │  ├─ ModeIndicatorUI.cs      # 모드 표시 UI
│  │  └─ MinimapUI.cs            # 미니맵 UI
│  ├─ Menu
│  │  ├─ MainMenuUI.cs           # 메인 메뉴
│  │  ├─ PauseMenuUI.cs          # 일시정지 메뉴
│  │  ├─ SettingsUI.cs           # 설정 메뉴
│  │  └─ GameOverUI.cs           # 게임오버 UI
│  └─ Tutorial
│     ├─ TutorialUI.cs           # 튜토리얼 UI
│     └─ HintUI.cs               # 힌트 시스템 UI
│
├─ Level
│  ├─ LevelManager.cs            # 레벨 관리
│  ├─ CheckpointSystem.cs        # 체크포인트 시스템
│  ├─ InteractableObject.cs      # 상호작용 오브젝트
│  ├─ Door.cs                    # 문 시스템
│  ├─ Switch.cs                  # 스위치 시스템
│  └─ CollectableItem.cs         # 수집 아이템
│
├─ Audio
│  ├─ SoundEffect.cs             # 효과음 관리
│  ├─ BackgroundMusic.cs         # 배경음악 관리
│  ├─ FootstepController.cs      # 발소리 시스템
│  └─ AmbientSound.cs            # 환경음 시스템
│
├─ Utils
│  ├─ Singleton.cs               # 제네릭 싱글톤 베이스
│  ├─ ObjectPool.cs              # 오브젝트 풀링
│  ├─ GameEvents.cs              # 게임 이벤트 시스템
│  ├─ Extensions.cs              # 유틸리티 확장 메서드
│  └─ DebugDrawer.cs             # 디버그 시각화
│
└─ ScriptableObjects
   ├─ StageData.cs               # 스테이지 데이터
   ├─ EnemyData.cs               # 적 데이터
   ├─ WeaponData.cs              # 무기 데이터
   ├─ SoundData.cs               # 사운드 데이터
   └─ GameSettings.cs            # 게임 설정 데이터
```

## 🎯 핵심 동작 요약

### 스텔스 시스템
- **시야각 감지**: Enemy VisionCone이 플레이어 위치를 실시간 체크, LayerMask 기반 장애물 인식
- **소음 시스템**: 플레이어 이동속도에 따른 소음 레벨 생성, 적 HearingSystem이 반응
- **은신 메커니즘**: 특정 오브젝트 뒤 은신 시 탐지 무효화, StealthController가 상태 관리

### 전투 시스템  
- **모드 전환**: ModeManager가 스텔스 ↔ 전투 모드 전환 관리, UI/사운드/이동속도 동기화
- **무기 시스템**: WeaponController가 무기별 공격 패턴 처리, 탄약/쿨타임 관리
- **적 반응**: 전투 모드 진입 시 주변 적들이 AlertState로 전환, 지원 요청 시스템

### Unity Analytics 추적
- **이벤트 수집**: GA.cs가 모든 주요 게임 이벤트를 CustomEvent로 전송
- **퍼널 분석**: StageRunTracker가 각 스테이지별 시도/완료/실패 데이터 자동 수집
- **성능 최적화**: UGSInitializer가 WebGL 환경에서 데이터 손실 방지를 위한 수동 플러시 실행

### 스테이지 진행
- **체크포인트**: CheckpointSystem이 진행률 저장, 사망 시 최근 체크포인트에서 재시작
- **목표 시스템**: StageManager가 스테이지별 목표 설정 및 달성 여부 추적
- **동적 난이도**: 플레이어 퍼포먼스에 따른 적 스폰률 및 탐지 민감도 조절

## 🧯 Troubleshooting
> 개발 과정에서 발생한 주요 문제들과 해결 과정을 정리한 섹션입니다.
