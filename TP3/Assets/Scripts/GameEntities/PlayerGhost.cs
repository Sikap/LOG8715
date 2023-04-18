using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerGhost : NetworkBehaviour
{
    [SerializeField] 
    private Player m_Player;
    [SerializeField] 
    private SpriteRenderer m_SpriteRenderer;

    public override void OnNetworkSpawn()
    {
        // L'entite qui appartient au client est recoloriee en rouge
        if (IsOwner)
        {
            m_SpriteRenderer.color = Color.red;
        }
    }

    private void Update()
    {
        if(IsServer || (IsClient && !IsOwner))
        {
            // Player.Snapshot currentSnapshot = m_Player.Position;
            transform.position = m_Player.Position.vector;
        }
        if(IsClient && IsOwner)
        {
            transform.position = m_Player.local_Position;
        }
    }

}
