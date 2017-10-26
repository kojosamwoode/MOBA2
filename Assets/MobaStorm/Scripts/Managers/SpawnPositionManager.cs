using UnityEngine;
using System.Collections;

public class SpawnPositionManager : MonoSingleton<SpawnPositionManager> {
    [SerializeField]
    private Transform m_blueContainer;

    [SerializeField]
    private Transform m_redContainer;

    private Transform[] m_blueSpawnPoints;
    private Transform[] m_redSpawnPoints;
    // Use this for initialization
    void Awake () {
        m_blueSpawnPoints = m_blueContainer.GetComponentsInChildren<Transform>(true);
        m_redSpawnPoints = m_redContainer.GetComponentsInChildren<Transform>(true);

    }
    /// <summary>
    /// Call this method to get the spawn position for each player character.
    /// </summary>
    /// <param name="selectionSocket"> Current MobaPlayer SlotNumber</param>
    /// <returns>Returns the spawn position for the current player character</returns>
    public Transform GetSocketSpawnPosition(int selectionSocket)
    {
        if (selectionSocket > 4)
        {
            selectionSocket -= 5;
            return m_redSpawnPoints[selectionSocket];
        }

        return m_blueSpawnPoints[selectionSocket];
    }
}
