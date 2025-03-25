using UnityEngine;

public class PageHandler : MonoBehaviour
{
    public static int pageIndex = 1;

    public void SetPageIndex(int newPageIndex)
    {
        if (newPageIndex >= 1)
        {
            pageIndex = newPageIndex;
        }
    }

    public int GetPageIndex()
    {
        return pageIndex;
    }
}
