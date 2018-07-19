using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ReflectionProtectionInjector
{

    private const string PREF_NAME = "ReflectProtect_";

    private const string INJECT_TEXT = "if (RuntimeMethodSearcher.FindReflectionCalls()) return ";

    [ModifyCode]
    public static void InjectAttributes()
    {
        var types = AssemblyUtils.GetTypesWithMethodAttribute<ReflectionProtectAttribute>();
        var paths = GetInjectablePaths(types);

        for (int i = 0; i < paths.Count; i++)
        {
            string path = paths[i];
            string text = File.ReadAllText(path);
            string modifiedText = GetModifiedText(types, text);

            SaveData(path, modifiedText);

            PlayerPrefs.SetString(PREF_NAME + i, path);
            PlayerPrefs.SetString(path, text);
        }
    }

    [RestoreCode]
    public static void RestoreOriginalText()
    {
        for (int i = 0; ; i++)
        {
            if (!PlayerPrefs.HasKey(PREF_NAME + i))
                break;

            string path = PlayerPrefs.GetString(PREF_NAME + i);
            string text = PlayerPrefs.GetString(path);

            SaveData(path, text);
        }

        ClearPrefs();
    }

    private static void ClearPrefs()
    {
        for (int i = 0; ; i++)
        {
            if (!PlayerPrefs.HasKey(PREF_NAME + i))
                return;
            else
                PlayerPrefs.DeleteKey(PREF_NAME + i);
        }
    }

    private static void SaveData(string path, string text)
    {
        File.WriteAllText(path, text);
        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh();
    }

    private static string GetModifiedText(List<Type> typeList, string fileText)
    {
        int index = 0;
        string typeName;

        do
        {
            typeName = FindType(fileText, ref index);
            fileText = AddReflectionProtection(typeList, typeName, fileText, ref index);
        } while (index < fileText.Length && !string.IsNullOrEmpty(typeName));

        return fileText;
    }

    private static string AddReflectionProtection(List<Type> typeList, string typeName, string fileText, ref int index)
    {
        if (!typeList.Select(t => t.ToString()).ContainsIgnoreCase(typeName))
            return fileText;

        Type type = typeList.Single(t => t.ToString().EqualsIgnoreCase(typeName));
        List<ReflectionProtectAttribute> reflectionAttributes = new List<ReflectionProtectAttribute>();

        foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
            if (Attribute.IsDefined(method, typeof(ReflectionProtectAttribute)))
                reflectionAttributes.Add(method.GetCustomAttribute<ReflectionProtectAttribute>());

        string modifiedText = fileText;
        foreach (var attribute in reflectionAttributes)
        {
            int attributeStart = modifiedText.IndexOf("ReflectionProtect", index, modifiedText.Length - index) - 1;
            int attributeEnd = attributeStart;

            do
            {
                attributeEnd = modifiedText.IndexOf(']', attributeEnd, modifiedText.Length - attributeEnd) + 1;
            } while (modifiedText[attributeEnd - 2] == '[');

            modifiedText = modifiedText.Remove(attributeStart, attributeEnd - attributeStart);

            for (int j = 0; j < attributeEnd - attributeStart; j++)
                modifiedText = modifiedText.Insert(attributeStart, " ");

            var methodStart = modifiedText.IndexOf('{', attributeEnd, modifiedText.Length - attributeEnd);
            modifiedText = modifiedText.Insert(methodStart + 1, INJECT_TEXT + attribute.ReturnValue + ";");
        }

        return modifiedText;
    }

    private static string FindType(string fileText, ref int index)
    {
        int structIndex = fileText.IndexOf(" struct ", index, fileText.Length - index);
        int classIndex = fileText.IndexOf(" class ", index, fileText.Length - index);

        if (structIndex < 0 && classIndex < 0)
            return null;

        index = classIndex > 0 ? classIndex + 6 : structIndex + 7;

        int classStart = fileText.IndexOf('{', index, fileText.Length - index) - 1;
        int inheritStart = fileText.IndexOf(':', index, fileText.Length - index) - 1;
        int typeEnd = inheritStart > 0 && inheritStart < classStart ? inheritStart : classStart;

        return fileText.Substring(index, typeEnd - index).Trim();
    }

    private static List<string> GetInjectablePaths(List<Type> injectableTypes)
    {
        List<string> validTypes = injectableTypes.Select(type => type.ToString()).ToList();
        List<string> paths = new List<string>();

        foreach (string assetPath in AssetDatabase.GetAllAssetPaths())
        {
            if (assetPath.EndsWith(".cs"))
            {
                string removedEnd = assetPath.Remove(assetPath.Length - 3, 3);
                string typeName = removedEnd.Remove(0, removedEnd.LastIndexOf('/') + 1);

                if (validTypes.Contains(typeName) && !paths.Contains(assetPath))
                    paths.Add(assetPath);
            }
        }

        return paths;
    }
}
