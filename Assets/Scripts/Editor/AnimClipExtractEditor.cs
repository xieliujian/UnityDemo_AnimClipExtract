
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AnimClipExtractEditor : Editor
{
    private const string ANIM_PATH = "Assets/Art/3D/Characters/";
    private const string PREFABS_PATH = "Assets/Prefabs/3D/Characters/";
    private const string ANIMCLIP_PATH = "/AnimClips/";
    private const string FBX_SUFFIX = ".fbx";
    private const string ANIM_SUFFIX = ".anim";
    private const string PREAFAB_SUFFIX = ".prefab";
    private const string INVALID_ANIM_NAME = "__preview__";

    private const string FILE_ERROR = "不能选中动画文件生成！";

    [MenuItem("Game/Anim/ExtractAll")]
    static void ExtractAll()
    {

    }

    [MenuItem("Game/Anim/ExtractOne")]
    static void ExtractOne()
    {
        if (Selection.objects.Length == 0)
            return;

        Object obj = Selection.objects[0];
        if (obj == null)
            return;

        GenerateAnim(obj.name);
    }

    /// <summary>
    /// 生成一个动画资源
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    private static void GenerateAnim(string fbxname)
    {
        SplitAnimClip(fbxname);
        CreatePrefab(fbxname);
    }

    private static void SplitAnimClip(string fbxname)
    {
        string srcpath = ANIM_PATH + fbxname;
        string clippath = PREFABS_PATH + fbxname + ANIMCLIP_PATH;

        DirectoryInfo dirinfo = new DirectoryInfo(srcpath);
        if (!dirinfo.Exists)
        {
            Debug.LogError(FILE_ERROR);
            return;
        }

        dirinfo = new DirectoryInfo(clippath);
        if (!dirinfo.Exists)
        {
            Directory.CreateDirectory(clippath);
        }

        var files = Directory.GetFiles(srcpath, "*.fbx");
        foreach (var file in files)
        {
            if (!file.Contains("@"))
                continue;

            Object[] clipobjs = AssetDatabase.LoadAllAssetsAtPath(file);
            if (clipobjs.Length <= 0)
                continue;

            foreach (var clipobj in clipobjs)
            {
                AnimationClip srcclip = clipobj as AnimationClip;
                if (srcclip == null)
                    continue;

                if (srcclip.name.Contains(INVALID_ANIM_NAME))
                    continue;

                string dstclippath = clippath + srcclip.name + ANIM_SUFFIX;
                AnimationClip dstclip = AssetDatabase.LoadAssetAtPath(dstclippath, typeof(AnimationClip)) as AnimationClip;
                if (dstclip != null)
                    AssetDatabase.DeleteAsset(dstclippath);

                AnimationClip tempclip = new AnimationClip();
                EditorUtility.CopySerialized(srcclip, tempclip);
                AssetDatabase.CreateAsset(tempclip, dstclippath);
            }
        }
    }

    private static void CreatePrefab(string fbxname)
    {
        string srcpath = ANIM_PATH + fbxname + "/" + fbxname + FBX_SUFFIX;
        string dstpath = PREFABS_PATH + fbxname + "/" + fbxname + PREAFAB_SUFFIX;
        GameObject obj = AssetDatabase.LoadAssetAtPath(srcpath, typeof(GameObject)) as GameObject;
        PrefabUtility.CreatePrefab(dstpath, obj);
    }
}



