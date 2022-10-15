#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
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
        public List<FileData> DriveFileDataList = new ();
        
        private static readonly Stack<string> PrevFolderIDStack = new ();
        private string currentViewFolderID;
        private bool isWaitForCreate;
        private bool isInitiated;
        
        public static void ParseSpreadSheet(string fileID, string specificWorkSheetName)
        {
            TypeMap.Init();
            
            UnityEditorWebRequest.Instance.ReadSpreadSheet(
                new ReadSpreadSheetReqModel(fileID), 
                null,
                x =>
                {
                    UnityGSParser.ParseJsonData(x);
                    UnityGSParser.ParseSheet(x, specificWorkSheetName);
                });
        }

        private void ParseAllSpreadSheet() => DriveFileDataList.Where(file => file.type == FileType.Excel)
                                                               .ForEach(x => ParseSpreadSheet(x.id, null));

        private void Show()
        {
            isInitiated = true;
            LoadDriveFiles(UgsConfig.Instance.GoogleFolderID);
        }

        private void LoadDriveFiles(string folderID)
        {
            DriveFileDataList.Clear();

            if (isWaitForCreate) return;
            isWaitForCreate = true;
            
            UnityEditorWebRequest.Instance.GetDriveDirectory(new GetDriveDirectoryReqModel(folderID), null, x =>
            {
                // 루트폴더가 아닐 경우, 상위로 폴더로 이동하는 FileData 추가
                if (folderID != UgsConfig.Instance.GoogleFolderID)
                {
                    DriveFileDataList.Add(new FileData(FileType.ParentFolder,
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
                    DriveFileDataList.Add(new FileData((FileType)x.fileType[i], 
                                                                      x.url[i], 
                                                                      x.fileId[i], 
                                                                      x.fileName[i],
                                                                      ActionSelector((FileType)x.fileType[i], x.fileId[i])));
                }

                DriveFileDataList = DriveFileDataList.OrderBy(fileData => fileData.fileName)
                                                       .ToList();
                isWaitForCreate = false;
            });
        }

        private Action ActionSelector(FileType fileType, string fileID) => fileType switch
        {
            FileType.ParentFolder => ToParent,
            FileType.Folder => () => ExplorerFolder(fileID),
            FileType.Excel => () => ParseSpreadSheet(fileID, null),
            _ => () => Debug.LogError("UnknownFileType Inserted!")
        };

        private void ExplorerFolder(string fileID)
        {
            PrevFolderIDStack.Push(currentViewFolderID);
            currentViewFolderID = fileID;
            LoadDriveFiles(currentViewFolderID);
        }

        private void ToParent()
        {
            var prevFolder = PrevFolderIDStack.Pop();
            currentViewFolderID = prevFolder;
            LoadDriveFiles(currentViewFolderID);
        }
        
        private void Clear()
        {
            isInitiated = false;
            isWaitForCreate = false;
            PrevFolderIDStack.Clear();
            DriveFileDataList.Clear();
        }
    }
    
#if ODIN_INSPECTOR
    #region PropertyDrawer
    public class UgsExplorerDrawer : OdinAttributeProcessor<UgsExplorer>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            switch (member.Name)
            {
                case "DriveFileDataList":
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
    #endregion
#endif
}