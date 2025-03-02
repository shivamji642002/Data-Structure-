// import java.util.Arrays;
// import java.util.List;
// import java.util.stream.Collectors;
// import java.util.stream.IntStream;
// import arrays.crud;
// import sorting.InsertionSort;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import entities.Node;
import linkedlist.StudentLL;
import trees.TreeDemo;
import trees.TreeNode;

public class App {
    public static int[] getRandomArray(int size) {
        int arr[] = new int[size];
        for (int i = 0; i < size; i++) {
            arr[i] = (int) (Math.random() * 10 + 1);
        }
        return arr;
    }

    public static void main(String[] args) throws Exception {
        System.out.println("Hello, World!");
        // System.out.println(Arrays.toString(getRandomArray(10)));
        // int arr[] = getRandomArray(10);
        // int arr[] = new int[] { 70, 22, 30, 10, 20 };
        // BubbleSort.bubbleSort(arr);
        // InsertionSort.insertionSort(arr);
        // System.out.println(Arrays.toString(arr));
        // int key = 6;
        // Linear.search(arr,key);
        // int pos = BinarySearch.binarySearch(arr, key);
        // if (pos != -1) {
        // System.out.println(key + " found at index pos :" + pos);
        // } else {
        // System.out.println("not found in array");
        // }

        // int arr[] = { 10};
        // crud.tryArray(arr);
        // IntStream intStream = Arrays.stream(arr);
        // intStream.forEach(e -> System.out.print(e+" "));

        // List<Integer> data=
        // intStream.boxed().filter(e->e>25).collect(Collectors.toList());
        // System.out.println(data);

        // System.out.println(intStream.boxed().map(Double::new).map(e->e*1.15).collect(Collectors.toList()));

        // JAVA 8 -> Stream API
        // Runnable r1 = new Runnable() { // Anonymous inner class
        // @Override
        // public void run() {
        // System.out.println("running");
        // }
        // };
        // Runnable r2 = ()->{System.out.println("running");};

        // int sp[] = { 7, 1, 5, 3, 6, 4 };

        // int minPrice = Integer.MAX_VALUE;// infinity
        // int maxProfit = 0;

        // for (int i = 0; i < sp.length; i++) {
        // if (sp[i] < minPrice) {
        // minPrice = sp[i]; // Update min price
        // }
        // int profit = sp[i] - minPrice;
        // if (profit > maxProfit) {
        // maxProfit = profit; // Update max profit
        // }
        // }

        // System.out.println("Maximum Profit: " + maxProfit);
        // Tapping rain water problem

        // StudentLL list = new StudentLL();
        // Node n1=new Node(11);
        // Node n2=new Node(12);
        // Node n3=new Node(13);
        // Node n4=new Node(14);

        // list.head=n1;
        // n1.next=n2;
        // n2.next=n3;
        // n3.next=n4;
        // list.add(new Node(10));
        // list.add(new Node(11));
        // list.add(new Node(12));
        // list.add(new Node(13));
        // list.add(new Node(14));
        // list.add(new Node(15));

        // list.showStuData();

        // Reference vs Object
        // i.e Node n1; vs new Node(10);
        // BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
        // int choice = -1;
        // do {
        // System.out.println("\u001b[2J");
        // System.out.print("\u001b[H");
        // System.out.println("Operations on Linked List");
        // System.out.println("""
        // 1.Add Node at last
        // 2.Add Node at beginning
        // 3.Add Node at position
        // 4.Print List
        // 0.exit
        // """);
        // System.out.print("Enter ur choice:");
        // choice = Integer.parseInt(br.readLine());
        // if (choice == 0)
        // break;

        // switch (choice) {
        // case 1:
        // list.add(App.getDataNode(br));
        // break;
        // case 4:
        // System.out.println("All Data");
        // list.showStuData();
        // break;
        // default:
        // System.out.println("invalid choice!");
        // break;
        // }
        // System.out.print("press any key to continue....");
        // br.readLine();
        // } while (choice != 0);
        // System.out.println("Thank You!");
        // br.close();

        TreeDemo tdemo = new TreeDemo();
        // tdemo.root = new TreeNode('A');

        // tdemo.root.left = new TreeNode('B');
        // tdemo.root.right = new TreeNode('C');
        // tdemo.root.left.left = new TreeNode('D');
        // tdemo.root.left.right = new TreeNode('E');
        // tdemo.root.left.left.right = new TreeNode('H');

        // tdemo.root.right.left = new TreeNode('F');
        // tdemo.root.right.right = new TreeNode('G');
        // tdemo.root.right.right.left = new TreeNode('I');

        tdemo.buildTree();
        System.out.print("Pre-Order:[");
        tdemo.preOrder(tdemo.root);
        System.out.println("]");
        System.out.print("In-Order:[");
        tdemo.inOrder(tdemo.root);
        System.out.println("]");
        System.out.print("Post-Order:[");
        tdemo.postOrder(tdemo.root);
        System.out.println("]");

    }

    // static Node getDataNode(BufferedReader br) throws NumberFormatException,
    // IOException {
    // System.out.print("Enter data:");
    // return new Node(Integer.parseInt(br.readLine()));
    // }
}