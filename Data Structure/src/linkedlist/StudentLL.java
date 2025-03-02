package linkedlist;

import entities.Node;

public class StudentLL {
    public Node head;
    public Node tail;

    public StudentLL() {
        head = null;
    }

    public void add(Node newNode) {
        if (head == null) {
            head = newNode;
            tail = newNode;
        }
        else 
        {
            tail.next = newNode;
            tail=tail.next;
        }
    }

    public void showStuData() {
        // System.out.println(head.data);

        Node cn = head;
        while (cn != null) {
            System.out.println(cn.data);
            cn = cn.next;
        }
    }
}
