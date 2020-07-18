using UnityEngine;
using UniversoAumentado.ARCraft.Network;

public class NetworkObject : MonoBehaviour {

    public enum Mode {
        LISTEN, BROADCAST
    }

    public NetworkObjectManager networkObjectManager;

    public string id;
    public int ownerID;
    public Mode mode;
    public string type;
    private float sendRate = 10f;

    private float lastUpdate = 0;

    void Start() {
        if (!networkObjectManager) {
            Debug.LogError("NetworkObject could not find a NetworkObjectManager in parent. Will not be able to sync.");
            enabled = false;
            return;
        }
        UpdateName();
    }

    void Update() {
        // Throttle network updates
        if (Time.time < lastUpdate + 1 / sendRate) return;

        lastUpdate = Time.time;

        if(mode == Mode.BROADCAST) {
            networkObjectManager.UpdateObject(this);
        }
    }

    void OnDestroy() {
        Debug.Log($"NetworkObject '{id}' destroyed.");
        networkObjectManager.RemoveObject(this);
    }

    public void SetID(string id) {
        this.id = id;
        UpdateName();
    }

    public void SetOwnerID(int id) {
        ownerID = id;
        UpdateName();
    }

    void UpdateName() {
        if(mode == Mode.LISTEN) {
            transform.name = $"NetworkObject-{id}";
        }
    }

    public ARPlaneServer.Classes.GameObject GetNetworkObject() {
        return new ARPlaneServer.Classes.GameObject() {
            id = id,
            ownerID = (ushort)ownerID,
            position = new ARPlaneServer.Classes.Vector3(transform.position.x, transform.position.y, transform.position.z),
            rotation = new ARPlaneServer.Classes.Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z),
            type = type
        };
    }

    public void SetObjectState(ARPlaneServer.Classes.GameObject state) {
        if (mode != Mode.LISTEN) return;

        transform.position = new UnityEngine.Vector3(state.position.x, state.position.y, state.position.z);
        transform.rotation = Quaternion.Euler(state.rotation.x, state.rotation.y, state.rotation.z);
    }

    public void SetObjectInfo(ARPlaneServer.Classes.GameObject info) {
        ownerID = info.ownerID;
        id = info.id;
        type = info.type;
        UpdateName();
    }
}
