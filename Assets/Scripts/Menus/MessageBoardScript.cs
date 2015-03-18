using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageBoardScript : MonoBehaviour
{

    public float fontPercent = 0.8f, scrollSpeed = 0.2f, destroyTime = 60f;
    public GameObject scrollingMessageType;

    private Queue<GameObject> messages = new Queue<GameObject>();

    private Rect rect;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>().rect;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    while (messages.Count > 0 && messages.Peek() == null)
	        messages.Dequeue();
	    foreach (var obj in messages)
	    {
	        obj.transform.position = new Vector2(obj.transform.position.x - scrollSpeed, obj.transform.position.y);
	    }
	}

    public void DisplayMessage(string Msg)
    {
        var instance = Instantiate(scrollingMessageType, new Vector2(rect.width, this.transform.position.y), Quaternion.identity) as GameObject;
        instance.GetComponent<RectTransform>().SetParent(this.transform);
        instance.GetComponent<Text>().text = Msg;
        instance.GetComponent<Text>().fontSize = (int)(this.rect.height*fontPercent);
        messages.Enqueue(instance);
        Destroy(instance, destroyTime);
    }
}
