using UnityEngine;
using UniversoAumentado.ARCraft.Network;

public class NetworkObject : MonoBehaviour {

    public enum State {
        LISTEN, BROADCAST
    }

    public NetworkObjectManager networkObjectManager;

    public string id;
    public int ownerID;
    public State state;
    public string type;

    void Start() {
        if (!networkObjectManager) {
            Debug.LogError("NetworkObject could not find a NetworkObjectManager in parent. Will not be able to sync.");
            enabled = false;
            return;
        }
        UpdateName();
    }

    void Update() {
        if(state == State.BROADCAST) {
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

    void UpdateName() {
        if(state == State.LISTEN) {
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
        if (this.state != State.LISTEN) return;

        transform.position = new UnityEngine.Vector3(state.position.x, state.position.y, state.position.z);
        transform.rotation = Quaternion.Euler(state.position.x, state.position.y, state.position.z);
    }

    public void SetObjectInfo(ARPlaneServer.Classes.GameObject info) {
        ownerID = info.ownerID;
        id = info.id;
        type = info.type;
        UpdateName();
    }
}
