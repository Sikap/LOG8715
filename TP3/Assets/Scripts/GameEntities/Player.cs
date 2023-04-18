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

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            local_Position = Position.vector;
            local_SnapshotInputHistory = new SnapshotInput(100);
            local_SnapshotPosition = new SnapshotLocation(100);
        }
    }

    public struct Snapshot: INetworkSerializable
    { 
        public Vector2 vector;
        public int tick;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter{
            serializer.SerializeValue(ref vector);
            serializer.SerializeValue(ref tick);
        }    
    }

    private NetworkVariable<Snapshot> m_Position = new NetworkVariable<Snapshot>();

    public Snapshot Position => m_Position.Value;

    private Queue<Vector2> m_InputQueue = new Queue<Vector2>();
    private Queue<int> m_TickQueue = new Queue<int>();


    // LOCATION HISTORY AND CURRENT LOCATION
    public Vector2 local_Position = new Vector2();
    private SnapshotLocation local_SnapshotPosition;

    // INPUT HISTORY AND CURRENT INPUT
    private Queue<Vector2> local_InputHistory = new Queue<Vector2>();
    private SnapshotInput local_SnapshotInputHistory;
    private Vector2 mostRecentInputPlayer = new Vector2();

    private void Awake()
    {
        m_GameState = FindObjectOfType<GameState>();
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
            

            localPrediction(mostRecentInputPlayer);
            local_SnapshotPosition.Add(local_Position, NetworkUtility.GetLocalTick());
            mostRecentInputPlayer = Vector2.zero;
        }
    }

    private void localPrediction(Vector2 input)
    {
        // Mise a jour de la position selon dernier input reçu, puis consommation de l'input
        local_Position += input * m_Velocity * Time.deltaTime;

        // Gestion des collisions avec l'exterieur de la zone de simulation
        var size = GameState.GameSize;
        if (local_Position.x - m_Size < -size.x)
        {
            local_Position = new Vector2(-size.x + m_Size, local_Position.y);
        }
        else if (local_Position.x + m_Size > size.x)
        {
            local_Position = new Vector2(size.x - m_Size, local_Position.y);
        }

        if (local_Position.y + m_Size > size.y)
        {
            local_Position = new Vector2(local_Position.x, size.y - m_Size);
        }
        else if (local_Position.y - m_Size < -size.y)
        {
            local_Position = new Vector2(local_Position.x, -size.y + m_Size);
        }        
    }

    private void UpdatePositionServer()
    {
        // Mise a jour de la position selon dernier input reçu, puis consommation de l'input
        if (m_InputQueue.Count > 0 && m_TickQueue.Count > 0)
        {
            var input = m_InputQueue.Dequeue();
            var tick = m_TickQueue.Dequeue();
            Snapshot currentSnapshot = m_Position.Value;
            currentSnapshot.vector += input * m_Velocity * Time.deltaTime;

            // Gestion des collisions avec l'exterieur de la zone de simulation
            var size = GameState.GameSize;
            if (currentSnapshot.vector.x - m_Size < -size.x)
            {
                currentSnapshot.vector = new Vector2(-size.x + m_Size, currentSnapshot.vector.y);
            }
            else if (currentSnapshot.vector.x + m_Size > size.x)
            {
                currentSnapshot.vector = new Vector2(size.x - m_Size, currentSnapshot.vector.y);
            }

            if (currentSnapshot.vector.y + m_Size > size.y)
            {
                currentSnapshot.vector = new Vector2(currentSnapshot.vector.x, size.y - m_Size);
            }
            else if (currentSnapshot.vector.y - m_Size < -size.y)
            {
                currentSnapshot.vector = new Vector2(currentSnapshot.vector.x, -size.y + m_Size);
            }
            currentSnapshot.tick = tick;
            m_Position.Value = currentSnapshot;
            InputClientProcessedClientRpc(currentSnapshot.vector, tick);
        }
    }

    private void UpdateInputClient()
    {
        Vector2 inputDirection = Vector2.zero;
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
        local_InputHistory.Enqueue(inputDirection);
        local_SnapshotInputHistory.Add(local_InputHistory,NetworkUtility.GetLocalTick());
        mostRecentInputPlayer = inputDirection;
        if (inputDirection != Vector2.zero)
        {    
            SendInputServerRpc(inputDirection,NetworkUtility.GetLocalTick());
        }
    }


    [ServerRpc]
    private void SendInputServerRpc(Vector2 input, int tick)
    {
        // On utilise une file pour les inputs pour les cas ou on en recoit plusieurs en meme temps.
        m_InputQueue.Enqueue(input);
        m_TickQueue.Enqueue(tick);
    }

    [ClientRpc]
    private void InputClientProcessedClientRpc(Vector2 position, int tick) {
        Snapshot serverSnashot = Position;        
        Vector2 correspondingTickLocalPosition = local_SnapshotPosition.Get(serverSnashot.tick);

        float distance = Vector2.Distance(serverSnashot.vector, correspondingTickLocalPosition);  
        
        if(distance > 0.1)
        {
            // local_Position = serverSnashot.vector;
            // local_SnapshotPosition.Add(local_Position, serverSnashot.tick);

            // int localTick =  NetworkUtility.GetLocalTick();
            // int tickServer = serverSnashot.tick;
              
            // int i = 0;
            // while(tickServer < localTick)
            // {
            //     local_InputHistory = local_SnapshotInputHistory.Get(serverSnashot.tick + 1 + i);
            //     localPrediction();
            //     locationSnapshot.AddSnapshot(NetworkObjectId, serverSnashot.tick + 1 + i, localPosition);
            //     tickServer++;
            //     i++;
            // }
            
        }
    } 
}