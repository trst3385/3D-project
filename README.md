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
                pausePanel.SetActive(false); // 초기 상태 비활성화
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
*   4-1. Resources.FindObjectsOfTypeAll은 강력하지만, 씬 전체를 탐색하므로 프로젝트 규모가 매우 커질 경우 성능상 부하가 발생할 수 있습니다.
*   4-2. 또한, 특정 "이름(PausePanel)"에 의존하는 방식은 하드코딩된 문자열이 변경될 경우 대응이 어렵습니다.
*   4-3. 현재 단계에서는 유지보수성과 생산성 측면에서 가장 효율적이라 판단하여 채택했으나, 향후 프로젝트가 확장된다면 ScriptableObject 기반의 의존성 주입이나 Tag/Layer 시스템을 활용한, 보다 유연한 구조로 개선할 계획입니다.

</details>
