// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Data.Json;

public class DataKeyedObjectLeaf : DataKeyedObject {

    public string otherProperty = "";
}

public class TestsData {

    public static void Advance(string name) {
        Debug.Log(name + "\r\n----------------------------------\r\n\r\n");
    }

    public static void RunTest() {

        Advance("Running tests...");
        
        ContentsConfig.contentRootFolder = "drawlabs";
        ContentsConfig.contentAppFolder = "game-drawlabs-brainball";
        ContentsConfig.contentDefaultPackFolder = "game-drawlabs-brainball-1";
        ContentsConfig.contentVersion = "1.0";
        ContentsConfig.contentIncrement = 2;
        
        Advance("Creating Contents cache paths");
        ContentPaths.CreateCachePaths();

        //Advance("TestGameCharacterSkin");
        //TestGameCharacterSkin();

        //Advance("TestGameCharacterSkinLoadData");
        //TestGameCharacterSkinLoadData();
        
        //Advance("TestGameState_LoadProfile");
        //TestGameState_LoadProfile();

        
        //Advance("TestGameState_SaveProfile");
        //TestGameState_SaveProfile();

        //Advance("TestGameProfileCharacter_GetCharacter");
        //TestGameProfileCharacter_GetCharacter();
        
        //Advance("TestGameProfileCharacter_GetCurrentCharacter");
        //TestGameProfileCharacter_GetCurrentCharacter();
        
        //Advance("TestGameProfileCharacter_currentCharacter");
        //TestGameProfileCharacter_currentCharacter();
                
        //Advance("TestGameProfileCharacter_currentProgress");
        //TestGameProfileCharacter_currentProgress();
        
        //Advance("TestGameColors_List");
        //TestGameColors_List();
        
        //Advance("TestGameColors_Code");
        //TestGameColors_Code();

        
        Advance("TestAppContentAssetModels_List");
        TestAppContentAssetModels_List();
    }

    public static void DumpObj(string name, string oname, object o) {        
        Debug.Log(string.Format("{0} : {1}  : {2} ", name, oname, o));
    }

    public static bool AssertEquals(string name, object a, object b) {
        string dataA = a.ToJson();
        string dataB = b.ToJson();
        bool equal = false;
        if(dataA == dataB) {            
            equal = true;
            Debug.Log(name + ": SUCCESS :" + equal);
        }
        else {       
            Debug.LogError(name + ": FAIL :" + equal);
        }

        DumpObj(name, "dataA", dataA);
        DumpObj(name, "dataB", dataB);

        return equal;
    }

    
    
    public static void TestAppContentAssetModels_List() {
        
        string name = "TestAppContentAssetModels_List";
        
        Debug.Log(name);
        
        GameState.LoadProfile();
        
        List<AppContentAssetModel> models = AppContentAssetModels.Instance.GetAll();
        DumpObj(name, "models", models);
        
        //AssertEquals(name, username, "Player");
        
        foreach(AppContentAssetModel model in models) {            
            Debug.Log("model:code:" + model.code);         
            Debug.Log("model:display_name:" + model.display_name);
        }
        
        DumpObj(name, "models.Count", models.Count);
    }
    
    public static void TestGameColors_List() {
        
        string name = "TestGameColors_List";
        
        Debug.Log(name);
        
        List<GameColor> colors = GameColors.Instance.GetAll();

        foreach(GameColor color in colors) {
            Color colorTo = ColorHelper.FromRGB(color.color.rgba);
            
            Debug.Log("color:code:" + color.code);
            
            Debug.Log("color:color:" + colorTo);
        }
                
        DumpObj(name, "colors.Count", colors.Count);
    }

    
    
    public static void TestGameColors_Code() {
        
        string name = "TestGameColors_Code";
        
        Debug.Log(name);
        
        GameColor color = GameColors.Instance.GetByCode("game-ucf-knights-gold");

        if(color != null) {
        
            Debug.Log("color:color:" + color.code);
            Debug.Log("color:color:" + color.GetColor());
        }
        else {
            
            Debug.Log("color:NOT FOUND:");
        }


    }

    public static void TestGameState_LoadProfile() {
        
        string name = "TestGameState_LoadProfile";
        
        Debug.Log(name);

        GameState.LoadProfile();

        string username = GameProfiles.Current.username;
            DumpObj(name, "username", username);

        AssertEquals(name, username, "Player");
    }

    public static void TestGameState_SaveProfile() {
        
        string name = "TestGameState_SaveProfile";
        
        Debug.Log(name);
        
        GameState.SaveProfile();
        
        string username = GameProfiles.Current.username;
        DumpObj(name, "username", username);
        
        AssertEquals(name, username, "Player");
    }
    
    public static void TestGameProfileCharacter_GetCharacter() {
        
        string name = "TestGameProfileCharacter_GetCharacter";
        
        Debug.Log(name);

        string characterCode = "default";
                
        GameProfileCharacterItem characterItem = GameProfileCharacters.Current.GetCharacter(characterCode);


        if(characterItem == null) {
            
            DumpObj(name, "characterItem:NULL", characterItem);
        }
        else {
            
            DumpObj(name, "characterItem:EXISTS", characterItem);
            
            DumpObj(name, "characterItem:characterCode", characterItem.characterCode);
            DumpObj(name, "characterItem:characterCostumeCode", characterItem.characterCostumeCode);
            DumpObj(name, "characterItem:code", characterItem.code);

            DumpObj(name, "characterItem:characterCode.profileRPGItem.GetAttack()", 
                    characterItem.profileRPGItem.GetAttack());
        }

        DumpObj(name, "characterItem", characterItem);

        //Debug.Break();
    }
    
    public static void TestGameProfileCharacter_GetCurrentCharacter() {
        
        string name = "TestGameProfileCharacter_GetCurrentCharacter";
        
        Debug.Log(name);
        
        string characterCode = "default";

        GameProfileCharacterItem item = GameProfileCharacters.Current.GetCurrentCharacter();

        item.characterCode = "testercode";
        DataAttribute d = new DataAttribute();
        d.code = "ddd";
        item.SetAttribute(d);

        
        DataAttribute a = new DataAttribute();
        a.code = "aaa";
        item.SetAttribute(a);
                
        if(item == null) {
            
            DumpObj(name, "item:NULL", item);
        }
        else {
            
            DumpObj(name, "item:EXISTS", item.ToJson());
            
            DumpObj(name, "item:characterCode", item.characterCode);
            DumpObj(name, "item:characterCostumeCode", item.characterCostumeCode);
            DumpObj(name, "item:code", item.code);
            
            //DumpObj(name, "characterItem:characterCode.profileCustomItem.code", 
            //        characterItem.profileCustomItem.code);
            //DumpObj(name, "characterItem:characterCode.profileRPGItem.GetAttack()", 
            //        characterItem.profileRPGItem.GetAttack());
        }
        
        DumpObj(name, "item", item);
        
        //Debug.Break();        
    }    
    
    public static void TestGameProfileCharacter_currentCharacter() {
        
        string name = "TestGameProfileCharacter_currentCharacter";
        
        Debug.Log(name);
        
        string characterCode = "default";
        
        GameProfileCharacterItem characterItem = GameProfileCharacters.currentCharacter;
        
        
        if(characterItem == null) {
            
            DumpObj(name, "characterItem:NULL", characterItem);
        }
        else {
            
            DumpObj(name, "characterItem:EXISTS", characterItem);
            
            DumpObj(name, "characterItem:characterCode", characterItem.characterCode);
            DumpObj(name, "characterItem:characterCostumeCode", characterItem.characterCostumeCode);
            DumpObj(name, "characterItem:code", characterItem.code);
            
            //DumpObj(name, "characterItem:characterCode.profileCustomItem.code", 
            //        characterItem.profileCustomItem.code);
            //DumpObj(name, "characterItem:characterCode.profileRPGItem.GetAttack()", 
            //        characterItem.profileRPGItem.GetAttack());
        }
        
        DumpObj(name, "characterItem", characterItem);
        
        //Debug.Break();        
    }

    
    public static void TestGameProfileCharacter_currentProgress() {
        
        string name = "TestGameProfileCharacter_currentCharacter";
        
        Debug.Log(name);
        
        string characterCode = "default";
        
        GameProfilePlayerProgressItem item = GameProfileCharacters.currentProgress;
        
        
        if(item == null) {
            
            DumpObj(name, "item:NULL", item);
        }
        else {
            
            DumpObj(name, "item:EXISTS", item);

            //DumpObj(name, "characterItem:characterCode.profileCustomItem.code", 
            //        characterItem.profileCustomItem.code);
            //DumpObj(name, "characterItem:characterCode.profileRPGItem.GetAttack()", 
            //        characterItem.profileRPGItem.GetAttack());
        }
        
        DumpObj(name, "characterItem", item);
        
        //Debug.Break();
        
    }


    // -----------------------------------------------------------------
    // GAME CHARACTER SKIN

    public static void TestGameCharacterSkin() {

        string name = "TestGameCharacterSkin";
        
        Debug.Log(name);
        
        GameCharacterSkin obj1 = new GameCharacterSkin();
        GameCharacterSkin obj2 = new GameCharacterSkin();
        
        obj1.active = true;
        
        obj1.display_name = "tester";
        
        string obj1Data = JsonMapper.ToJson(obj1);
                
        obj2 = JsonMapper.ToObject<GameCharacterSkin>(obj1Data);
        
        string obj2Data = JsonMapper.ToJson(obj2);

        AssertEquals(name, obj1, obj2);
    }
    
    public static void TestGameCharacterSkinLoadData() {
        
        string name = "TestGameCharacterSkinLoadData";
        
        Debug.Log(name);

        try {
            GameCharacterSkins.Instance.LoadData();

            Debug.Log(name + ":GameCharacterSkins:" + GameCharacterSkins.Instance.items.Count);
            Debug.Log(name + ":SUCCESS:" + true);
        }
        catch(Exception e) {

            Debug.Log(e);
        }
    }







    /*
    
    public static void TestDefault() {
        
        DataKeyedObjectLeaf leaf = new DataKeyedObjectLeaf();
        
        leaf.active = true;
        
        leaf.display_name = "tester";
        
        Debug.Log("DataKeyedObjectLeaf:leaf:display_name:" + leaf.display_name);
        Debug.Log("DataKeyedObjectLeaf:leaf:display_name2:" + leaf.Get(BaseDataObjectKeys.display_name));
        
        string leafData = JsonMapper.ToJson(leaf);
        
        Debug.Log("DataKeyedObjectLeaf:leafData:" + leafData);
        
        DataKeyedObjectLeaf leaf2 = new DataKeyedObjectLeaf();
        
        leaf2 = JsonMapper.ToObject<DataKeyedObjectLeaf>(leafData);
        
        Debug.Log("DataKeyedObjectLeaf:display_name:" + leaf2.display_name);
        
        string leaf2Data = JsonMapper.ToJson(leaf2);
        
        Debug.Log("DataKeyedObjectLeaf:leaf2Data:" + leaf2Data);
        
        
        AssertEquals("DataKeyedObjectLeaf", leaf, leaf2);
    }
    */
    
}



