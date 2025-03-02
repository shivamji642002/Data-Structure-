package arrays;

import java.util.Arrays;

public class crud {
    static int arr[];

    public static void tryArray(int arr[]) {
        crud.arr = arr;
        // printArray();
        insert(1, 25);
    }

    static void printArray() {
        System.out.print("[");
        for (int i = 0; i < arr.length; i++) {
            System.out.print(arr[i]);
            if (i < arr.length - 1)
                System.out.print(",");
        }
        System.out.println("]");
    }
    static void insert(int pos,int value)
    {
        if (pos>arr.length || pos<0) // 
        {
            return;    
        }
        arr=Arrays.copyOf(arr, arr.length+1);
        for(int i=arr.length-2;i>=pos;i--)
        {
            arr[i+1]=arr[i];
        }
        arr[pos]=value;
        printArray();
    }
}
