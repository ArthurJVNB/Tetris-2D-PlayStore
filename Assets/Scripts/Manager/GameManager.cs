using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        //static GameManager instance;

        [SerializeField] GameplayManager gameplayManager;

        //private void Awake()
        //{
        //    if (instance == null)
        //    {
        //        instance = this;
        //        DontDestroyOnLoad(gameObject);
        //    }
        //    else
        //    {
        //        Destroy(gameObject);
        //    }
        //}

        private void Start()
        {
            if (gameplayManager)
                gameplayManager.StartGame();
        }
    }
}