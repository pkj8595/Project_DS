using UnityEngine;

[CreateAssetMenu(fileName = "SOProductionTable", menuName = "Data/SOProductionTable", order = 1)]
public class SOProductionTable : SOData
{
    public string Name;
    public string Desc;
    public SOItemKeyValue[] Items;
    public float ProductTime;
}

[System.Serializable]
public class SOItemKeyValue
{
    public SOBase data;
    public int amount;
}
