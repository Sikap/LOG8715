using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private float m_Velocity;

    [SerializeField]
    private float m_Size = 1;

    private GameState m_GameState;

    // GameState peut etre nul si l'entite joueur est instanciee avant de charger MainScene
    private GameState GameState
    {
        get
        {
            if (m_GameState == null)
            {
                m_GameState = FindObjectOfType<GameState>();
            }
            return m_GameState;
        }
    }

    private NetworkVariable<Vector2> m_Position = new NetworkVariable<Vector2>();

    public Vector2 Position => m_Position.Value;

    private Queue<Vector2> m_InputQueue = new Queue<Vector2>();
    public Queue<Vector2> localInputQueue = new Queue<Vector2>();
    public Vector2 localPosition;
    public  SnapshotInputArray inputSnapshot;
    public  SnapshotArray locationSnapshot;
    public Queue<Vector2>[] lastKnownInputOfOtherPlayers;
    private void Awake()
    {
        m_GameState = FindObjectOfType<GameState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            localPosition = Position;
            inputSnapshot = new SnapshotInputArray(100);
            locationSnapshot = new SnapshotArray(100);
            lastKnownInputOfOtherPlayers = new Queue<Vector2>[100];
        }
    }

    private void FixedUpdate()
    {
        // Si le stun est active, rien n'est mis a jour.
        if (GameState == null || GameState.IsStunned)
        {
            return;
        }

        // Seul le serveur met à jour la position de l'entite.
        if (IsServer)
        {
            UpdatePositionServer();
        }

        // Seul le client qui possede cette entite peut envoyer ses inputs. 
        if (IsClient && IsOwner)
        {           
            Debug.Log("OWNER PREDICTION of NetworkObject " + NetworkObjectId + " for tick " +  NetworkUtility.GetLocalTick());
            UpdateInputClient();
            inputSnapshot.AddSnapshot(NetworkObjectId, NetworkUtility.GetLocalTick(), localInputQueue);
            
            localPrediction();
            locationSnapshot.AddSnapshot(NetworkObjectId, NetworkUtility.GetLocalTick(), localPosition);


            //Réconciliation avec serveur si ancienne prediction est fausse
            Vector2 serverPosition = Position;
            
            int delay =  Mathf.RoundToInt( GameState.CurrentRTT * NetworkUtility.GetLocalTickRate());
            int serverTick = NetworkUtility.GetLocalTick() - delay;            
            Debug.Log("Current tick " + NetworkUtility.GetLocalTick() );  
            Debug.Log("delay " + delay );  
            Debug.Log("serverTick " + serverTick ); 
            Vector2 correspondingTickLocalPosition = locationSnapshot.GetSnapshotValue(NetworkObjectId, serverTick);

            if(serverPosition != correspondingTickLocalPosition)
            {
                localPosition = serverPosition;
                Debug.Log("NEED CORRECTION for NetworkObject " + NetworkObjectId + " for tick " + serverTick);    
                locationSnapshot.AddSnapshot(NetworkObjectId, serverTick, localPosition);
                for(int i = 0; i < delay; i++)
                {
                    Debug.Log("CORRECTION of NetworkObject " + NetworkObjectId + " for tick " +  (serverTick + 1 + i));
                    localInputQueue = inputSnapshot.GetSnapshotValue(NetworkObjectId, (serverTick + 1 + i));
                    localPrediction();
                    locationSnapshot.AddSnapshot(NetworkObjectId, serverTick + 1 + i, localPosition);
                }
            }

        }

        if (IsClient && !IsOwner)
        {
            if(lastKnownInputOfOtherPlayers[NetworkObjectId % 100] == null)
            {
                localInputQueue.Enqueue(Vector2.zero);
            }else if (lastKnownInputOfOtherPlayers[NetworkObjectId % 100].Count > 0)
            {
               
                //Debug.Log("PALYER GHOST PREDICTION of NetworkObject " + NetworkObjectId + " for tick " +  NetworkUtility.GetLocalTick() + " with last known input " + lastKnownInputOfOtherPlayers[NetworkObjectId % 100].Peek());
                localInputQueue = lastKnownInputOfOtherPlayers[NetworkObjectId % 100];
            }else
            {
                localInputQueue.Enqueue(Vector2.zero);
            }
            localPrediction();
            locationSnapshot.AddSnapshot(NetworkObjectId, NetworkUtility.GetLocalTick(), localPosition);
            
            Vector2 serverPosition = Position;
            int delay =  Mathf.RoundToInt( (GameState.CurrentRTT) * NetworkUtility.GetLocalTickRate());
            int serverTick = NetworkUtility.GetLocalTick() - delay;            
            //Debug.Log("Current tick " + NetworkUtility.GetLocalTick() );  
            //Debug.Log("delay " + delay );  
            //Debug.Log("serverTick " + serverTick );  
            Vector2 correspondingTickLocalPosition = locationSnapshot.GetSnapshotValue(NetworkObjectId, serverTick);

            if(serverPosition != correspondingTickLocalPosition)
            {
                localPosition = serverPosition;
                //Debug.Log("NEED CORRECTION for NetworkObject " + NetworkObjectId + " for tick " + serverTick);    
                locationSnapshot.AddSnapshot(NetworkObjectId, serverTick, localPosition);
                for(int i = 0; i < delay; i++)
                {
                    //Debug.Log("CORRECTION of NetworkObject " + NetworkObjectId + " for tick " +  (serverTick + 1 + i));
                    localInputQueue = inputSnapshot.GetSnapshotValue(NetworkObjectId, (serverTick + 1 + i));
                    localPrediction();
                    locationSnapshot.AddSnapshot(NetworkObjectId, serverTick + 1 + i, localPosition);
                }
            }


        }
    }

    private void UpdatePositionServer()
    {
        // Mise a jour de la position selon dernier input reçu, puis consommation de l'input
        if (m_InputQueue.Count > 0)
        {
            var input = m_InputQueue.Dequeue();
            m_Position.Value += input * m_Velocity * Time.deltaTime;

            // Gestion des collisions avec l'exterieur de la zone de simulation
            var size = GameState.GameSize;
            if (m_Position.Value.x - m_Size < -size.x)
            {
                m_Position.Value = new Vector2(-size.x + m_Size, m_Position.Value.y);
            }
            else if (m_Position.Value.x + m_Size > size.x)
            {
                m_Position.Value = new Vector2(size.x - m_Size, m_Position.Value.y);
            }

            if (m_Position.Value.y + m_Size > size.y)
            {
                m_Position.Value = new Vector2(m_Position.Value.x, size.y - m_Size);
            }
            else if (m_Position.Value.y - m_Size < -size.y)
            {
                m_Position.Value = new Vector2(m_Position.Value.x, -size.y + m_Size);
            }
        }
    }

    private void localPrediction()
    {
        // Mise a jour de la position selon dernier input reçu, puis consommation de l'input
        if (localInputQueue.Count > 0)
        {
            var input = localInputQueue.Dequeue();
            localPosition += input * m_Velocity * Time.deltaTime;

            // Gestion des collisions avec l'exterieur de la zone de simulation
            var size = GameState.GameSize;
            if (localPosition.x - m_Size < -size.x)
            {
                localPosition = new Vector2(-size.x + m_Size, localPosition.y);
            }
            else if (localPosition.x + m_Size > size.x)
            {
                localPosition = new Vector2(size.x - m_Size, localPosition.y);
            }

            if (localPosition.y + m_Size > size.y)
            {
                localPosition = new Vector2(localPosition.x, size.y - m_Size);
            }
            else if (localPosition.y - m_Size < -size.y)
            {
                localPosition = new Vector2(localPosition.x, -size.y + m_Size);
            }
        }
    }

    private void UpdateInputClient()
    {
        Vector2 inputDirection = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputDirection += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDirection += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDirection += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDirection += Vector2.right;
        }
        localInputQueue.Enqueue(inputDirection.normalized);
        
        //Debug.Log("SendInputServerRpc with senderId " + NetworkObjectId + " with tick " + NetworkUtility.GetLocalTick() +" with  input " + inputDirection.normalized);

        SendInputServerRpc(NetworkObjectId, NetworkUtility.GetLocalTick(), inputDirection.normalized);
    }

    [ServerRpc]
    private void SendInputServerRpc(ulong senderId, int localTick, Vector2 input)
    {
        // On utilise une file pour les inputs pour les cas ou on en recoit plusieurs en meme temps.
        m_InputQueue.Enqueue(input);
        
        // Broadcast the input to all clients.
        ReceiveInputClientRpc(senderId, localTick, input);
    }

    [ClientRpc]
    private void ReceiveInputClientRpc(ulong senderId, int localTick, Vector2 input)
    {
        if (IsOwner) return;
        // This method is called on all clients when the server broadcasts an input from a player.
        // Store the input locally on the player object.
        Debug.Log("ReceiveInputClientRpc with senderId " + senderId + " with tick " + localTick +" with  input " + input);
        Debug.Log("ReceiveInputClientRpc has NetworkObjectId " + NetworkObjectId + " with local tick " + NetworkUtility.GetLocalTick());

    
        var tmpInputQueue = new Queue<Vector2>();
        tmpInputQueue.Enqueue(input);            
        
        lastKnownInputOfOtherPlayers[senderId % 100] = tmpInputQueue;
       
        Debug.Log("LastKnownInputOfOtherPlayers of  senderId" + senderId + " for tick " + localTick +" for input " +  lastKnownInputOfOtherPlayers[senderId % 100].Peek()  + " arrived at tick " + NetworkUtility.GetLocalTick());

        inputSnapshot.AddSnapshot(senderId, localTick, tmpInputQueue);

    }
}
