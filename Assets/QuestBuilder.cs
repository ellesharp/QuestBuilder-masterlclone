using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class QuestBuilder : EditorWindow {
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
	private QLocation[] qLocs;
	private QNpc[] qNpcs;
    private static int max = 0;
	private QuestNode first;
	private ComboBox selNode;

    private bool isPressed = false;
    private Vector2 mouseInit;

    // Change offsets for handling dragging the UI.
    private int offsetX = 0, offsetY = 0, oXInit = 0, oYInit = 0;

    private Dictionary<QuestNode, ComboBox> typeBoxes, npcBoxes, locationBoxes;

    private QuestModel currentQuest;
    
    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/Quest Builder")]
    static void Init () {
        // Get existing open window or if none, make a new one:
        QuestBuilder window = (QuestBuilder)EditorWindow.GetWindow(typeof(QuestBuilder));
		window.PopulateObjects();
    }
	
	void PopulateObjects(){
		qLocs = GameObject.FindObjectsOfType(typeof(QLocation)) as QLocation[];
		qNpcs = GameObject.FindObjectsOfType(typeof(QNpc)) as QNpc[];
        typeBoxes = new Dictionary<QuestNode, ComboBox>();
        npcBoxes = new Dictionary<QuestNode, ComboBox>();
        locationBoxes = new Dictionary<QuestNode, ComboBox>();
		selNode = new ComboBox();
        setUpComboBox();
        first = new QuestNode("FirstNode", qNpcs[0] as MonoBehaviour, 0,0);
        currentQuest = new QuestModel(first);
        typeBoxes.Add(first,new ComboBox());
        npcBoxes.Add(first, new ComboBox());
        locationBoxes.Add(first, new ComboBox());
	}
	
    void OnGUI () {
        float height = this.position.height;
        float width = this.position.width;
        float drawY = 30 + offsetY;
		
		
		
//        GUILayout.Label("Quest Setting", EditorStyles.boldLabel);
        GUI.Label(new Rect(10+offsetX, drawY + offsetY, 75, 20), "Quest Name");
        myString = GUI.TextField(new Rect(100+offsetX, drawY+offsetY, width - 200, 20), myString);

        if (GUI.Button(new Rect(width - 90 + offsetX, drawY+offsetY, 80, 20), "Save"))
        {
            saveToXML();
        }

        drawY += 35;



        //myString = EditorGUILayout.TextField ("Quest Name", myString);
        
        //groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled); // Begin group toggle
        //	myBool = EditorGUILayout.Toggle ("Toggle", myBool);
        //	myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup ();

        
		
		// Object List
//		GUILayout.Label("Quest NPCs in Scene", EditorStyles.boldLabel);
//		if(qNpcs != null && qNpcs.Length > 0) {
//			Debug.Log("SIZE>0");
//			GUILayoutOption[] options = new GUILayoutOption[0];
//			foreach(QNpc o in qNpcs){
//				string label = o.name + " | Tag="+o.tag +":"; //"// Tag: ";
//				if(o.tag != "Untagged"){
//					//EditorGUILayout.ObjectField(label,o,typeof(Object),true,new GUILayoutOption[0]);
//				}
//				else {
//					EditorGUILayout.LabelField(label);
//				}
//			}
//		}

        float nodeAreaTop = drawY;

        yCol = new int[8];
        for(int i = 0; i < yCol.Length; i++){
            yCol[i] = (int)drawY;
        }

        //foreach (QuestNode node in currentQuest.getNodes())
        //{
        //    drawNode(node, 0, nodeWidth);
        //}

        if (currentQuest.getRootNode() != null)
        {
            drawNode(currentQuest.getRootNode(), 0, nodeWidth);
        }
		
		// Mouse input handled here

        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.MouseDown || isPressed)
        {
            if (isPressed)
            {
                Vector2 mousePos_2D = new Vector2(currentEvent.mousePosition.x, (Screen.height - currentEvent.mousePosition.y));
                Vector2 diff = mousePos_2D - mouseInit;
                offsetX = oXInit + (int)(diff.x + 0.5);
                offsetY = oYInit - (int)(diff.y + 0.5);
                if (offsetX > 0)
                    offsetX = 0;
                if (offsetY > 0)
                    offsetY = 0;
                Repaint();
            }
            else
            {
                mouseInit = new Vector2(currentEvent.mousePosition.x, (Screen.height - currentEvent.mousePosition.y));
                oXInit = offsetX;
                oYInit = offsetY;
                isPressed = true;
            }
        }
        if (currentEvent.type == EventType.MouseUp)
        {
            isPressed = false;
        }
    }

    private GUIStyle listStyle = new GUIStyle();
    GUIContent[] typeComboBoxList, npcComboBoxList, locationComboBoxList;
    private int nodeWidth = 200;
    private int[] yCol;

    public void setUpComboBox()
    {
        typeComboBoxList = new GUIContent[3];

        typeComboBoxList[0] = new GUIContent("NPC");
        typeComboBoxList[1] = new GUIContent("Object");
        typeComboBoxList[2] = new GUIContent("Location");

        npcComboBoxList = new GUIContent[qNpcs.Length];
        for (int i = 0; i < qNpcs.Length; i++)
        {
            npcComboBoxList[i] = new GUIContent(qNpcs[i].name);
        }

        locationComboBoxList = new GUIContent[qLocs.Length];
        for (int i = 0; i < qLocs.Length; i++)
        {
            locationComboBoxList[i] = new GUIContent(qLocs[i].name);
        }

        listStyle.normal.textColor = Color.black; 
        listStyle.onHover.background =
        listStyle.hover.background = new Texture2D(2, 2);
        listStyle.padding.left =
        listStyle.padding.right =
        listStyle.padding.top =
        listStyle.padding.bottom = 4;
    }
	
	
    GUIContent[] questComboBoxList;
	
	public void setupQuestCombos(Dictionary<string, QuestNode> dict)
	{
		questComboBoxList = new GUIContent[dict.Count];
		
		int i = 0;
		
		foreach (string s in dict.Keys)
		{
			questComboBoxList[i] = new GUIContent(s);
			i++;
		}
	}

    public Rect drawNode(QuestNode node, int r, int w)
    {
        int cHeight = 150; 
        int x = (int)(r * nodeWidth * 1.3) + offsetX;
		yCol[r] += 25;
        int yInit = yCol[r] + offsetY;
		
		// Creates main node box with name at top
        GUI.Box(new Rect(x, yCol[r] + offsetY, nodeWidth, cHeight), node.getQuestID());
        yCol[r] += 25;
		
		// Creates the quest rename field below
        node.setQuestID(GUI.TextField(new Rect(x + 5, yCol[r] + offsetY, nodeWidth - 10, 20), node.getQuestID()));
        yCol[r] += 25;

        int selectedItemIndex = typeBoxes[node].GetSelectedItemIndex();
        int topBoxY = yCol[r] + offsetY;
        yCol[r] += 25;

        //Draw outcome nodes
        foreach (QuestNode n in node.getOutcomeNodes())
        {
            // draw child node -- maybe should just draw line to children (requires x,y info in node)?
            Rect childBox = drawNode(n, r + 1, nodeWidth);
			
			// draws line to child node
            GUIHelper.DrawLine(new Vector2(x + nodeWidth, (yCol[r]) + offsetY), new Vector2(childBox.x, childBox.y + childBox.height / 2), Color.black);
            
			// draws and activates the 'x' button for deletion
			if (GUI.Button(new Rect(childBox.x, childBox.y, 20, 20), "X"))
            {
                deleteNode(n);
                node.removeOutcomeNode(n);
            }
        }       
        
        yCol[r] += 25;
		
		// outcome label
		GUI.Label(new Rect(x + 60, yCol[r] + offsetY + 5, 92, 20), "- Outcomes -");
		
		yCol[r] += 25;
		
		
        //Draw add node button
        if (GUI.Button(new Rect(x + 5, yCol[r] + offsetY, 92, 20), "New Child..."))
        {
            QuestNode newNode = new QuestNode("New Node " + max++, qNpcs[0], 0, 0);
            node.addOutcomeNode(newNode);
            currentQuest.addNode(newNode);
            

            typeBoxes.Add(newNode, new ComboBox());
            npcBoxes.Add(newNode, new ComboBox());
            locationBoxes.Add(newNode, new ComboBox());
        }
		
		// Draw 'Select Node' button
		if (GUI.Button(new Rect(x + 102, yCol[r] + offsetY, 92, 20), "Select Node..."))
        {
			Dictionary<string, QuestNode> qInventory = new Dictionary<string, QuestNode>();
			GetNodeList(first, qInventory);
			
			setupQuestCombos(qInventory);
			
			int selectedNode = selNode.List(
            new Rect(x + 102, yCol[r] +offsetY, 92, 20), "vvvvv", questComboBoxList, listStyle);
			Debug.Log("YOU REACHED ME!!!!	"+ selectedNode.ToString());
			
		}

        // NPC selected
        if (selectedItemIndex == 0)
        {
            int selectedNPC = npcBoxes[node].GetSelectedItemIndex();
            selectedNPC = npcBoxes[node].List(
            new Rect(x + 5, topBoxY + 25, nodeWidth - 10, 20), npcComboBoxList[selectedNPC].text, npcComboBoxList, listStyle);
			Debug.Log(selectedNPC);
        }
        // Object Selected
        else if (selectedItemIndex == 1)
        {

        }
        // Location Selected
        else
        {
            int selectedLoc = locationBoxes[node].GetSelectedItemIndex();
            selectedLoc = locationBoxes[node].List(
            new Rect(x + 5, topBoxY + 25, nodeWidth - 10, 20), locationComboBoxList[selectedLoc].text, locationComboBoxList, listStyle);
        }

        selectedItemIndex = typeBoxes[node].List(
            new Rect(x + 5, topBoxY, nodeWidth - 10, 20), typeComboBoxList[selectedItemIndex].text, typeComboBoxList, listStyle);

        yCol[r] += 30;
        return new Rect(x, yInit, nodeWidth, cHeight);
    }

    public void deleteNode(QuestNode node){
            for(int i = 0; i < node.getOutcomeNodes().Count; i++)
            {
                QuestNode n = node.getOutcomeNodes()[i] as QuestNode;
                if (n.getOutcomeNodes().Count == 0)
                {
                    node.removeOutcomeNode(n);

                    typeBoxes.Remove(node);
                    npcBoxes.Remove(node);
                    locationBoxes.Remove(node);
                    i--;
                }
                else
                {
                    deleteNode(n);
                    node.removeOutcomeNode(n);
                    i--;
                }
            }
    }

    public void saveToXML()
    {

    }

    public static object SelectList(ICollection list, object selected, GUIStyle defaultStyle, GUIStyle selectedStyle)
    {
        foreach (object item in list)
        {
            if (GUILayout.Button(item.ToString(), (selected == item) ? selectedStyle : defaultStyle))
            {
                if (selected == item)
                // Clicked an already selected item. Deselect.
                {
                    selected = null;
                }
                else
                {
                    selected = item;
                }
            }
        }

        return selected;
    }

    public delegate bool OnListItemGUI(object item, bool selected, ICollection list);

    public static object SelectList(ICollection list, object selected, OnListItemGUI itemHandler)
    {
        ArrayList itemList;

        itemList = new ArrayList(list);

        foreach (object item in itemList)
        {
            if (itemHandler(item, item == selected, list))
            {
                selected = item;
            }
            else if (selected == item)
            // If we *were* selected, but aren't any more then deselect
            {
                selected = null;
            }
        }

        return selected;
    }
	
	public void GetNodeList(QuestNode node, Dictionary<string, QuestNode> oList)
	{
		if (node.getOutcomeNodes().Count > 0)
		{
			foreach(QuestNode n in node.getOutcomeNodes())
			{
				GetNodeList(n, oList);
			}
		}
		
		if (!oList.ContainsKey(node.getQuestID()))
		{
			oList.Add(node.getQuestID(), node);
		}
	}
}