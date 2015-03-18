using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class MessageBoardScript : MonoBehaviour
{
	class BoardMessage {
		public BoardMessage(GameObject g) {
			this.gameObject = g;

			//this.width = maxX - minX;
			//this.width = g.GetComponent<RectTransform>().rect.width;
		}
		public float maxX {
			get{
				Vector3[] corners = new Vector3[4]; 
				gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
				float maxX = corners[0].x;
				foreach (Vector3 v in corners) {
					maxX = Math.Max(maxX, v.x);
				}
				return maxX;
			}
		}

		public float minX {
			get {
				Vector3[] corners = new Vector3[4]; 
				gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
				float minX = corners[0].x;
				foreach (Vector3 v in corners) {
					minX = Math.Min(minX, v.x);
				}
				return minX;
			}
		}
		public GameObject gameObject;
		public float width;
	}
    public float fontPercent = 0.8f, scrollSpeed = 0.2f, destroyTime = 60f;
	public float spacing = 0;
    public GameObject scrollingMessageType;

	private Queue<BoardMessage> messages = new Queue<BoardMessage>();

    private Rect rect;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>().rect;
    }
	
	// Update is called once per frame
	void Update ()
	{
		while (messages.Count > 0 && messages.Peek().maxX  < rect.min.x) {
			BoardMessage m = messages.Dequeue();
			Destroy(m.gameObject, 0F);
		}

		BoardMessage obj;
		if (messages.Count > 0) {
			obj = messages.Peek();
			obj.gameObject.transform.position = new Vector2(obj.gameObject.transform.position.x - scrollSpeed, obj.gameObject.transform.position.y);
		}
		int i = 1;
		while (messages.Count > i && messages.ElementAt(i).minX - messages.ElementAt(i-1).maxX > spacing) {
			obj = messages.ElementAt(i);
			obj.gameObject.transform.position = new Vector2(obj.gameObject.transform.position.x - scrollSpeed, obj.gameObject.transform.position.y);
			i = i + 1;
		}

	}

    public void DisplayMessage(string Msg)
    {
		float xPos = this.gameObject.transform.position.x + spacing;
		var instance = Instantiate (scrollingMessageType, new Vector2 (xPos, this.transform.position.y), Quaternion.identity) as GameObject;
		instance.GetComponent<RectTransform>().SetParent(this.transform);
		Text text = instance.GetComponent<Text> ();
		text.text =  Msg + "\t";
		text.fontSize = (int)(this.rect.height*fontPercent);
		//text.cachedTextGenerator.Populate (Msg, text.GetGenerationSettings(Vector2.zero));
		LayoutRebuilder.MarkLayoutForRebuild (instance.transform as RectTransform);
        messages.Enqueue(new BoardMessage(instance));
    }
}
