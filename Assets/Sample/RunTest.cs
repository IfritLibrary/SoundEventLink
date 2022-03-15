using System;
using System.Collections;
using System.Collections.Generic;
using SoundEventLink;
using UnityEngine;

public class RunTest : MonoBehaviour
{
    [SerializeField] private EnumSoundEventLinkData _bgm;
    [SerializeField] private EnumSoundEventLinkData _se;

    private void OnGUI()
    {
        if (GUILayout.Button("PlayBGM"))
        {
            SoundEventLink.Runtime.SoundEventLink.Instance.Play(_bgm, transform.position);
        }

        if (GUILayout.Button("PlaySE"))
        {
            SoundEventLink.Runtime.SoundEventLink.Instance.Play(_se, transform.position);
        }
    }
}
