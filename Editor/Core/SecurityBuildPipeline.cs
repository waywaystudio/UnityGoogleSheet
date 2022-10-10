using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Wayway.Engine.UnityGoogleSheet.Editor.Core
{
    public class SecurityBuildPipeline : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;
        
        public void OnPostprocessBuild(BuildReport report) { }
        
        public void OnPreprocessBuild(BuildReport report)
        {
            var confirm = EditorPrefs.GetBool("UGS.BuildMsg", false);
            if (!confirm)
            {
                var x = "UGS Setting Object File (Assets/UG/Resources/UGSettingObject.asset) Is Included Api url, password, google drive id. So, not recommended to include UGSettingObject.asset  before distributing it to users as a release. \n\nThis Message Only Onetime Showing.";
                var res = EditorUtility.DisplayDialog("UGS Warning", x, "OK!");
                if (res)
                {
                    EditorPrefs.SetBool("UGS.BuildMsg", true);
                }

            }
        }
    }
}