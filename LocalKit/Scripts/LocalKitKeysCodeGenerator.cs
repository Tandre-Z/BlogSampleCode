using System.Collections.Generic;
using System.IO;
using System.Text;
using QFramework;
using UnityEditor;

public static class LocalKitKeysCodeGenerator
{
    private static string GeneratorKeyClass(List<string> fieldNames)
    {
        var code = new RootCode()
            .EmptyLine()
            .Namespace("LocalKit", ns =>
            {
                ns.Class("LocalKitKeys", string.Empty, false, false, cls =>
                {
                    foreach (var fieldNameValue in fieldNames)
                    {
                        var fieldName = fieldNameValue.ToUpper().Replace(' ', '_');
                        cls.Custom($"public const string {fieldName} = \"{fieldNameValue}\";");
                    }
                });
            });
        var stringBuilder = new StringBuilder();
        var codeWrite = new StringCodeWriter(stringBuilder);
        code.Gen(codeWrite);

        return stringBuilder.ToString();
    }

    public static void GeneratorKeyClassFile(List<string> fieldNames, string filePath)
    {
        var code = GeneratorKeyClass(fieldNames);
        //Debug.Log("filePath:" + filePath);
        //生成路径并创建文件
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        //写入文件
        File.WriteAllText(filePath, code);
        //刷新资源
        AssetDatabase.Refresh();
    }
}
