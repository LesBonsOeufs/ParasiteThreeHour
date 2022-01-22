///-----------------------------------------------------------------
/// Author : Gabriel Bernabeu
/// Date : 22/01/2022 13:01
///-----------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LesBonsOeufs.ParasiteThreeHour
{
    public delegate void KeyControllerEventHandler(KeyController sender);

    public class KeyController
    {
        private KeyCode digKey;
        private KeyCode screamKey;
        public bool isDigging;

        public event KeyControllerEventHandler OnScream;

        public void SetKeys(KeyCode dig, KeyCode scream)
        {
            digKey = dig;
            screamKey = scream;
        }

        public void CheckInputs()
        {
            isDigging = Input.GetKey(digKey);

            if (Input.GetKeyDown(screamKey))
                OnScream?.Invoke(this);
        }
    }
}