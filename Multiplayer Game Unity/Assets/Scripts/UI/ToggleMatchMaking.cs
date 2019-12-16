using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMatchMaking : MonoBehaviour
{
    public GameObject MMUI;
    public GameObject NMMUI;
    bool matchmaking = false;
    // Start is called before the first frame update
    public void ToggleUI()
    {
        matchmaking = !matchmaking;

        MMUI.SetActive(matchmaking);
        NMMUI.SetActive(!matchmaking);

    }
}
