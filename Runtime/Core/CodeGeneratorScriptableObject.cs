using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;
using Wayway.Engine.UnityGoogleSheet.Core.IO;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public class CodeGeneratorScriptableObject : ICodeGenerator
    {
        public CodeGeneratorScriptableObject(SheetInfo info)
        {
            sheetInfo = info;
        }
        
        private static SheetInfo sheetInfo;

        public string GenerateForm { get; private set; } = 
@"/*     ===== Do not touch this. Auto Generated Code. =====    */
/*     If you want custom code generation modify this => 'CodeGeneratorScriptableObject.cs'  */
//     ReSharper disable BuiltInTypeReferenceStyle
//     ReSharper disable PartialTypeWithSinglePart
//     ReSharper disable ConvertToConstant.Local
#pragma warning disable CS0414
#if UNITY_EDITOR
using System.Reflection;
using Sirenix.OdinInspector.Editor;
#endif
using Sirenix.OdinInspector;
@assemblies
namespace @namespace
{    
    public partial class @Class@suffix : ScriptableObject
    { 
/* Fields. */    
        [SerializeField] 
        [TableList(AlwaysExpanded = true, HideToolbar = true, DrawScrollView = true, IsReadOnly = true)] 
        private List<@Class> @classList = new ();
        private Dictionary<@keyType, @Class> @classTable = new ();        

/* Properties. */
        public List<@Class> @ClassList => @classList;
        public Dictionary<@keyType, @Class> @ClassTable => @classTable ??= new Dictionary<@keyType, @Class>();

/* Editor Functions. */
    #if UNITY_EDITOR
        private string spreadSheetID = ""@spreadSheetID"";
        private string sheetID = ""@sheetID"";
    #endif
@loadFunctions
/* innerClass. */
        [Serializable]
        public class @Class
        {
@types
        }
    }
@drawer
}";

        public string Generate()
        {
            var @namespace = $"SpreadSheet.{sheetInfo.sheetFileName}";
            var className = sheetInfo.sheetName;
            
            TypeMap.Init();

            WriteAssembly(new [] 
            {
                "System",
                "System.Collections.Generic",
                "UnityEngine"
            }, 
                sheetInfo.sheetTypes, 
                sheetInfo.isEnumChecks);

            WriteNamespace(@namespace);
            WriteLoadFunction(sheetInfo.sheetFileName);
            WriteDrawer();
            WritePascalClassReplace(ToPascalCasing(className));
            WriteCamelClassReplace(ToCamelCasing(className));
            WriteClassSuffix(UgsConfig.Instance.Suffix);
            WriteSpreadSheetData(sheetInfo.spreadSheetID, sheetInfo.sheetID);
            
            WriteTypes(sheetInfo.sheetTypes, 
                       sheetInfo.sheetVariableNames, 
                       sheetInfo.isEnumChecks);

            Debug.Log($"Generate <color=green><b>{className} ScriptableObject.cs</b></color> Complete");
            
            return GenerateForm;
        }
        
        private void WriteAssembly(string[] assemblies, IReadOnlyList<string> types = null, IReadOnlyList<bool> isEnums = null)
        {
            if (assemblies != null)
            {
                var builder = new StringBuilder();
                foreach (var assembly in assemblies)
                {
                    builder.AppendLine($"using {assembly};");
                }

                GenerateForm = GenerateForm.Replace("@assemblies", builder.ToString());
                builder.AppendLine("@assemblies");
            }

            if (types != null && isEnums != null)
            {
                var builder = new StringBuilder();
                for (var i = 0; i < types.Count; i++)
                {
                    var type = types[i];
                    var isEnum = isEnums[i];
                    type = type.Replace(" ", null);
                    type = type.Replace("Enum<", null);
                    type = type.Replace(">", null);
                    
                    if (isEnum && !string.IsNullOrEmpty(TypeMap.EnumMap[type].NameSpace))
                    {
                        builder.AppendLine($"using {TypeMap.EnumMap[type].NameSpace};");
                    }
                }
                
                GenerateForm = GenerateForm.Replace("@assemblies", builder.ToString());
            }
            else
            {
                GenerateForm = GenerateForm.Replace("@assemblies", null);
            }
        }
       
        private void WriteNamespace(string @namespace)
        {
            GenerateForm = GenerateForm.Replace("@namespace", @namespace);
        }
        
        private void WritePascalClassReplace(string @class)
        {
            GenerateForm = GenerateForm.Replace("@Class", @class);
        }
        
        private void WriteCamelClassReplace(string @class)
        {
            GenerateForm = GenerateForm.Replace("@class", @class);
        }

        private void WriteTypes(IReadOnlyList<string> types, IReadOnlyList<string> fieldNames, IReadOnlyList<bool> isEnum)
        {
            if (types != null)
            {
                var builder = new StringBuilder();
                
                for (var i = 0; i < types.Count(); i++)
                {
                    if (isEnum[i] == false)
                    {
                        var targetType = types[i];
                        var targetField = fieldNames[i];
                        TypeMap.StrMap.TryGetValue(targetType, out var outType);
                        
                        if (outType == null)
                        {
                            var debugTypes = string.Join("  ", sheetInfo.sheetTypes);
                            
                            Debug.Log("<color=#00ff00><b>-------UGS IMPORTANT ERROR DEBUG---------</b></color>");
                            Debug.LogError($"<color=white><b>Error Sheet Name => </b></color>{sheetInfo.sheetFileName}.{sheetInfo.sheetName}");
                            Debug.LogError($"<color=white><b>Your type list => </b></color> => {debugTypes}");
                            Debug.LogError($"<color=#00ff00><b>error field =>:</b></color> {targetField} : {sheetInfo.sheetTypes[i]}");
                            
                            throw new TypeParserNotFoundException("Type Parser Not Found, You made your own type parser? check custom type document on gitbook document.");
                        }
                        
                        builder.AppendLine($"\t\t\tpublic {GetCSharpRepresentation(TypeMap.StrMap[types[i]], true)} {ToPascalCasing(fieldNames[i])};");
                    }
                    else
                    {
                        var str = types[i];
                        
                        str = str.Replace("<", null);
                        str = str.Replace(">", null);
                        str = str.Replace(" ", null);
                        str = str.Remove(0, 4);
                        
                        builder.AppendLine($"\t\t\tpublic {GetCSharpRepresentation(TypeMap.EnumMap[str].Type, true)} {ToPascalCasing(fieldNames[i])};");
                    }
                }
                
                GenerateForm = GenerateForm.Replace("@types", builder.ToString());
                GenerateForm = GenerateForm.Replace("@keyType", types[0]);
            }
        }
        
        private void WriteSpreadSheetData(string spreadID, string sheetID)
        {
            GenerateForm = GenerateForm.Replace("@spreadSheetID", spreadID);
            GenerateForm = GenerateForm.Replace("@sheetID", sheetID);
        }

        private void WriteLoadFunction(string sheetFileName)
        {
            var builder = new StringBuilder();
            builder.Append(@"        
        public void LoadFromJson()
        {
    #if UNITY_EDITOR
            @classList = Wayway.Engine.UnityGoogleSheet.Editor.Core.UgsEditorUtility
                .LoadFromJson<@Class>(""@sheetFileName"");    
    #endif              
        }
        
        public void LoadFromGoogleSpreadSheet()
        {
    #if UNITY_EDITOR
            Wayway.Engine.UnityGoogleSheet.Editor.Core.UgsExplorer
                .ParseWorkSheet(spreadSheetID, ""@Class"");

            LoadFromJson();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.Refresh();
    #endif
        }");
            
            GenerateForm = GenerateForm.Replace("@loadFunctions", builder.ToString());
            GenerateForm = GenerateForm.Replace("@sheetFileName", sheetFileName);
        }
        
        private void WriteClassSuffix(string suffix)
        {
            GenerateForm = GenerateForm.Replace("@suffix", suffix);
        }

        private void WriteDrawer()
        {
            var builder = new StringBuilder();
            builder.Append(@"        
#if UNITY_EDITOR
    #region Attribute Setting
    public class @ClassDrawer : OdinAttributeProcessor<@Class@suffix>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            switch (member.Name)
            {
                case ""LoadFromJson"":
                    attributes.Add(new PropertySpaceAttribute(5f, 0f));
                    attributes.Add(new ButtonAttribute(ButtonSizes.Medium));
                    break;
                case ""LoadFromGoogleSpreadSheet"":
                    attributes.Add(new ButtonAttribute(ButtonSizes.Medium));
                    break;
            }
        }
    }
    #endregion
#endif");
            
            GenerateForm = GenerateForm.Replace("@drawer", builder.ToString());
        }

        private static string GetCSharpRepresentation(Type t, bool trimArgCount)
        {
            if (t.IsGenericType)
            {
                var genericArgs = t.GetGenericArguments().ToList();

                return GetCSharpRepresentation(t, trimArgCount, genericArgs);
            }

            return t.Name;
        }

        private static string GetCSharpRepresentation(Type t, bool trimArgCount, IList<Type> availableArguments)
        {
            if (t.IsGenericType)
            {
                var value = t.Name;
                if (trimArgCount && value.IndexOf("`", StringComparison.Ordinal) > -1)
                {
                    value = value[..value.IndexOf("`", StringComparison.Ordinal)];
                }

                if (t.DeclaringType != null)
                    value = GetCSharpRepresentation(t.DeclaringType, trimArgCount, availableArguments) + "+" + value;
                var argString = "";
                var thisTypeArgs = t.GetGenericArguments();
                
                for (var i = 0; i < thisTypeArgs.Length && availableArguments.Count > 0; i++)
                {
                    if (i != 0) argString += ", ";
                    argString += GetCSharpRepresentation(availableArguments[0], trimArgCount);
                    availableArguments.RemoveAt(0);
                }
                
                if (argString.Length > 0)
                {
                    value += "<" + argString + ">";
                }

                return value;
            }

            return t.Name;
        }
        
        private static string ToCamelCasing(string original)
        {
            if (string.IsNullOrEmpty(original) || char.IsLower(original, 0))
            {
                return original;
            }

            return char.ToLowerInvariant(ToPascalCasing(original)[0]) + original[1..];
        }
        
        private static string ToPascalCasing(string original)
        {
            var invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            var whiteSpace = new Regex(@"(?<=\s)");
            var startsWithLowerCaseChar = new Regex("^[a-z]");
            var firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            var lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            var upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                .Split(new [] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));
            
            var result = string.Concat(pascalCase);

            return result.Length <= 2
                ? result.ToUpper()
                : result;
        }
    }
}
