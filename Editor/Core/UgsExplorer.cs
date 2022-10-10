#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core;
using Wayway.Engine.UnityGoogleSheet.Core.HttpProtocolV2;

#pragma warning disable CS0414

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace Wayway.Engine.UnityGoogleSheet.Editor.Core
{
#if ODIN_INSPECTOR
    [OnInspectorDispose("Clear")]
#endif
    public class UgsExplorer : ScriptableObject
    {
        public static UgsExplorer Instance => Resources.LoadAll<UgsExplorer>("").FirstOrDefault();
        
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

        public List<FileData> LoadedFileDataList = new ();
        
        private static readonly Stack<string> PrevFolderIDStack = new ();
        private string currentViewFolderID;
        private bool isWaitForCreate;
        private bool isInitiated;

        public static void ParseWorkSheet(string fileID, string workSheetName)
        {
            PreloadType();
            
            UnityEditorWebRequest.Instance.ReadSpreadSheet(
                new ReadSpreadSheetReqModel(fileID), 
                null, 
                x =>
                {
                    UnityGSParser.ParseJsonData(x);
                    UnityGSParser.ParseSheet(x, workSheetName);
                });
        }
        
        public static void ParseSpreadSheet(string fileID)
        {
            PreloadType();
            
            UnityEditorWebRequest.Instance.ReadSpreadSheet(
                new ReadSpreadSheetReqModel(fileID), 
                null,
                x =>
                {
                    UnityGSParser.ParseJsonData(x);
                    UnityGSParser.ParseSheet(x);
                });
        }
        
        private void ParseAllSpreadSheet()
        {
            PreloadType();
            
            foreach (var file in LoadedFileDataList.Where(file => file.type == FileType.Excel))
            {
                ParseSpreadSheet(file.id);
            }
        }

        private static void PreloadType() => TypeMap.Init();

        private void Show()
        {
            isInitiated = true;
            CreateFile(UgsConfig.Instance.GoogleFolderID);
        }

        private void Clear()
        {
            isInitiated = false;
            isWaitForCreate = false;
            PrevFolderIDStack.Clear();
            LoadedFileDataList.Clear();
        }

        private void CreateFile(string folderID)
        {
            LoadedFileDataList.Clear();

            if (isWaitForCreate) return;
            isWaitForCreate = true;
            
            UnityEditorWebRequest.Instance.GetDriveDirectory(new GetDriveDirectoryReqModel(folderID), null, x =>
            {
                // 루트폴더가 아닐 경우, 상위로 폴더로 이동하는 FileData 추가
                if (folderID != UgsConfig.Instance.GoogleFolderID)
                {
                    LoadedFileDataList.Add(new FileData(FileType.ParentFolder,
                                                         UgsConfig.Instance.GoogleFolderID, 
                                                            UgsConfig.Instance.GoogleFolderID, 
                                                    "../",
                                                            () => ExplorerFolder(UgsConfig.Instance.GoogleFolderID)));
                }
                else
                {
                    currentViewFolderID = folderID;
                }
                
                // 구글드라이브 폴더의 파일을 List<FileData>에 추가
                for (var i = 0; i < x.fileId.Count; i++)
                {
                    LoadedFileDataList.Add(new FileData((FileType)x.fileType[i], 
                                                                      x.url[i], 
                                                                      x.fileId[i], 
                                                                      x.fileName[i],
                                                                      ActionSelector((FileType)x.fileType[i], x.fileId[i])));
                }

                LoadedFileDataList = LoadedFileDataList.OrderBy(fileData => fileData.fileName).ToList();
                isWaitForCreate = false;
            });
        }

        private Action ActionSelector(FileType fileType, string fileID) => fileType switch
        {
            FileType.ParentFolder => ToParent,
            FileType.Folder => () => ExplorerFolder(fileID),
            FileType.Excel => () => ParseSpreadSheet(fileID),
            _ => () => Debug.LogError("UnknownFileType Inserted!")
        };

        private void ExplorerFolder(string fileID)
        {
            PrevFolderIDStack.Push(currentViewFolderID);
            currentViewFolderID = fileID;
            CreateFile(currentViewFolderID);
        }

        private void ToParent()
        {
            var prevFolder = PrevFolderIDStack.Pop();
            currentViewFolderID = prevFolder;
            CreateFile(currentViewFolderID);
        }
    }
    
#if ODIN_INSPECTOR
    #region PropertyDrawer
    public class UgsExplorerDrawer : OdinAttributeProcessor<UgsExplorer>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<System.Attribute> attributes)
        {
            switch (member.Name)
            {
                case "LoadedFileDataList":
                    attributes.Add(new ListDrawerSettingsAttribute
                    {
                        Expanded = true,
                        NumberOfItemsPerPage = 30,
                        IsReadOnly = true
                    });
                    break;
                case "Show":
                    attributes.Add(new HideIfAttribute("isInitiated"));
                    attributes.Add(new ButtonAttribute(ButtonSizes.Large)
                    {
                        Name = "Load from GoogleDrive"
                    });
                    break;
                case "ParseAllSpreadSheet":
                    attributes.Add(new ShowIfAttribute("isInitiated"));
                    attributes.Add(new ButtonAttribute(ButtonSizes.Large)
                    {
                        Name = "Parse All Spread Sheet in this Section"
                    });
                    break;
            }
        }
    }

    public class FileDataDrawer : OdinAttributeProcessor<UgsExplorer.FileData>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<System.Attribute> attributes)
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