using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

#pragma warning disable CS0414

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace Wayway.Engine.UnityGoogleSheet.Editor.Core
{
    public enum FileType
    {
        Folder, 
        ParentFolder, 
        Excel, 
        Unknown
    }
        
    [Serializable]
    public class FileData
    {
        public FileData(FileType type, string url, string id, string fileName, Action actionTemplate)
        {
            this.type = type;
            this.url = url;
            this.fileName = fileName;
            this.id = id;
            this.actionTemplate = actionTemplate;
        }
            
        public FileType type;
        public string id;
        public string url;
        public string fileName;
        private Action actionTemplate;
        private bool IsExcelType => type is FileType.Excel;

        /* Function(A,B)는 오딘의 버튼이미지 출력방식 때문 구현.
         * Sirenix에서 이미지를 출력할 수 있게 패치한다고 발표함.
         * 이후 구조 변경할 예정.
         */ 
        public void FunctionA() => actionTemplate?.Invoke();
        public void FunctionB() => actionTemplate?.Invoke();
            
        public void Link()
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("url is Empty");
                return;
            }
                
            Application.OpenURL(url);
        }
    }
    
#if ODIN_INSPECTOR
    #region PropertyDrawer
    
    public class FileDataDrawer : OdinAttributeProcessor<FileData>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            switch (member.Name)
            {
                case "type" :
                    attributes.Add(new HideInInspector());
                    break;
                case "id" :
                    attributes.Add(new HideInInspector());
                    break;
                case "url" :
                    attributes.Add(new HideInInspector());
                    break;
                case "fileName" :
                    attributes.Add(new HorizontalGroupAttribute("Group")
                    {
                        PaddingLeft = 10f
                    });
                    attributes.Add(new HideLabelAttribute());
                    attributes.Add(new DisplayAsStringAttribute());
                    attributes.Add(new PropertyOrderAttribute(3f));
                    break;
                case "FunctionA" :
                    attributes.Add(new HorizontalGroupAttribute("Group")
                    {
                        Width = 30f
                    });
                    attributes.Add(new ButtonAttribute(" ")
                    {
                        ButtonHeight = 22
                    });
                    attributes.Add(new ButtonIconAttribute("Folder"));
                    attributes.Add(new PropertyOrderAttribute(1f));
                    attributes.Add(new HideIfAttribute("IsExcelType"));
                    break;
                case "FunctionB" :
                    attributes.Add(new HorizontalGroupAttribute("Group")
                    {
                        Width = 30f
                    });
                    attributes.Add(new ButtonAttribute(" ")
                    {
                        ButtonHeight = 22
                    });
                    attributes.Add(new ButtonIconAttribute("Download"));
                    attributes.Add(new PropertyOrderAttribute(1f));
                    attributes.Add(new ShowIfAttribute("IsExcelType"));
                    break;
                case "Link" :
                    attributes.Add(new HorizontalGroupAttribute("Group")
                    {
                        Width = 30f
                    });
                    attributes.Add(new ButtonAttribute(" ")
                    {
                        ButtonHeight = 22
                    });
                    attributes.Add(new ButtonIconAttribute("Link"));
                    attributes.Add(new PropertyOrderAttribute(2f));
                    break;
            }
        }
    }

    #endregion
#endif
}

