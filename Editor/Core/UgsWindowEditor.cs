#if ODIN_INSPECTOR
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core;

namespace Wayway.Engine.UnityGoogleSheet.Editor.Core
{
    public class UgsWindowEditor : OdinMenuEditorWindow
    {
        [MenuItem("Wayway/UnityGoogleSheet")]
        private static void OpenWindow()
        {
            var window = GetWindow<UgsWindowEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 800);
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var explorer = Resources.LoadAll<UgsExplorer>("").FirstOrDefault();
            var dataList = Resources.LoadAll<UgsDataList>("").FirstOrDefault();
            
            var tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                // { "Home",            this,                     EditorIcons.House         },
                { "UGS Config",      UgsConfig.Instance ,      EditorIcons.SettingsCog   },
                { "UGS Generator",   explorer,                 EditorIcons.Table         },
                { "UGS DataList",    dataList,                 EditorIcons.List          },
            };

            tree.DefaultMenuStyle.Height = 40;
            tree.DefaultMenuStyle.IconSize = 20;
            tree.DefaultMenuStyle.IconOffset = -2;
                    
            return tree;
        }
    }
}
#endif