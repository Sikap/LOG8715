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

    public struct Snapshot: INetworkSerializable
    { 
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter{}    
        public Vector2 position;
        public int tick;
    }
    private NetworkVariable<Snapshot> m_Position = new NetworkVariable<Snapshot>();

    public Snapshot Position => m_Position.Value;

    private Queue<Snapshot> m_InputQueue = new Queue<Snapshot>();
    public Queue<Vector2> localInputQueue = new Queue<Vector2>();
    public Vector2 localPosition;
    public  SnapshotInputArray inputSnapshot;
    public  SnapshotArray locationSnapshot;
    private void Awake()
    {
        m_GameState = FindObjectOfType<GameState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            localPosition = Position.position;
            inputSnapshot = new SnapshotInputArray(100);
            locationSnapshot = new SnapshotArray(100);
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
            UpdateInputClient();
            Debug.Log(
                "UpdateInputClient: "
                + "NetworkObjectId: " + NetworkObjectId
                + "LocalTick: " + NetworkUtility.GetLocalTick()
                + "LocalInput: " + localInputQueue.Peek().ToString()
            );
            inputSnapshot.AddSnapshot(NetworkObjectId, NetworkUtility.GetLocalTick(), localInputQueue);
            

            localPrediction();
            Debug.Log(
                "LocalPrediction: "
                + "NetworkObjectId: " + NetworkObjectId
                + "LocalTick: " + NetworkUtility.GetLocalTick()
                + "LocalPosition: " + localPosition
            );
            locationSnapshot.AddSnapshot(NetworkObjectId, NetworkUtility.GetLocalTick(), localPosition);
            
            //Réconciliation avec serveur si ancienne prediction est fausse
            Snapshot serverSnashot = Position;        
            Vector2 correspondingTickLocalPosition = locationSnapshot.GetSnapshotValue(NetworkObjectId, serverSnashot.tick);
            Debug.Log("correspondingTickLocalPosition : " + correspondingTickLocalPosition.ToString());
            Debug.Log("serverSnashot : " + serverSnashot.position.ToString());

            float distance = Vector2.Distance(serverSnashot.position, correspondingTickLocalPosition);  
           
            //Debug.Log("serverSnashot : " + serverSnashot.position.ToString());
            //Debug.Log("Distance between serverPosition and correspondingTickLocalPosition : " + distance);
            if(distance > 0.1)
            {
                localPosition = serverSnashot.position;
                locationSnapshot.AddSnapshot(NetworkObjectId, serverSnashot.tick, localPosition);

                int localTick =  NetworkUtility.GetLocalTick();
                int tick = serverSnashot.tick;
                //Debug.Log("LocalTick: " + localTick);
                //Debug.Log("serverSnashotTick: " + tick);
                /*  
                while(tick < localTick);
                {
                    Debug.Log("ServerSnashotTick : " +  serverSnashot.tick);
                    int i = 0;
                    //localInputQueue = inputSnapshot.GetSnapshotValue(NetworkObjectId, serverSnashot.tick + 1 + i);
                    //localPrediction();
                    //locationSnapshot.AddSnapshot(NetworkObjectId, serverSnashot.tick + 1 + i, localPosition);
                    tick++;
                    i++;
                }
                */
            }
        }

    }

    private void UpdatePositionServer()
    {

        // Mise a jour de la position selon dernier input reçu, puis consommation de l'input
        if (m_InputQueue.Count > 0)
        {
            Snapshot input = m_InputQueue.Dequeue();                
            Snapshot currentSnapshot = m_Position.Value; 


            if(currentSnapshot.tick < input.tick){
                        
                currentSnapshot.position += input.position * m_Velocity * Time.deltaTime; 
                
                // Gestion des collisions avec l'exterieur de la zone de simulation
                var size = GameState.GameSize;
                if (currentSnapshot.position.x - m_Size < -size.x)
                {
                    currentSnapshot.position =  new Vector2(-size.x + m_Size, currentSnapshot.position.y);
                }
                else if (currentSnapshot.position.x + m_Size > size.x)
                {
                    currentSnapshot.position = new Vector2(size.x - m_Size, currentSnapshot.position.y);
                }

                if (currentSnapshot.position.y + m_Size > size.y)
                {
                    currentSnapshot.position = new Vector2(currentSnapshot.position.x, size.y - m_Size);
                }
                else if (currentSnapshot.position.y - m_Size < -size.y)
                {
                    currentSnapshot.position = new Vector2(currentSnapshot.position.x, -size.y + m_Size);
                }            
                currentSnapshot.tick =  input.tick;
                
                Debug.Log("currentSnapshot position: " + currentSnapshot.position);
                Debug.Log("currentSnapshot tick: " + currentSnapshot.tick);
                
                Debug.Log("input position: " + input.position);
                Debug.Log("input tick: " + input.tick);

                m_Position.Value = currentSnapshot; 
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
        //Debug.Log("input: " + inputDirection.normalized);
        //Debug.Log("tick: " + NetworkUtility.GetLocalTick());
        localInputQueue.Enqueue(inputDirection.normalized);
        SendInputServerRpc(NetworkObjectId, inputDirection.normalized, NetworkUtility.GetLocalTick());
    }

    [ServerRpc]
    private void SendInputServerRpc(ulong senderId, Vector2 inputDirection, int tick)
    {
        // On utilise une file pour les inputs pour les cas ou on en recoit plusieurs en meme temps.
        //Debug.Log("senderId: " + senderId);
        //Debug.Log("input: " + inputDirection.ToString());
        //Debug.Log("tick: " + tick);
        Snapshot input = new Snapshot { position = inputDirection, tick =tick };
        m_InputQueue.Enqueue(input);
    }
}
