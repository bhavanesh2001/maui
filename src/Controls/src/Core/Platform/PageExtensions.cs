#nullable disable
using System;
using System.Linq;

namespace Microsoft.Maui.Controls.Platform
{
	public static class PageExtensions
	{
		internal static Page GetCurrentPage(this Page currentPage)
		{
			if (currentPage.NavigationProxy.ModalStack.LastOrDefault() is Page modal)
				return modal;
			else if (currentPage is FlyoutPage fp)
				return GetCurrentPage(fp.Detail);
			else if (currentPage is Shell shell && shell.CurrentItem?.CurrentItem is IShellSectionController ssc)
				return ssc.PresentedPage;
			else if (currentPage is IPageContainer<Page> pc)
				return GetCurrentPage(pc.CurrentPage);
			else
				return currentPage;
		}

		internal static void ClearInternalChildren(this Page page)
		{
			// Removes all pushed pages from InternalChildren, preserving the root page to maintain NavigationPage validity.
			var internalChildren = page.InternalChildren;
			if (internalChildren.Count > 1)
			{
				for (int i = internalChildren.Count - 1; i > 0; i--)
				{
					internalChildren.Remove(internalChildren[i]);
				}
			}
		}
	}
}