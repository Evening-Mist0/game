using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 序列化和反序列化Json时  使用的是哪种方案
/// </summary>
public enum JsonType
{
    JsonUtlity,
    LitJson,
}

/// <summary>
/// Json数据管理类 主要用于进行 Json的序列化存储到硬盘 和 反序列化从硬盘中读取到内存中
/// </summary>
public class JsonMgr
{
    private static JsonMgr instance = new JsonMgr();
    public static JsonMgr Instance => instance;

    private JsonMgr() { }

    //存储Json数据 序列化
    public void SaveData(object data, string fileName, JsonType type = JsonType.LitJson)
    {
        //确定存储路径
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        //序列化 得到Json字符串
        string jsonStr = "";
        switch (type)
        {
            case JsonType.JsonUtlity:
                jsonStr = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr = JsonMapper.ToJson(data);
                break;
        }
        //把序列化的Json字符串 存储到指定路径的文件中
        File.WriteAllText(path, jsonStr);
    }

    //读取指定文件中的 Json数据 反序列化
    public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new()
    {
        //确定从哪个路径读取
        //读取玩家游玩数据
        string path = Application.persistentDataPath + "/" + fileName + ".json";

        //不存在玩家游玩数据，读取默认数据
        if(!File.Exists(path))
           path = Application.streamingAssetsPath + "/" + fileName + ".json";

        //如果读写文件夹中都还没有 那就返回一个默认对象
        if (!File.Exists(path))
            return new T();

        //进行反序列化
        string jsonStr = File.ReadAllText(path);
        //数据对象
        T data = default(T);
        switch (type)
        {
            case JsonType.JsonUtlity:
                data = JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(jsonStr);
                break;
        }

        //把对象返回出去
        return data;
    }

    /// <summary>
    /// ScriptableObject 专用
    /// </summary>
    /// <typeparam name="T">要读取的SO类型</typeparam>
    /// <param name="fileName">Json文件名</param>
    /// <returns></returns>
    public T LoadScriptableObjectData<T>(string fileName) where T : ScriptableObject
    {
        // 攥写取存档数据路径（肉鸽存档、玩家进度）
        string savePath = Path.Combine(Application.persistentDataPath, fileName + ".json");

        string json = string.Empty;

        //读取存档，如果存在该存档路径，进行覆盖，如果不存在直接返回空（由于规则指定了SO要作为一个中转站交接数据，所以读取不到存档数据，就应当从Resources里面加载数据，而不是通过Json的StramingAsset文件夹的默认数据加载）
        if (File.Exists(savePath))
        {
            //如果存在，创建SO直接覆盖
            T obj = ScriptableObject.CreateInstance<T>();
            json = File.ReadAllText(savePath);

            //把JSON内容覆盖到SO（只会进行本次游戏的覆盖，重新游戏SO还是变为默认配置）
            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, obj);
            }
            Debug.Log($"[JsonMgr] 读取存档成功：{savePath}");
            return obj;
        }

        //如果存档不存在存档数据，通过Resource读取SO的默认数据（默认数据是通过SO，这里不能直接用Json获取） 注意：现在没有json格式的默认数据
        string resPath = "BaseCardScriptableObject/" + typeof(T).Name;
        T so = Resources.Load<T>(resPath);
        if (so != null)
        {
            Debug.Log($"[JsonMgr] 读取 Resources 默认数据：{resPath}");
            return so;
        }

        //如果Resource路径都找不到，说明命名规范，返回一个非持久化数据，并给予警告
        Debug.LogWarning($"[JsonMgr] Resources 也未找到：{resPath}，创建临时SO");
        return ScriptableObject.CreateInstance<T>();
    }
}
