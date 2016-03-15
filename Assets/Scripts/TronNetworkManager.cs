using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TronNetworkManager : MonoBehaviour {

	public bool isAtStartup = true;

	private Button bothButton;
	private Button clientButton;

	NetworkClient myClient;
	private NetworkManager netManager;

	void Awake() {
		netManager = GetComponent <NetworkManager> ();
	}

	void Start() {

	}

	void Update ()
	{
		
	}


	void OnGUI()
	{
		if (isAtStartup)
		{
			if (GUI.Button (new Rect (400, 100, 500, 100), "Start as Host")) {
				SetupHost ();
			}
			if (GUI.Button (new Rect (400, 200, 500, 100), "Start as Client")) {
				SetupClient ();
			}

		}
	}  

	// Create a server and listen on a port
	public void SetupHost ()
	{
		/*NetworkServer.Listen (4444);
		myClient = new NetworkClient();
		myClient.RegisterHandler(MsgType.Connect, OnConnected);     
		myClient.Connect("127.0.0.1", 4444);
*/

		netManager.StartHost ();

		isAtStartup = false;
		GameObject tempCam = GameObject.FindWithTag ("tmp");
		GameObject.Destroy (tempCam);

	}

	// Create a client and connect to the server port
	public void SetupClient()
	{
		/*	
		myClient = new NetworkClient();
		myClient.RegisterHandler(MsgType.Connect, OnConnected);     
		myClient.Connect("127.0.0.1", 4444);
		*/


		netManager.StartClient ();

		isAtStartup = false;

		GameObject tempCam = GameObject.FindWithTag ("tmp");
		GameObject.Destroy (tempCam);

	}

	// Create a local client and connect to the local server
	public void SetupLocalClient()
	{
		myClient = ClientScene.ConnectLocalServer();
		myClient.RegisterHandler(MsgType.Connect, OnConnected);     
		isAtStartup = false;
		netManager.StartClient ();
		GameObject tempCam = GameObject.FindWithTag ("tmp");
		GameObject.Destroy (tempCam);
	}

	// client function
	public void OnConnected(NetworkMessage netMsg)
	{
		Debug.Log("Connected to server");
	}
}



