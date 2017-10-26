using UnityEngine;
using System.Collections;

public interface IPooled {

    void OnInstantiate();
    void OnUnSpawn();
}
