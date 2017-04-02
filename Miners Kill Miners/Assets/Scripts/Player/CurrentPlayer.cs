using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using DarkRift;
using System.Linq.Expressions;
using System.Linq;
using UnityEngine.UI;

namespace Roland
{
    public class CurrentPlayer : Singleton<CurrentPlayer>
    {
        PlayerDataContainer playerDataColleciton = null;
        bool loaded = false;
        //public Dictionary<int, Player> theActivePlayers = new Dictionary<int, Player>();
        Player thePlayer;
        public int OurID;
        public int AmountOfPlayers = 0;

        public int HealthPoints
        {
            get { return healthpoints; }
            set
            {
                healthpoints = value;
                CurrentPlayer.Instance.UpdateHealthPointInGame();
            }
        }
        public int DigPower
        {
            get { return digpower; }
            set 
            {
                digpower = value;
                DigPowerUI.text = digpower.ToString();
            }
        }

        private int healthpoints = 3;
        private int digpower = 1;

        //temp holder for our player. Since the player class only creates after ready is clicked
        //we need a place to store the weapons the user has.
        //this will be changed as we move into a full game and do not need to rely on scenes
        //so for now its like this, but eventually all the required data will be gotten
        //from the room scene. then we don't need to use this.
        public Dictionary<Items_e, int> AmountOfItems = new Dictionary<Items_e, int>();

        public List<EquipmentBase> AmountOfEquipments = new List<EquipmentBase>();
        public List<int> AmountOfBombs = null;
        public List<Image> HPIcons = new List<Image>();

        public Sprite HPIconSprite;

        Text theUIText;
        public Text DigPowerUI;

        int money;

        public GameSceneController controller;

        public int Money
        {
            set
            {
                money = value;
                UpdateText(money);
            }
            get
            {
                return money;
            }
        }

        public void GetHPIcons(Transform parent)
        {
            List<Transform> ParentTransforms = new List<Transform>();
            HPIcons.Clear();
            for(int i = 0; i < parent.childCount; i++)
            {
                ParentTransforms.Add(parent.GetChild(i));
            }
            for(int i = 0; i < ParentTransforms.Count; i++)
            {
                for(int k = 0; k < ParentTransforms[i].childCount; k++)
                {
                    HPIcons.Add(ParentTransforms[i].GetChild(k).GetComponent<Image>());
                }
            }
            UpdateHealthPointInGame(HealthPoints);
        }

        public void DeleteHorizontalLayout(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).GetComponent<HorizontalLayoutGroup>());
            }
        }

        public void UpdateHealthPointInGame(int currentHealth, bool LoseHp = false)
        {
            if(LoseHp)
            {
                //MEans we lose hp
                //Fire Particle
                if(currentHealth < 0)
                {
                    currentHealth = 0;
                }
                ExplosionParticleEffect.Instance.PositionParticleAndExplode(HPIcons[currentHealth].transform);
            }
            for (int i = 0; i < HPIcons.Count; i++)
            {
                HPIcons[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < currentHealth; i++)
            {
                HPIcons[i].gameObject.SetActive(true);
                if (!HPIcons[i].transform.parent.gameObject.activeInHierarchy)
                {
                    HPIcons[i].transform.parent.gameObject.SetActive(true);
                }
                HPIcons[i].sprite = HPIconSprite;
            }
        }

        public void UpdateHealthPointInGame()
        {
            UpdateHealthPointInGame(HealthPoints);
        }


        public bool BuyThings(int cost)
        {
            int temp = Money - cost;
            if(temp >= 0)
            {
                Money = temp;
                UpdateText(Money);
                return true;
            }
            else
            {
                return false;
            }
        }

        void UpdateText(int number)
        {
            if (!controller.GameHasStarted)
                theUIText = controller.MoneyShop;
            else
                theUIText = controller.MoneyGame;
            theUIText.text = "Money: $" + number.ToString();
        }


        protected CurrentPlayer()
        {
            if (!loaded)
            {
               // playerDataColleciton = PlayerDataContainer.Load(Path.Combine(Application.dataPath, "PlayerData.xml"));
                loaded = true;
            }
            //InitializeList();
          
        }
        ~CurrentPlayer()
        {
            //playerDataColleciton.Save(Path.Combine(Application.dataPath, "PlayerData.xml"));
        }


        public Player ThePlayer
        {
            get { return thePlayer; }
            set { thePlayer = value; }
        }
        


        public PlayerDataContainer GetPlayerDataCollection
        {
            get { return playerDataColleciton; }
        }

        public void AddPlayer(PlayerData AddThisPlayer)
        {
            playerDataColleciton.playerDatas.Add(AddThisPlayer);
        }

        public PlayerData GetPlayerData(string name)
        {
            for( int i = 0; i < playerDataColleciton.playerDatas.Count; i++)
            {
                if(playerDataColleciton.playerDatas[i].Name == name)
                {
                    return playerDataColleciton.playerDatas[i];
                }
            }
            return null;
        }

        public PlayerData[] GetAllPlayerDatas()
        {
            return playerDataColleciton.playerDatas.ToArray();
        }

        public string[] GetPlayerDataNames()
        {
            string[] nameArray = new string[playerDataColleciton.playerDatas.Count];
            for(int i = 0; i < playerDataColleciton.playerDatas.Count; i++)
            {
                nameArray[i] = playerDataColleciton.playerDatas[i].Name;
            }
            return nameArray;
        }

        void OnApplicationQuit()
        {
            //playerDataColleciton.Save(Path.Combine(Application.dataPath, "PlayerData.xml"));
        }

        void Awake()
        {
            if (!loaded)
            {
                playerDataColleciton = PlayerDataContainer.Load(Path.Combine(Application.dataPath, "PlayerData.xml"));
                loaded = true;
            }
            theUIText = GameObject.Find("Money").GetComponent<Text>();
        }

        public void InitializeList()
        {
            List<Items_e> AllItems = Items_e.GetValues(typeof(Items_e)).Cast<Items_e>().ToList();
            for (int i = 0; i < AllItems.Count; ++i)
            {
                AmountOfItems.Add(AllItems[i], 0);
            }
        }

        public void Start()
        {
            AmountOfEquipments = AmountOfEquipments.OrderBy(o => o.OrderID).ToList();
            if(AmountOfBombs != null)
            {
                for (int i = 0; i < AmountOfEquipments.Count; i++)
                {
                    //test
                    //AmountOfEquipments[i].Amount = AmountOfBombs[i];
                    AmountOfBombs.Add(AmountOfEquipments[i].Amount);
                }
            }
        }

        public void Restart()
        {
            //StartCoroutine(RestartGame());
            RestartGame();
        }

        void RestartGame()
        {
            //yield return new WaitForSeconds(1.0f);
            if (thePlayer != null)
            {
                Debug.Log("The Player is " + thePlayer);
                AmountOfBombs = new List<int>();
                for (int i = 0; i < AmountOfEquipments.Count; i++)
                {
                    AmountOfBombs.Add(AmountOfEquipments[i].Amount);
                }
                AmountOfEquipments.Clear();
            }
            healthpoints = 3;
            digpower = 1;
        }
    }
}