using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWidget : Widget
{
    private Button ExitButton   = null;
    private Slider VolumeSlider = null;


    protected override void Awake()
    {
        base.Awake();

        // 1. GET REFERENCES
        var a = GetComponentsInChildren<Slider>(true);
        VolumeSlider = a[0];
 
        var b = GetComponentsInChildren<Button>(true);
        ExitButton = b[0];
        

        // 2. EVENT SUBSCRIPTION
        VolumeSlider.onValueChanged.AddListener(UpdateVolume);
        ExitButton  .onClick       .AddListener(Close);
    }


    private void UpdateVolume(float Value)
    {
        Game.Volume = Value;
    }
    private void Close()
    {
        Widget.RemoveWidget(this);
    }
}
