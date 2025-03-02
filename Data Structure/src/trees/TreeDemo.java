package trees;

import java.util.Scanner;

public class TreeDemo {

    Scanner sc = new Scanner(System.in);
    public TreeNode root;

    public TreeDemo() {
        root = null;
    }

    public void preOrder(TreeNode cTreeNode) {
        // NLR
        if (cTreeNode == null) {
            return;
        }
        System.out.print(cTreeNode.data + " ");
        preOrder(cTreeNode.left);// recursion
        preOrder(cTreeNode.right);// // recursion
    }

    public void inOrder(TreeNode cTreeNode) {
        // LNR
        if (cTreeNode == null) {
            return;
        }
        inOrder(cTreeNode.left);
        System.out.print(cTreeNode.data + " ");
        inOrder(cTreeNode.right);
    }

    public void postOrder(TreeNode cTreeNode) {
        // LRN
        if (cTreeNode == null) {
            return;
        }
        postOrder(cTreeNode.left);
        postOrder(cTreeNode.right);
        System.out.print(cTreeNode.data + " ");
    }

    TreeNode buildTreeUtil(TreeNode treeCn) {
       
            System.out.print("Enter Node Data:");
            char ch = sc.next().charAt(0);
            if (ch == 'X' || ch == 'x') {
                return null;
            } else {
                treeCn=new TreeNode(ch);
                System.out.println("going left of "+treeCn.data);
                treeCn.left=buildTreeUtil(treeCn.left);
                System.out.println("going right of "+treeCn.data);
                treeCn.right=buildTreeUtil(treeCn.right);
                return treeCn;
            }
        

    }

    public void buildTree() {
        // if (root == null) {
        //     System.out.print("Enter Root Node Data:");
        //     root = new TreeNode(sc.next().charAt(0));
        // } else {
            this.root = buildTreeUtil(this.root);
        // }
    }
}