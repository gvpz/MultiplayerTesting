using System;
using System.Collections.Generic;
using FlaxEditor;
using FlaxEngine;

namespace Game;

/// <summary>
/// SceneManager Script.
/// </summary>
public class SceneManager : GamePlugin
{

    public override void Initialize()
    {
        //Scripting.FixedUpdate += FixedUpdate;
    }
    
    public void LoadScene(Guid scene, Scene currentScene)
    {
        Level.LoadScene(scene);
        Level.UnloadScene(currentScene);
    }

    public void FixedUpdate()
    {
        
    }

    public static SceneManager instance;

    public static SceneManager Instance
    {
        get
        {
            if (instance == null)
                instance = PluginManager.GetPlugin<SceneManager>();
            return instance;
        }
    }

}
