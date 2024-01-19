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
        
        username = ((TextBox)usernameInput.Control).Text;
        
        hostPortString = ((TextBox)hostPortInput.Control).Text;
        ((Label)hostIPText.Control).Text = NetworkManager.ipAddress;
        
        joinPortString = ((TextBox)joinPortInput.Control).Text;
        joinIPString = ((TextBox)joinIPInput.Control).Text;
    }

    private void Host()
    {
        Debug.Log("Calling MainMenuScript.Host()");

        if (!ushort.TryParse(hostPortString, out var port)) port = 7777;
        username ??= "Player";
        
        if (!NetworkManager.Instance.Host("username", 7777)) return;
        
        Debug.Log("NetworkManager.Host() = true.  Calling Level.LoadScene");
        
        SceneManager.Instance.LoadScene(1);
        
    }

    private void Join()
    {
        if (ushort.TryParse(joinPortString, out var port))
            NetworkManager.Instance.Connect(username, joinIPString, port);
    }

    private async void DisableButtons()
    {
        
    }
}
