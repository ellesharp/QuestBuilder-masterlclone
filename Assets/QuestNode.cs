using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Text;

public class QuestNode
{
    ArrayList outcomeNodes;
    MonoBehaviour questItem;
    String questID;
	
	// top left corner (x,y)
	int xpos, ypos;

    public QuestNode(String questIdentifier, MonoBehaviour item, int x, int y)
    {
        questID = questIdentifier;
        this.questItem = item;
        outcomeNodes = new ArrayList();
		xpos = x;
		ypos = y;
    }

    public void addOutcomeNode(QuestNode node)
    {
        outcomeNodes.Add(node);
    }

    public void removeOutcomeNode(QuestNode node)
    {
        outcomeNodes.Remove(node);
    }

    public String getQuestID()
    {
        return questID;
    }

    public void setQuestID(String newID)
    {
        questID = newID;
    }

    public ArrayList getOutcomeNodes()
    {
        return outcomeNodes;
    }
	
	public void setX(int newX)
	{
		xpos = newX;
	}
	
	public int getX()
	{
		return xpos;	
	}
	
	public void setY(int newY)
	{
		ypos = newY;
	}
	
	public int getY()
	{
		return ypos;	
	}
}
