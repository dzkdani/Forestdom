using System;
using UnityEngine;

public class RewardedAdManager : MonoBehaviour
{
    private static RewardedAdManager _instance;
    public static RewardedAdManager instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    DateTime current;
    DateTime previous;
    TimeSpan timeDifference;
    long temp = 0;

    private void SaveDateTime()
    {
        PlayerPrefs.SetString("LastDateTime", DateTime.Now.ToBinary().ToString());
        Debug.Log("Time Saved : " + DateTime.Now);
    }

    int rewardCount = 0;
    public void RewardRecieved()
    {
        rewardCount -= 1;
        if (rewardCount == 0)
        {
            SaveDateTime();
        }
    }

    public bool isRewardedAvailable()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else
        {
            if (CheckRewardCount())
            {
                if (rewardCount < 3 && Convert.ToInt32(timeDifference.TotalHours) % 8 == 0)
                {
                    int refreshedCount = Convert.ToInt32(timeDifference.TotalHours) / 8;
                    rewardCount += refreshedCount;
                }
                else
                {
                    rewardCount = 3;
                }
                return rewardCount > 0;
            }
            else
            {
                return rewardCount > 0;
            }
        }
    }

    bool CheckRewardCount()
    {
        if (PlayerPrefs.GetString("LastDateTime", "0") == "0")
        {
            rewardCount = 3;

            return false;
        }
        else
        {
            temp = Convert.ToInt64(PlayerPrefs.GetString("LastDateTime"));

            previous = DateTime.FromBinary(temp);
            Debug.Log("LastDateTime : " + previous);

            timeDifference = current.Subtract(previous);
            Debug.Log("Time Difference : " + timeDifference);

            return true;
        }
    }
}
