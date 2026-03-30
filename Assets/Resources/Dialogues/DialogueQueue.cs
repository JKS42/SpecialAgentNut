public class DialogueQueue
{
    private DialogueItem[] items;
    private int front;
    private int rear;
    private int count;

    // Constructor: creates a fixed-size circular queue
    public DialogueQueue(int size)
    {
        items = new DialogueItem[size];
        front = 0;
        rear = -1;
        count = 0;
    }

    // Adds a new DialogueItem to the queue
    public void Enqueue(DialogueItem item)
    {
        if (count == items.Length)
        {
            return;
        }

        rear = (rear + 1) % items.Length;
        items[rear] = item;
        count++;
    }

    // Removes and returns the next DialogueItem from the queue
    public DialogueItem Dequeue()
    {
        if (count == 0)
        {
            return null;
        }

        DialogueItem item = items[front];
        front = (front + 1) % items.Length;
        count--;

        return item;
    }

    // Returns true if the queue has no elements
    public bool IsEmpty()
    {
        return count == 0;
    }
}
