using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Networking;

namespace Game;

/// <summary>
/// MainMenuScript Script.
/// </summary>
public class MainMenuScript : Script
{
    private UIControl usernameInput;
    
    private UIControl hostButton;
    private UIControl hostPortInput;
    private UIControl hostIPText;
    
    private UIControl joinButton;
    private UIControl joinPortInput;
    private UIControl joinIPInput;

    private string username;
    private string hostPortString;
    private string joinPortString;
    private string joinIPString;

    public SceneReference testBed;
    
    public override void OnStart()
    {
        usernameInput = Actor.Scene.FindActor<UIControl>("UsernameInput");
        hostButton = Actor.Scene.FindActor<UIControl>("HostPlayButton");
        hostPortInput = Actor.Scene.FindActor<UIControl>("HostPortInput");
        hostIPText = Actor.Scene.FindActor<UIControl>("HostIPLabel");
        joinButton = Actor.Scene.FindActor<UIControl>("JoinPlayButton");
        joinPortInput = Actor.Scene.FindActor<UIControl>("JoinPortInput");
        joinIPInput = Actor.Scene.FindActor<UIControl>("JoinIPInput");
        
        ((Button)hostButton.Control).Clicked += Host;
        ((Button)joinButton.Control).Clicked += Join;
    }

    public override void OnUpdate()
    {
        Screen.CursorLock = CursorLockMode.None;
        Screen.CursorVisible = true;
        
        username = ((TextBox)usernameInput.Control).Text;
        
        hostPortString = ((TextBox)hostPortInput.Control).Text;
        ((Label)hostIPText.Control).Text = NetworkManager.ipAddress;
        
        joinPortString = ((TextBox)joinPortInput.Control).Text;
        joinIPString = ((TextBox)joinIPInput.Control).Text;
    }

    private void Host()
    {
        if (!ushort.TryParse(hostPortString, out var port)) port = 7777;
        username ??= "Player";
        
        if (!NetworkManager.Instance.Host(username, port)) return;
        SceneManager.Instance.LoadScene(testBed.ID, Level.GetScene(0));
    }

    private void Join()
    {
        if (!ushort.TryParse(joinPortString, out var port)) port = 7777;
        NetworkManager.Instance.Connect(username, joinIPString, port);
    }
}
