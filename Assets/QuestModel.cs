using System;
using System.Collections;
using System.Linq;
using System.Text;

class QuestModel
{
    ArrayList questNodes;
    QuestNode root;

    public QuestModel(QuestNode rootNode)
    {
        questNodes = new ArrayList();
        questNodes.Add(rootNode);
        root = rootNode;
    }

    public void addNode(QuestNode newNode)
    {
        questNodes.Add(newNode);
    }

    public ArrayList getNodes()
    {
        return questNodes;
    }

    public void removeNodes(QuestNode toRemove)
    {
        questNodes.Remove(toRemove);
    }

    public QuestNode getRootNode()
    {
        return root;
    }

}
