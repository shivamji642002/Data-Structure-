package searching;

public class Linear {

    public static int search(int arr[],int key) 
    {
        int foundIndex=-1;
        for (int i = 0; i < arr.length; i++) {
            if (arr[i]==key) {
                System.out.println("found "+key+" at "+i+"th index");
                foundIndex=i;
                break;
            }
        }
        return foundIndex;
    }
}