using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class SpawningPool 
{
    [SerializeField] int _monsterCount = 0;

    [SerializeField] List<GameObject> _gateList;
    [SerializeField] readonly Queue<int> _enemyQueue = new Queue<int>();
    Action _endWaveAction;

    public void Init()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    public void AddMonsterCount(int value) 
    {
        _monsterCount += value; 
    }

    public void StartWaveEnemySpawn(Data.WaveData data)
    {
        //wave 데이터의 캐릭터 코드를 모두 큐에 삽입
        for (int i = 0; i < data.arr_characterNum.Length; i++)
        {
            for (int j = 0; j < data.arr_charAmount[i]; j++)
            {
                _enemyQueue.Enqueue(data.arr_characterNum[i]);
            }
        }

        if (0 < _gateList.Count)
            RunSpawnWave().Forget();
        else
            Debug.LogError("not find gate");
    }

    async UniTaskVoid RunSpawnWave()
    {
        await UniTask.NextFrame();

        while(_enemyQueue.Count > 0)
        {
            _monsterCount++;
            PawnBase obj = Managers.Game.SpawnPawn(_enemyQueue.Dequeue(), Define.ETeam.Enemy);
            int gateIndex = UnityEngine.Random.Range(0, _gateList.Count);
            obj.transform.position = _gateList[gateIndex].transform.position;
            if (obj.transform.position == Vector3.zero)
            {
                Debug.LogError($"gateIndex :{gateIndex} _gateCount :{_gateList.Count} \n스폰 포인트가 (0,0,0)입니다.\n{System.Environment.StackTrace}");
            }
            await UniTask.Delay(1000);
        }

        while (_monsterCount > 0)
        {
            await UniTask.Delay(1000);
        }

        EndWave();
    }

    public void EndWave()
    {
        _endWaveAction?.Invoke();
    }

    public void SetEndWaveAction(Action action)
    {
        _endWaveAction -= action;
        _endWaveAction += action;
    }

}
