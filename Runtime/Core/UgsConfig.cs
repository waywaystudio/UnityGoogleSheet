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
        public bool DoGenerateCSharpScript;
        public bool DoGenerateScriptableObject;
        public bool CompareHashCode;
        
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
                    attributes.Add(new TitleGroupAttribute("Common Setting", "Generate Options", order : 1f));
                    attributes.Add(new FolderPathAttribute());
                    attributes.Add(new InfoBoxAttribute(
                        "\nUGS ???????????? ?????????.\n" +
                        "Path??? ????????? <color=green>Resources</color> ??? ??????????????? ?????????.\n"));
                    attributes.Add(new PropertySpaceAttribute(0f, 25f));
                    break;
                case "CompareHashCode":
                    attributes.Add(new TitleGroupAttribute("Common Setting", order : 2f));
                    attributes.Add(new InfoBoxAttribute(
                        "\n?????? ?????? ???????????? ????????? ??? ?????? ???????????? ???????????????..\n" +
                        "?????? ????????? ????????? ???????????? ????????????. ???????????? ???????????????.\n" +
                        "?????? ???????????? ???????????? ????????? ???????????????, ????????? ???????????? ??????????????????\n"));
                    break;
                case "DoGenerateCSharpScript":
                    attributes.Add(new TitleGroupAttribute("C# Script", "Generate Options", order : 2f));
                    attributes.Add(new InfoBoxAttribute(
                        "\nUGS ???????????? ?????????.\n" +
                        "????????? ???????????? ??????????????? ???????????? ????????????.(Json ????????? ???????????????)\n"));
                    break;
                case "CSharpScriptPath":
                    attributes.Add(new TitleGroupAttribute("C# Script"));
                    attributes.Add(new ShowIfAttribute("DoGenerateCSharpScript"));
                    attributes.Add(new FolderPathAttribute());
                    attributes.Add(new PropertySpaceAttribute(0f, 25f));
                    break;
                case "DoGenerateScriptableObject":
                    attributes.Add(new InfoBoxAttribute(
                        "\nWayway ???????????? ?????????.\n" +
                        "???????????? ??????????????? ??????????????? ???????????????.\n"));
                    attributes.Add(new TitleGroupAttribute("ScriptableObject", "Generate Options", order : 3f));
                    break;
                case "Suffix":
                    attributes.Add(new TitleGroupAttribute("ScriptableObject"));
                    attributes.Add(new ShowIfAttribute("DoGenerateScriptableObject"));
                    attributes.Add(new TooltipAttribute("Generated Script Name Suffix. (ClassNameSuffix.asset)"));
                    break;
                case "ScriptableObjectDataPath":
                    attributes.Add(new TitleGroupAttribute("ScriptableObject"));
                    attributes.Add(new ShowIfAttribute("DoGenerateScriptableObject"));
                    attributes.Add(new FolderPathAttribute());
                    break;
                case "ScriptableObjectScriptPath":
                    attributes.Add(new TitleGroupAttribute("ScriptableObject"));
                    attributes.Add(new ShowIfAttribute("DoGenerateScriptableObject"));
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
