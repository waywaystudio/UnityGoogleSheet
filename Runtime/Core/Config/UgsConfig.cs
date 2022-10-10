using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
#endif

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public class UgsConfig : ScriptableObject
    {
        public bool doGenerateCSharpScript;
        public bool doGenerateScriptableObject;
        
        public string GoogleScriptUrl;
        public string Password;
        public string GoogleFolderID;
        public string JsonDataPath;
        public string CSharpScriptPath;
        public string Suffix;
        public string ScriptableObjectDataPath;
        public string ScriptableObjectScriptPath;
        
        public static UgsConfig Instance => Resources.LoadAll<UgsConfig>("")[0];

        private void SaveConfig()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
#if UNITY_EDITOR && ODIN_INSPECTOR
    #region Attribute Setting
    public class UgsConfigDrawer : OdinAttributeProcessor<UgsConfig>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<System.Attribute> attributes)
        {
            switch (member.Name)
            {
                case "GoogleScriptUrl":
                    attributes.Add(new TitleGroupAttribute("Credentials", "Settings", order : 0f));
                    break;
                case "Password":
                    attributes.Add(new TitleGroupAttribute("Credentials"));
                    break;
                case "GoogleFolderID":
                    attributes.Add(new TitleGroupAttribute("Credentials"));
                    attributes.Add(new PropertySpaceAttribute(0f, 25f));
                    break;
                case "JsonDataPath":
                    attributes.Add(new TitleGroupAttribute("JSON File", "Generate Options", order : 1f));
                    attributes.Add(new FolderPathAttribute());
                    attributes.Add(new InfoBoxAttribute(
                        "\nUGS 기본옵션 입니다.\n" +
                        "Path에 반드시 <color=green>Resources</color> 가 포함되어야 합니다.\n"));
                    attributes.Add(new PropertySpaceAttribute(0f, 25f));
                    break;
                case "doGenerateCSharpScript":
                    attributes.Add(new TitleGroupAttribute("C# Script", "Generate Options", order : 2f));
                    attributes.Add(new InfoBoxAttribute(
                        "\nUGS 기본옵션 입니다.\n" +
                        "체크를 해제하면 스크립트를 생성하지 않습니다.(Json 파일만 생성합니다)\n"));
                    break;
                case "CSharpScriptPath":
                    attributes.Add(new TitleGroupAttribute("C# Script"));
                    attributes.Add(new ShowIfAttribute("doGenerateCSharpScript"));
                    attributes.Add(new FolderPathAttribute());
                    attributes.Add(new PropertySpaceAttribute(0f, 25f));
                    break;
                case "doGenerateScriptableObject":
                    attributes.Add(new InfoBoxAttribute(
                        "\nWayway 추가옵션 입니다.\n" +
                        "체크하면 스크립터블 오브젝트를 생성합니다.\n"));
                    attributes.Add(new TitleGroupAttribute("ScriptableObject", "Generate Options", order : 3f));
                    break;
                case "Suffix":
                    attributes.Add(new TitleGroupAttribute("ScriptableObject"));
                    attributes.Add(new ShowIfAttribute("doGenerateScriptableObject"));
                    attributes.Add(new TooltipAttribute("Generated Script Name Suffix. (ClassNameSuffix.asset)"));
                    break;
                case "ScriptableObjectDataPath":
                    attributes.Add(new TitleGroupAttribute("ScriptableObject"));
                    attributes.Add(new ShowIfAttribute("doGenerateScriptableObject"));
                    attributes.Add(new FolderPathAttribute());
                    break;
                case "ScriptableObjectScriptPath":
                    attributes.Add(new TitleGroupAttribute("ScriptableObject"));
                    attributes.Add(new ShowIfAttribute("doGenerateScriptableObject"));
                    attributes.Add(new FolderPathAttribute());
                    attributes.Add(new PropertySpaceAttribute(0f, 25f));
                    break;
                case "SaveConfig":
                    attributes.Add(new PropertyOrderAttribute(4f));
                    attributes.Add(new ButtonAttribute(ButtonSizes.Large));
                    break;
            }
        }
    }
    #endregion
#endif
}
