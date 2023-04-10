using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CircleGhost : NetworkBehaviour
{
    [SerializeField]
    private MovingCircle m_MovingCircle;
    private  SnapshotArray snapshot;
    private CircleSpawner m_CircleSpawner;
    private GameState m_GameState;
    private Vector2 localPosition;
    private Vector2 localVelocity;


    private void Awake()
    {
        m_CircleSpawner = FindObjectOfType<CircleSpawner>();
        m_GameState = FindObjectOfType<GameState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            snapshot = new SnapshotArray((ulong)m_CircleSpawner.NbCircles);
            localPosition = m_MovingCircle.Position;
            localVelocity = m_MovingCircle.Velocity;
        }
    }

    private void Update()
    {
        if(IsServer)
        {
            transform.position = m_MovingCircle.Position;
        }
        if(IsClient)
        {
            transform.position = localPosition;
        }
        
    }

    private void FixedUpdate()
    { 
        if(IsClient)   
        {       
            //Prediction local + sauvgarde de la prediction.           
            //Debug.Log("Local prediction of NetworkObject " + NetworkObjectId + " for tick " + NetworkUtility.GetLocalTick());
            localPrediction();        
            snapshot.AddSnapshot(NetworkObjectId, NetworkUtility.GetLocalTick(), localPosition);
            
            //Réconciliation avec serveur si ancienne prediction est fausse
            Vector2 serverPosition = m_MovingCircle.Position;
            
            float delay =  (m_GameState.CurrentRTT/2) * NetworkUtility.GetLocalTickRate();
            int serverTick = Mathf.RoundToInt(NetworkUtility.GetLocalTick() - delay);            

            Vector2 correspondingTickLocalPosition = snapshot.GetSnapshotValue(NetworkObjectId, serverTick);

            if(serverPosition != correspondingTickLocalPosition)
            {
                localPosition = serverPosition;
                localVelocity = m_MovingCircle.Velocity;        
                //Debug.Log("NEED CORRECTION for NetworkObject " + NetworkObjectId + " for tick " + serverTick);    
                snapshot.AddSnapshot(NetworkObjectId, serverTick, localPosition);
                for(int i = 0; i < delay; i++)
                {
                    //Debug.Log("CORRECTION of NetworkObject " + NetworkObjectId + " for tick " +  (serverTick + 1 + i));    
                    localPrediction();
                    snapshot.AddSnapshot(NetworkObjectId, serverTick + 1 + i, localPosition);
                }
            }
        }
    }

    
    private void localPrediction()
    {

        // Mise a jour de la position du cercle selon sa vitesse                    
        float localRaduis = m_MovingCircle.Radius;
        localPosition += localVelocity * Time.deltaTime;
    
        // Gestion des collisions avec l'exterieur de la zone de simulation
        var size = m_GameState.GameSize;
        if (localPosition.x - localRaduis < -size.x)
        {
            localPosition = new Vector2(-size.x + localRaduis, localPosition.y);
            localVelocity *= new Vector2(-1, 1);
        }
        else if (localPosition.x + localRaduis > size.x)
        {
            localPosition = new Vector2(size.x - localRaduis, localPosition.y);
            localVelocity *= new Vector2(-1, 1);
        }

        if (localPosition.y + localRaduis > size.y)
        {
            localPosition = new Vector2(localPosition.x, size.y - localRaduis);
            localVelocity *= new Vector2(1, -1);
        }
        else if (localPosition.y - localRaduis < -size.y)
        {
            localPosition = new Vector2(localPosition.x, -size.y + localRaduis);
            localVelocity *= new Vector2(1, -1);
        }

    }

}
