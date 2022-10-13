#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif

namespace Wayway.Engine.UnityGoogleSheet.Editor.Core
{
#if ODIN_INSPECTOR
    [OnInspectorInit("GetScriptableObjectList")]
#endif
    public class UgsDataList : ScriptableObject
    {
        public static UgsDataList Instance => Resources.LoadAll<UgsDataList>("").FirstOrDefault();
        
        [SerializeField] private List<ScriptableObject> spreadSheetDataList;
        [SerializeField] private List<Object> tableDataList;

        private void GetScriptableObjectList()
        {
            spreadSheetDataList = UgsUtility.GetScriptableObjectList(UgsConfig.Instance.ScriptableObjectDataPath, UgsConfig.Instance.Suffix);
            tableDataList = UgsUtility.GetObjectList("ScriptableObject", UgsConfig.Instance.ScriptableObjectScriptPath, UgsConfig.Instance.Suffix);
        }
        
        private void UpdateTableObjectList()
        {
            var tableObjectList = UgsUtility.GetObjectList("ScriptableObject", UgsConfig.Instance.ScriptableObjectScriptPath, UgsConfig.Instance.Suffix);

            tableObjectList.ForEach(x =>
            {
                if (!UgsUtility.FindScriptableObject(x.name, UgsConfig.Instance.ScriptableObjectDataPath,
                        out var result))
                {
                    result = UgsUtility.CreateScriptableObject(x.name, UgsConfig.Instance.ScriptableObjectDataPath);
                }
                
                UgsUtility.InvokeFunction(result, "LoadFromJson");
                EditorUtility.SetDirty(result);
            });

            GetScriptableObjectList();
            AssetDatabase.Refresh();
        }
    }
    
#if UNITY_EDITOR && ODIN_INSPECTOR
    #region Attribute Setting
    public class UgsDataListDrawer : OdinAttributeProcessor<UgsDataList>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<System.Attribute> attributes)
        {
            switch (member.Name)
            {
                case "spreadSheetDataList":
                    attributes.Add(new TitleGroupAttribute("SpreadSheet Data List", "values"));
                    attributes.Add(new SearchableAttribute());
                    attributes.Add(new ListDrawerSettingsAttribute
                    {
                        Expanded = true,
                        IsReadOnly = true,
                        HideAddButton = true,
                        HideRemoveButton = true,
                    });
                    attributes.Add(new PropertySpaceAttribute(0f, 20f));
                    break;
                case "tableDataList":
                    attributes.Add(new TitleGroupAttribute("SpreadSheet Data Script List", "just for Check"));
                    attributes.Add(new SearchableAttribute());
                    attributes.Add(new ListDrawerSettingsAttribute
                    {
                        Expanded = true,
                        IsReadOnly = true,
                        HideAddButton = true,
                        HideRemoveButton = true,
                    });
                    attributes.Add(new PropertySpaceAttribute(0f, 20f));
                    break;
                case "UpdateTableObjectList":
                    attributes.Add(new ButtonAttribute("Create & Update", ButtonSizes.Large));
                    break;
            }
        }
    }
    #endregion
#endif
}
#endif