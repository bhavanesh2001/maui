using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Microsoft.Maui.Platform;

internal class LabelHyperLinkHelper
{
    NSLayoutManager? _layoutManager;
    NSTextStorage? _textStorage;
    NSTextContainer? _textContainer;

    NSAttributedString? _lastAttributedText;
    CGSize _lastSize;
    WeakReference<MauiLabel>? _mauiLabel;

    public LabelHyperLinkHelper(MauiLabel label)
    {
        _mauiLabel = new WeakReference<MauiLabel>(label);
    }

    public void UpdateLayoutIfNeeded(NSAttributedString attributedText, CGSize size)
    {
        if (_mauiLabel is null)
            return;

        if (_layoutManager is null || _textStorage is null || _textContainer is null ||
            !_lastAttributedText?.IsEqual(attributedText) == true || !_lastSize.Equals(size))
        {
            _lastAttributedText = attributedText;
            _lastSize = size;

            _layoutManager = new NSLayoutManager();
            _textStorage = new NSTextStorage();
            _textContainer = new NSTextContainer(size)
            {
                LineFragmentPadding = 0
            };

            if (_mauiLabel.TryGetTarget(out var label))
            {
                _textContainer.LineBreakMode = label.LineBreakMode;
                _textContainer.MaximumNumberOfLines = (nuint)label.Lines;
            }

            _textStorage.SetString(attributedText);
            _layoutManager.AddTextContainer(_textContainer);
            _textStorage.AddLayoutManager(_layoutManager);
            _layoutManager.EnsureLayoutForTextContainer(_textContainer);
        }
    }

    public NSUrl? TryGetUrlAtPoint(CGPoint location)
    {
        if (_mauiLabel is null || _layoutManager is null || _textStorage is null || _textContainer is null || _lastAttributedText is null)
            return null;

        if (!_mauiLabel.TryGetTarget(out var label) || label.Font is null || label.AttributedText is null)
            return null;

        _layoutManager.EnsureLayoutForTextContainer(_textContainer);

        var charIndex = (nint)_layoutManager.GetCharacterIndex(location, _textContainer);

        var attr = _lastAttributedText.GetAttribute(UIStringAttributeKey.Link, charIndex, out _);

        return attr as NSUrl;
    }

    public void TryOpenHyperlinkAt(CGPoint location)
    {
        if (_mauiLabel is null)
            return;

        if (!_mauiLabel.TryGetTarget(out var label) || label.AttributedText is null)
            return;

        var bounds = label.Bounds;
        var textInsets = label.TextInsets;
        var contentSize = new CGSize(
            bounds.Width - textInsets.Left - textInsets.Right,
            bounds.Height - textInsets.Top - textInsets.Bottom);

        UpdateLayoutIfNeeded(label.AttributedText, contentSize);

        var url = TryGetUrlAtPoint(location);

        if (url != null)
        {
            UIApplication.SharedApplication.OpenUrl(url, new UIApplicationOpenUrlOptions(), null);
        }
    }

    internal void Invalidate()
    {
        _layoutManager = null;
        _textStorage = null;
        _lastAttributedText = null;
        _textContainer = null;
        _mauiLabel = null;
    }
}
