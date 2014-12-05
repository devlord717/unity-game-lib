using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class BaseGameLevels<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameLevels<T> instance;
    private static object syncRoot = new Object();
    public static string BASE_DATA_KEY = "game-level-data";
    public static float gridHeight = 1f;
    public static float gridWidth = 80f;
    public static float gridDepth = 40f;
    public static float gridBoxSize = 4f;
    public static bool centeredX = true;
    public static bool centeredY = false;
    public static bool centeredZ = true;

    public static T BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new T();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseGameLevels<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLevels<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLevels() {
        Reset();
    }

    public BaseGameLevels(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public virtual T GetDefaultLevel() {
        T levelReturn = new T();
        foreach (T level in GetAll()) {
            return level;
        }
        return levelReturn;
    }

    public void PrepareDefaultData() {
        
        /*
        items = new List<GameLevel>();
    
        SetGameLevel(
            "airship", 
            "Airship", 
            "Airship",
            "The airship acts as the game’s central hub.", 
            "airship",
            0,
            0);
        
        LogUtil.Log("GameLevels:" + JsonMapper.ToJson(items));
        */
    }
    
    public void SetGameLevel(string code, string name, string displayName, 
                             string description, string type, int sortIndex, int typeSortIndex) {
        bool found = false;
        
        for (int i = 0; i < GameLevels.Instance.items.Count; i++) {
            if (GameLevels.Instance.items[i].code.ToLower() == code.ToLower()) {
                GameLevels.Instance.items[i].code = code;
                GameLevels.Instance.items[i].name = name;
                GameLevels.Instance.items[i].display_name = displayName;
                GameLevels.Instance.items[i].description = description;
                GameLevels.Instance.items[i].type = type;
                GameLevels.Instance.items[i].sort_order = sortIndex;
                GameLevels.Instance.items[i].sort_order_type = typeSortIndex;               
                found = true;
                break;
            }
        }
        
        if (!found) {
            GameLevel obj = new GameLevel();
            obj.active = true;
            obj.SetAttributeStringValue("default", "default");

            obj.code = code;
            obj.description = description;
            obj.display_name = displayName;
            obj.game_id = "11111111-1111-1111-1111-111111111111";
            obj.key = code;
            obj.name = name;
            obj.order_by = "";
            obj.sort_order = sortIndex;
            obj.sort_order_type = typeSortIndex;
            obj.status = "";
            obj.type = "default";
            obj.uuid = UniqueUtil.Instance.CreateUUID4();
            GameLevels.Instance.items.Add(obj);
        }
    }
    
    public void SetGameLevel(GameLevel gameLevel) {
        bool found = false;
        
        for (int i = 0; i < items.Count; i++) {
            if (GameLevels.Instance.items[i].code.ToLower() == gameLevel.code.ToLower()) {
                GameLevels.Instance.items[i] = gameLevel;
                found = true;
                break;
            }
        }
        
        if (!found) {
            GameLevels.Instance.items.Add(gameLevel);
        }
    }
    
    /*
    public override GameLevel GetById(string levelCode) {
        foreach(GameLevel level in GetAll()) {
            if(level.code == levelCode) {
                return level;
            }
        }
        return null;
    }
    */
    
    public List<GameLevel> GetByWorldId(string worldCode) {
        List<GameLevel> filteredLevels = new List<GameLevel>();
        foreach (GameLevel level in GameLevels.Instance.GetAll()) {
            if (level.world_code == worldCode) {
                filteredLevels.Add(level);
            }
        }
        return filteredLevels;
    }
    
    public void ReloadLevel() {
        ReloadLevel(GameLevels.Current.code);
    }
    
    public void ReloadLevel(string levelCode) {
        GameLevelItems.Instance.Load(levelCode);
    }
    
    public void ChangeCurrentAbsolute(string code) {
        GameLevels.Current.code = "changeme";
        ChangeCurrent(code);
    }
    
    public void ChangeCurrent(string code) {
        if (GameLevels.Current.code != code) {
            GameLevels.Current = GameLevels.Instance.GetById(code);
            string originalCode = code;
            if (string.IsNullOrEmpty(GameLevels.Current.code)) {
                //code = "level-" + code;
                GameLevels.Current = GameLevels.Instance.GetById(code);
            }
            
            if (string.IsNullOrEmpty(GameLevels.Current.code)) {
                // TODO not found add?
                GameLevel gameLevel = new GameLevel();
                gameLevel.code = code;
                gameLevel.date_created = DateTime.Now;
                gameLevel.date_modified = DateTime.Now;
                gameLevel.description = originalCode;
                gameLevel.display_name = code;
                gameLevel.name = originalCode;
                gameLevel.game_id = ContentsConfig.contentAppFolder;
                gameLevel.key = originalCode;
                gameLevel.world_code = GameWorlds.Current.code;
                GameLevels.Instance.items.Add(gameLevel);
            }
            
            if (string.IsNullOrEmpty(GameLevels.Current.code)) {
                GameLevels.Current = GameLevels.Instance.GetById(code);
            }


            // Update World

            if(!string.IsNullOrEmpty(GameLevels.Current.world_code)) {
                GameWorlds.Instance.ChangeCurrent(GameLevels.Current.world_code);
            }
            
            LogUtil.Log("Changing Level: code:" + code);    
        }
    } 

    //

    public static GameLevelGridData GetLevelGridTerrains(GameLevelGridData dataItems, List<GameDataTerrainPreset> presets) {
        
        foreach(GameDataTerrainPreset terrainDataItem in presets) {
            
            GamePreset terrainPreset = GamePresets.Instance.GetById(terrainDataItem.code);
            
            if(terrainPreset != null) {
                
                GamePresetItem terrainPresetItem = terrainPreset.GetItemRandomByProbability(terrainPreset.data.items);
                
                if(terrainPresetItem != null) {
                    dataItems = GameLevelGridData.AddAssets(dataItems, terrainPresetItem.code, 1);
                }
            }
        }
        
        return dataItems;
    }
    
    public static GameLevelGridData GetLevelGridAssets(GameLevelGridData dataItems, List<GameDataAssetPreset> presets) {        
        
        foreach(GameDataAssetPreset assetDataItem in presets) {
            
            int minAssetLimit = (int)assetDataItem.min;
            int maxAssetLimit = (int)assetDataItem.max;

            int randomAssetLimit = UnityEngine.Random.Range(minAssetLimit, maxAssetLimit);
            
            int totalAssetLimit = 0;

            bool isNestedLimitsType = assetDataItem.Get(BaseDataObjectKeys.data_type) == "nested_limits" ? true : false;

            GamePreset assetPreset = GamePresets.Instance.GetById(assetDataItem.code);
            
            if(assetPreset != null) {

                if(!isNestedLimitsType) {

                    for(int i = 0; i < randomAssetLimit; i++) {

                        GamePresetItem presetItem = assetPreset.GetItemRandomByProbability(assetPreset.data.items);
                        
                        if(presetItem != null) {
                            int amount = 1;
                            dataItems = GameLevelGridData.AddAssets(dataItems, presetItem.code, amount);
                            totalAssetLimit += amount;
                        }
                    }
                }
                else {

                    foreach(GamePresetItem presetItem in assetPreset.data.items) {
                    
                        int amount = UnityEngine.Random.Range((int)presetItem.min, (int)presetItem.max);
                        totalAssetLimit += amount;
                        
                        dataItems = GameLevelGridData.AddAssets(dataItems, presetItem.code, amount);
                        
                        if(totalAssetLimit > maxAssetLimit) {
                            // Too many for this set to add more...
                            break;
                        }
                    }
                }
            }
        }
        
        return dataItems;
    }
}

public class BaseGameLevelKeys {
    public static string LEVEL_INITIAL_DIFFICULTY = "initial-diff";
    public static string LEVEL_SPONSOR_NAME = "sponsor";
    public static string LEVEL_SPONSOR_IMAGE = "sponsor-img";
}

public class BaseGameLevel : GameDataObject {

    public virtual GameDataObjectItem data {
        get {
            return Get<GameDataObjectItem>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<GameDataObjectItem>(BaseDataObjectKeys.data, value);
        }
    } 

    public BaseGameLevel() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}