#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
// ReSharper disable UnusedMember.Local
#endif

namespace Wayway.Engine.UnityGoogleSheet.Editor.Core
{
    public class UgsDataList : ScriptableObject
    {
        private List<ScriptableObject> spreadSheetDataList = new ();
        private List<MonoScript> tableDataList = new ();

#if ODIN_INSPECTOR
        [OnInspectorInit]
#endif
        private void GetSheetDataObjectList()
        {
            spreadSheetDataList.Clear();
            tableDataList.Clear();
            
            spreadSheetDataList = GetScriptableObjectList(UgsConfig.Instance.ScriptableObjectDataPath, UgsConfig.Instance.Suffix);
            tableDataList = UgsUtility.GetMonoScriptList(UgsConfig.Instance.ScriptableObjectScriptPath, $"{UgsConfig.Instance.Suffix}");
        }
        
        private void UpdateTableObjectList()
        {
            var tableObjectList = UgsUtility.GetMonoScriptList(UgsConfig.Instance.ScriptableObjectScriptPath, $"{UgsConfig.Instance.Suffix}");

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

            GetSheetDataObjectList();
            AssetDatabase.Refresh();
        }
        
        private static List<ScriptableObject> GetScriptableObjectList(string folderPath, string filter)
        {
            var result = new List<ScriptableObject>();

            if (string.IsNullOrEmpty(folderPath)) folderPath = "Assets";
            if (string.IsNullOrEmpty(filter)) filter = "";

            var gUIDs = AssetDatabase.FindAssets(filter, new [] { folderPath });

            foreach (var x in gUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(x);
                var data = AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject)) as ScriptableObject;

                if (!result.Contains(data) && data is not null)
                    result.Add(data);
            }

            return result;
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
                    attributes.Add(new ShowInInspectorAttribute());
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
                    attributes.Add(new ShowInInspectorAttribute());
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