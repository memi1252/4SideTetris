using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BackEnd;
using Unity.VisualScripting;

public class RankingUI : MonoBehaviour
{
    public GameObject rankingPanel;
    [SerializeField] private GameObject rankingSlotPrefab;
    private int index = 0;

    
    public class UserData
    {
        public string name;
        public int point;
        public float playTime;
        public int blockCount;
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        index = 0;

        
        if (rankingPanel == null)
            rankingPanel = GameObject.Find("RankingPanel");

        
        // PlayerDataTransactionRead playerDataTransactionRead = new PlayerDataTransactionRead();
        // playerDataTransactionRead.AddGetOtherData()
        var bro = Backend.GameData.Get("USER_DATA", new Where());
        if (bro.IsSuccess())
        {
            LitJson.JsonData gameDataJson = bro.FlattenRows();
            if (gameDataJson.Count > 0)
            {
                
                List<UserData>  userDataList = new List<UserData>();
                
                
               UserData userData = new UserData();
               for (int i = 0; i < gameDataJson.Count; i++)
               {
                   userData.name = gameDataJson[i]["name"].ToString();
                     userData.point = int.Parse(gameDataJson[i]["point"].ToString());
                     userData.playTime = float.Parse(gameDataJson[i]["playTime"].ToString());
                     userData.blockCount = int.Parse(gameDataJson[i]["blockCount"].ToString());
                     userDataList.Add(userData);
                     userData = new UserData();
               }
               var sortedList = userDataList.OrderByDescending(user => user.point).ToList();
               
               
               
               
                //UI에 표시
                foreach (var user in sortedList)
                {
                    if(user.point == 0) 
                        continue; 
                    GameObject slot = Instantiate(rankingSlotPrefab);
                    slot.transform.SetParent(rankingPanel.transform, false);
                    slot.GetComponent<RankingSlot>().setRank(
                        (++index).ToString(),
                        user.name,
                        user.point.ToString(),
                        user.playTime,
                        user.blockCount.ToString()
                    );
                }
                
            }
            else
            {
                Debug.LogWarning("랭킹 데이터가 존재하지 않습니다.");
            }
        }
        else
        {
            Debug.LogError("랭킹 데이터 조회 실패: " + bro);
        }
    }

    public void Hide()
    {
        foreach (var slot in transform.GetComponentsInChildren<RankingSlot>())
        {
            Destroy(slot.gameObject);
        }
        gameObject.SetActive(false);
    }
}
