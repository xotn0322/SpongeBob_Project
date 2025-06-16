using System.Collections.Generic;
using System;

public class EnemyData
{
    public string id;

    public int Hp;

    public EEnemyName EnemyName =>
        Enum.TryParse(id, out EEnemyName result) ? result : throw new Exception($"Unknown id: {id}");
}

public class EnemyDataList
{
    public List<EnemyData> enemies;
}