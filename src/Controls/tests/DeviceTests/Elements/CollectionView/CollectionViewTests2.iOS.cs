using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Handlers.Items2;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.Maui.DeviceTests;

public partial class CollectionViewTests
{
    [Fact]
    public async Task ItemsSourceGroupedClearDoestCrash_CV2()
    {
        SetupBuilder();

        var data = new List<string> { "test 1", "test 2", "test 3" };
        var groupData = new ObservableCollection<CollectionViewStringGroup>
                {
                    new ("Header 1", data),
                    new ("Header 2", data),
                    new ("Header 3", data)
                };

        var collectionView = new CollectionView2
        {
            IsGrouped = true,
            ItemsSource = groupData,
            ItemTemplate = new DataTemplate(() => new Label())
        };

        await CreateHandlerAndAddToWindow<CollectionViewHandler2>(collectionView, async handler =>
        {
            await Task.Delay(1000);
            groupData.Clear();
            groupData.Add(new("Header 1", new string[] { "oi" }));
        });
    }

    [Fact]
    public async Task CollectionViewContentRespectsMargin_CV2()
    {
        SetupBuilder();

        // We'll use an EmptyView to assess whether the CollectionView's content 
        // is being properly offset by the margin
        var emptyView = new VerticalStackLayout();
        var emptyViewContent = new Label { Text = "test" };
        emptyView.Add(emptyViewContent);

        double margin = 2;

        var collectionView = new CollectionView2
        {
            Margin = new Thickness(margin),
            EmptyView = emptyView,
        };

        var frame = collectionView.Frame;

        await CreateHandlerAndAddToWindow<CollectionViewHandler2>(collectionView, async handler =>
        {
            await WaitForUIUpdate(frame, collectionView);

            if (emptyViewContent.Handler.PlatformView is not UIView nativeLabel)
            {
                throw new XunitException("EmptyView Content is not a UIView");
            }

            var point = new CGPoint(nativeLabel.Frame.Left, nativeLabel.Frame.Top);

            // Convert the local point to an absolute point in the window 
            var absPoint = nativeLabel.ConvertPointToView(point, null);

            Assert.Equal(margin, absPoint.X);
        });
    }

    [Fact("Cells Do Not Leak CV2")]
    public async Task CellsDoNotLeak_CV2()
    {
        SetupBuilder();

        var labels = new List<WeakReference>();
        VerticalCell cell = null;

        {
            var bindingContext = "foo";
            var collectionView = new MyUserControl2
            {
                Labels = labels
            };
            collectionView.ItemTemplate = new DataTemplate(collectionView.LoadDataTemplate);

            var handler = await CreateHandlerAsync(collectionView);

            await InvokeOnMainThreadAsync(() =>
            {
                cell = new VerticalCell(CGRect.Empty);
                cell.Bind(collectionView.ItemTemplate, bindingContext, collectionView);
            });

            Assert.NotNull(cell);
        }

        // HACK: test passes running individually, but fails when running entire suite.
        // Skip the assertion on Catalyst for now.
#if !MACCATALYST
        await AssertionExtensions.WaitForGC(labels.ToArray());
#endif
    }

    /// <summary>
    /// Simulates what a developer might do with a Page/View
    /// </summary>
    class MyUserControl2 : CollectionView2
    {
        public List<WeakReference> Labels { get; set; }

        /// <summary>
        /// Used for reproducing a leak w/ instance methods on ItemsView.ItemTemplate
        /// </summary>
        public object LoadDataTemplate()
        {
            var label = new Label();
            Labels.Add(new(label));
            return label;
        }
    }
}
