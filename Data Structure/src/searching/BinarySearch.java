package searching;

public class BinarySearch {

    public static int binarySearch(int arr[],int key)
    {
        int foundIndex=-1;

        int low=0,high=arr.length-1,mid;

        while (low<=high) 
        {
            mid=(low+high)/2;

            if (arr[mid]==key) 
            {
                foundIndex=mid;
                break;
            } 
            if(key<arr[mid])
            {
                high=mid-1;
            }
            if(key>arr[mid])
            {
                low=mid+1;
            }
        }
        return foundIndex;
    }
}
