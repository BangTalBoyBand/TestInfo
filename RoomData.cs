using UnityEngine;
using System;

namespace RoomListScripts.Test
{
    [System.Serializable]
    public class RoomData
    {
        public string roomId;
        public string title;
        public string creator;
        public int playerCount;
        public string likeRate;
        public string playCount;
        public float rating;
        public Difficulty difficulty;
        public string[] tags;
        public Sprite thumbnailSprite;
        public Color thumbnailColor;
        
        public RoomData(string title, string creator, int playerCount, string likeRate, 
            string playCount, float rating, Difficulty difficulty, string[] tags)
        {
            this.roomId = Guid.NewGuid().ToString();
            this.title = title;
            this.creator = creator;
            this.playerCount = playerCount;
            this.likeRate = likeRate;
            this.playCount = playCount;
            this.rating = rating;
            this.difficulty = difficulty;
            this.tags = tags;
            
            // 랜덤 색상 생성 (임시)
            this.thumbnailColor = new Color(
                UnityEngine.Random.Range(0.3f, 0.8f),
                UnityEngine.Random.Range(0.3f, 0.8f),
                UnityEngine.Random.Range(0.3f, 0.8f)
            );
        }
        
        public float GetLikePercentage()
        {
            string numberOnly = likeRate.Replace("%", "");
            if (float.TryParse(numberOnly, out float percentage))
            {
                return percentage;
            }
            return 0f;
        }
        
        public int GetPlayCountNumber()
        {
            string processed = playCount.Replace(",", "");
            
            if (processed.EndsWith("M"))
            {
                processed = processed.Replace("M", "");
                if (float.TryParse(processed, out float millions))
                {
                    return (int)(millions * 1000000);
                }
            }
            else if (processed.EndsWith("K"))
            {
                processed = processed.Replace("K", "");
                if (float.TryParse(processed, out float thousands))
                {
                    return (int)(thousands * 1000);
                }
            }
            else if (int.TryParse(processed, out int count))
            {
                return count;
            }
            
            return 0;
        }
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}
