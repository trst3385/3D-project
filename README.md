# 3D project


<details>
<summary><b>비활성화된 UI 오브젝트 탐색 실패 문제 해결 및 자동 참조 로직 설계(PauseManager.cs)</b></summary>
<br/>

### 🚨 문제점

- GameObject.Find()를 사용하여 PausePanel을 탐색하려 했으나, UI가 비활성화(SetActive(false))된 상태일 경우 탐색이 불가능하여 null이 반환되는 문제가 발생했습니다.
- 기존의 수동 드래그 앤 드롭(Inspector 연결) 방식은 프로젝트 규모가 커질수록 참조 누락의 위험이 있고, 유지보수성이 떨어진다는 단점이 있었습니다.
      
### 🔍 원인 분석

- **탐색 제한**: 유니티의 GameObject.Find()는 활성화된 오브젝트만을 대상으로 탐색을 수행하므로, 비활성화된 오브젝트는 찾을 수 없는 설계적 제약이 있습니다.
- **조용한 실패(Silent Failure)**: null이 반환되었음에도 불구하고, if (pausePanel != null)과 같은 방어 코드에 의해 로직이 정상적으로 건너뛰어지면서, 시스템이 '작동하지 않음'을 인지하기까지 시간이 지체되었습니다.

### 🛠 해결 과정

1. **전역 탐색 로직 도입**: Resources.FindObjectsOfTypeAll<Canvas>()를 사용하여 씬 내의 모든 캔버스(비활성화 상태 포함)를 탐색 범위에 포함하였습니다.
 
2. **인스턴스 필터링**: canvas.gameObject.scene.name != null 조건을 추가하여, 프리팹이 아닌 실제 씬에 배치된 캔버스 인스턴스만을 정교하게 필터링하였습니다.
   
3. **계층 탐색 구현**: Transform.Find("PausePanel") 메서드를 활용하여 캔버스 자식 오브젝트 중 특정 이름의 UI를 동적으로 탐색하고 할당하였습니다.
   
4. **시스템 자동화**: 별도의 인스펙터 연결 없이 Start() 시점에 시스템이 스스로 UI를 찾아 연결하도록 설계하여, 개발 편의성과 시스템의 견고함을 동시에 확보하였습니다.
💻 핵심 코드 구현
```csharp 
void Start()
{
    //1. 씬 내의 모든 캔버스(비활성화 포함)를 탐색
    Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
    
    foreach (Canvas canvas in allCanvases)
    {
        //2. 씬에 포함된 객체만 필터링 (프리팹 제외)
        if (canvas.gameObject.scene.name != null)
        {
            //3. 자식 중 "PausePanel" 이름을 가진 오브젝트를 탐색
            Transform found = canvas.transform.Find("PausePanel");
            if (found != null)
            {
                pausePanel = found.gameObject;
                pausePanel.SetActive(false);//초기 상태 비활성화
                break;//탐색 완료 후 종료
            }
        }
    }
}
```

  
### 💡 배운 점

1. **유니티 검색 메커니즘 이해**: 호출 시점과 오브젝트의 활성/비활성 여부에 따라 탐색 함수들이 제각각 다른 결과를 도출함을 이해하였습니다. 상황에 맞는 적절한 API 선택이 중요함을 배웠습니다.
2. **방어적 설계의 양면성**: if (null) 방어 코드는 버그를 막는 좋은 수단이지만, 근본적인 원인을 은폐할 수 있음을 깨달았습니다. 따라서 로직 오류를 빠르게 파악하기 위해 적절한 로그와 디버깅을 병행해야 한다는 점을 익혔습니다.
3. **코드의 견고함**: 하드코딩된 참조를 제거하고 시스템이 스스로 환경을 인식하게 만드는 '자동 연결' 설계가 대규모 프로젝트의 유지보수 효율을 어떻게 극대화하는지 체감하였습니다.
4. **방법론의 한계와 트레이드오프**:
   * **성능 부하**: Resources.FindObjectsOfTypeAll은 강력하지만, 씬 전체를 탐색하므로 프로젝트 규모가 매우 커질 경우 성능상 부하가 발생할 수 있습니다.
   * **하드코딩 의존성**: 또한, 특정 "이름(PausePanel)"에 의존하는 방식은 하드코딩된 문자열이 변경될 경우 대응이 어렵습니다.
   * **향후 개선 계획**: 현재 단계에서는 유지보수성과 생산성 측면에서 가장 효율적이라 판단하여 채택했으나, 향후 프로젝트가 확장된다면 ScriptableObject 기반의 의존성 주입이나 Tag/Layer 시스템을 활용한, 보다 유연한 구조로 개선할 계획입니다.

</details>


<details>
<summary><b>유연한 게임 시스템 구축을 위한 리팩토링 프로젝트</b></summary> 
      
![Architecture](https://img.shields.io/badge/DataDriven-Design-orange)
      
이 프로젝트는 하드코딩된 로직을 ScriptableObject 기반의 데이터 구조로 전환하여 **시스템의 확장성과 코드의 유지보수성** 을 높이는 데 중점을 두고 개발되었습니다.<br/>

### 🚨 문제점
- 몬스터 스탯, 스폰 속도 등 라운드별 설정값이 `EnemySpawner`와 `GameManager` 매니저 스크립트에 하드코딩되어 있어, 수치 변경 시마다 다수의 스크립트를 수정해야 하는 비효율이 발생했습니다.
- 이런 방식은 매니저 간 데이터 참조가 분산되어 있어 데이터 일관성을 유지하기 어렵고, 코드의 결합도(Coupling)가 높아 유지보수에 어려움이 있었습니다.

### 🔍 원인 분석
- **데이터와 로직의 혼재**: 데이터값이 코드 내부에 직접 명시되어 있어, 게임 디자인 변경이 코드 수정으로 이어지는 구조였습니다.
- **분산된 의존성**: 여러 매니저가 각기 다른 곳에서 데이터를 참조하여, 시스템 전체의 데이터가 여러 스크립트에 흩어져 있어, **어느 쪽이 최신 설정값인지 관리하기가 매우 번거로웠습니다.**

### 🛠 해결 과정
**1. 핵심 구현: 중앙 집중형 데이터 허브 (GameManager.cs)**
```csharp 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Data")]
    public RoundData currentRoundData;//데이터 허브의 중심
    
    //이벤트 기반 통신: 결합도 감소
    public event Action OnBossSpawn;
    public event Action OnGameClear;

    private void Awake() => Instance = this;

    //다른 매니저들이 데이터를 참조할 창구 (직접적인 필드 접근보다 안전함)
    public RoundData GetCurrentRoundData() => currentRoundData;
    
    public void TriggerBossSpawn() => OnBossSpawn?.Invoke();
}
```
**2. 활용 예시: EnemySpawner.cs (결합도 감소)**
```csharp
private void SpawnEnemy()
{
    //GameManager라는 '창구'를 통해서만 데이터를 가져옴
    var data = GameManager.Instance.GetCurrentRoundData();
    
    if (data.enemyPrefab != null)
    {
        Instantiate(data.enemyPrefab, spawnPoint.position, Quaternion.identity);
    }
}
```


1. **데이터 구조화(ScriptableObject)**: `RoundData` SO를 생성하여 이곳에 등장할 몬스터의 프리팹, 스폰 간격 등 라운드 설정을 에셋 파일로 분리했습니다.
2. **중앙 집중형 허브 설계**: `GameManager`를 데이터 허브로 구축하여 이 매니자 스크립트에서만 `RoundData` SO를 연결하고, 나머지 매니저가 `GameManager`를 통해서만 데이터에 접근하도록 구조를 변경했습니다.
3. **이벤트 기반 통신(Action/Delegate)**: `OnBossSpawn`, `OnGameClear` 등의 이벤트(옵저버 패턴)를 도입하여 매니저 간의 직접적인 참조를 제거하고 결합도를 낮췄습니다.
4. **로직 단순화**: SO의 데이터 존재 여부(bossPrefab != null)만으로 로직을 판단하게 하여 불필요한 조건문을 제거했습니다.


### 💡 배운 점 및 향후 계획
- **확장성 확보**: 이제 라운드 추가 시 코드 수정 없이 `RoundData` SO에셋 파일만 생성하면 되는 유연한 시스템을 구축했습니다.
- **설계의 중요성**: 기능 구현보다 '어떻게 시스템을 설계할 것인가', '다른 개발자들도 편하게 이 스크립트를 확인할 수 있는가' 가 개발자의 핵심 역량임을 재확인했습니다.
- **향후 개선 계획**: 현재는 라운드 데이터를 배열`(RoundData[])` 로 관리할 예정이며, 추후 인덱스 기반 자동 전환 로직을 구현하여 완전히 자동화된 웨이브 시스템을 완성할 계획입니다.
</details>
