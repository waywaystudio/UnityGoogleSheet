#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using Wayway.Engine.UnityGoogleSheet.Core;

namespace Wayway.Engine.UnityGoogleSheet.Editor.Core
{
    public class UgsWindowEditor : OdinMenuEditorWindow
    {
        [MenuItem("Wayway/UGS")]
        private static void OpenWindow()
        {
            var window = GetWindow<UgsWindowEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 700);
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                // { "Home",            this,                     EditorIcons.House         },
                { "UGS Config",      UgsConfig.Instance ,      EditorIcons.SettingsCog   },
                { "UGS Generator",   UgsExplorer.Instance,     EditorIcons.Table         },
                { "UGS DataList",    UgsDataList.Instance,     EditorIcons.List          },
            };

            tree.DefaultMenuStyle.Height = 40;
            tree.DefaultMenuStyle.IconSize = 20;
            tree.DefaultMenuStyle.IconOffset = -2;
                    
            return tree;
        }
    }
}
#endif