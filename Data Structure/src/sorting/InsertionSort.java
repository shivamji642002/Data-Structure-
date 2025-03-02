package sorting;

public class InsertionSort {

    /**
     * use this sorting technique when list is nearly sorted<br>
     * Time Complexity in Best Case O(n)<br>
     * Time Complexity in Worst Case O(n^2)<br>
     * 
     * @param arr array to be sorted
     */
    public static void insertionSort(int arr[]) {
        int size = arr.length;
        for (int i = 1; i < size; i++) {
            System.out.println("Pass " + (i));
            int key = arr[i];
            int j = i - 1;
            while (j >= 0 && arr[j] > key) {
                System.out.println("comparing " + key + " with " + arr[j] + " j=" + j);
                arr[j + 1] = arr[j];
                j = j - 1;
            }
            arr[j + 1] = key;
        }
    }
}