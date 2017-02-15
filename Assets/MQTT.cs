using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;

public class MQTT : MonoBehaviour {
	private const string SPLINE_TOPIC = "color_spline";

	public string host = "192.168.0.6";
	public int port = 1883;

	public string clientName = "UnityColorSpline";

	private ColorSplineObject colorSplineObject;
	private MqttClient4Unity client;

	// Use this for initialization
	void Start () {
		colorSplineObject = GetComponent<ColorSplineObject>();
		colorSplineObject.UpdateSplineEvent += new ColorSplineObject.UpdateSplineEventHandler(OnSplineUpdate);
		client = new MqttClient4Unity(host, port, false, null);
		client.Connect(clientName);
		client.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(OnOtherMessage);
		client.MqttMsgPublished += new MqttClient.MqttMsgPublishedEventHandler(OnMessage);
		client.Subscribe(SPLINE_TOPIC); //TODO This isn't working
	}

	private void OnMessage(object sender, MqttMsgPublishedEventArgs e) 
	{
		Debug.Log("OnMessage " + e.ToString());
		Debug.Log("Message id: " + e.MessageId);
	}

	private void OnOtherMessage(object sender, MqttMsgPublishEventArgs e) 
	{
		Debug.Log("OnOtherMessage " + e.ToString());
	}

	void OnDestroy() {
		client.Disconnect();
	}

	private void OnSplineUpdate(ColorSpline spline) {
		client.Publish(SPLINE_TOPIC, Encoding.ASCII.GetBytes(spline.ToJSON()));
	}

	// Update is called once per frame
	void Update () {
	}
}
