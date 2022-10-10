## UGS - Unity Google Sheet
* 기존 유료에셋을 간소화, 정리, 기능추가한 버전
* 원본 Readme는 https://shlifedev.gitbook.io/unitygooglesheets/

## 사용방법

**UnityTopMenu > Wayway > UGS**

### UGS Config
* Credentials
  * 값 설정방법은 원본 Readme 파일 참조
  * Json File은 반드시 생성됨.        
* C# Script
  * UGS original 형태.
  * Do Generate C Sharp Script를 선택하고, 스크립트가 생성될 디렉토리를 설정하면 끝.
* Scriptable Object
  * 추가 기능
  * Do Generate Scriptable Object 선택
  * Suffix는, 자동으로 생성되는 스크립트 접미사
  * C# Script 생성과 병행시, 중복 이름 방지
  * 가급적 Data 유지
  * DataPath는 Scriptable Object.asset이 저장될 디렉토리
  * ScriptPath는 해당 클래스 스크립트가 저장될 디렉토리

### UGS Generator
* Load from GoogleDrive 클릭.
  * 폴더 버튼 : 해당폴더 탐색
  * 링크 버튼 : 웹으로 이동
  * **싱크 버튼** : 자동생성

### UGS DataList
* SpreadSheetDataList
  * 프로젝트 내에 있는 Scriptable Objects를 가짐.
* SpreadSheet Data Sciprt List
  * 프로젝트 내에 있는 SO scripts를 가짐.
* **Create & Updata**
  * SpreadSheet Data Sciprt List 를 통해서, 아직 SO가 만들어지지 않은 스크립트는 SO를 만들고, 기존에 SO는 내용을 업데이트 합니다.

---
## UGS Scriptable Object : TableObject
### ex.SpreadSheetScriptableObject.cs
```C#

public partial class SpreadSheetScriptableObject : TableObject
{
    [SerializeField]
    private List<SpreadSheetScriptableObject> list = new ();
    private Dictionary<@keyType, SpreadSheetScriptableObject> table = new ();
    // and more Properties...
    
    public override void LoadFromJson() 
    {
        //...    
    }
    public override void LoadFromGoogleSpreadSheet()
    {
        //...
    }    
    // and more Functions...
}    

```

* Runtime 에서는 Dictionary 사용을 추천
* Load From Json
  * 프로젝트 내부에 저장되어 있는 Json파일을 읽음.
  * 웹과 통신하지 않음.
  * 스프레드 시트 변경 후에, 싱크시키는 기능으로는 비추천
* Load From Google SpreadSheet
  * SO가 싱크되어 있는 SpreadSheet를 웹에서 읽어 갱신함.
  * 데이타 갱신시 주로 사용
  * 개별적용임으로 모든 시트를 갱신하기 위해서는 **UGS Generator**를 사용해야 함.

---
# 패치내역
* 22.10.09 첫 작성
* 22.10.10 Submoudlize, Dependency
