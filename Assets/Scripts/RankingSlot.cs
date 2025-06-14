using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingSlot : MonoBehaviour
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text nicknameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text playTimeText;
    [SerializeField] private Text blockCountText;

    public void setRank(string rank, string nickname, string score, float playTime, string blockCount)
    {
        rankText.text = $"{rank}위";
        nicknameText.text = nickname;
        scoreText.text = $"{score}점";
        playTimeText.text = $"{(int)playTime / 60:00} : {(int)playTime % 60:00}";
        // playTimeText.text = $"{int.Parse(playTime) / 60:00} : {int.Parse(playTime) % 60:00}";
        blockCountText.text = $"{blockCount}개";
    }
}
