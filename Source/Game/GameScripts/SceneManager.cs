using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// SceneManager Script.
/// </summary>
public class SceneManager : GamePlugin
{
    public Guid currentScene = Guid.Empty;
    public string[] sceneStrings = {"1267625e-4d26-483a-9f29-631bc6b7c248", "0e424818-701b-4caa-adaa-13556ee921f9"};
    public List<Guid> scenes = new List<Guid>();

    public override void Initialize()
    {
        foreach (var id in sceneStrings)
        {
            scenes.Add(Guid.Parse(id));
        }
    }
    
    public void LoadScene(int scene)
    {
        Level.LoadSceneAsync(scenes[scene]);
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
