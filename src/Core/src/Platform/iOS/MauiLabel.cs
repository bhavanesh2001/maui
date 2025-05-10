#nullable disable

using System;
using System.Diagnostics.CodeAnalysis;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Graphics;
using ObjCRuntime;
using UIKit;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;

namespace Microsoft.Maui.Platform
{
	public class MauiLabel : UILabel, IUIViewLifeCycleEvents
	{
		UIControlContentVerticalAlignment _verticalAlignment = UIControlContentVerticalAlignment.Center;
		CGPoint _touchStart;
		const double maxTapDistance = 2.0;
		bool _isTextTypeHTML;
		internal bool IsTextTypeHTML
		{
			get => _isTextTypeHTML;
			set
			{
				if (_hyperLinkHelper == null && value)
				{
					_hyperLinkHelper = new LabelHyperLinkHelper(this);
				}
				else if (!value)
				{
					_hyperLinkHelper?.Invalidate();
					_hyperLinkHelper = null;
				}
				_isTextTypeHTML = value;
			}
		}
		LabelHyperLinkHelper _hyperLinkHelper;
		public UIEdgeInsets TextInsets { get; set; }
		internal UIControlContentVerticalAlignment VerticalAlignment
		{
			get => _verticalAlignment;
			set
			{
				_verticalAlignment = value;
				SetNeedsDisplay();
			}
		}

		public MauiLabel(RectangleF frame) : base(frame)
		{
		}

		public MauiLabel()
		{
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			if (!IsTextTypeHTML) return;
				
			StoreTouchStart(touches);
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			if (!IsTextTypeHTML) return;

			if (!IsValidTouch(touches, out var location)) return;

			_hyperLinkHelper.TryOpenHyperlinkAt(location);
		}

		public override void DrawText(RectangleF rect)
		{
			rect = TextInsets.InsetRect(rect);

			if (_verticalAlignment != UIControlContentVerticalAlignment.Center
				&& _verticalAlignment != UIControlContentVerticalAlignment.Fill)
			{
				rect = AlignVertical(rect);
			}

			base.DrawText(rect);
		}

		void StoreTouchStart(NSSet touches)
		{
			if (touches.AnyObject is UITouch touch)
			{
				_touchStart = touch.LocationInView(this);
			}
		}

		bool IsValidTouch(NSSet touches, out CGPoint location)
		{
			location = default;

			if (AttributedText is null || touches is null || touches.Count == 0)
			{
				return false;
			}

			if (touches.AnyObject is not UITouch touch)
			{
				return false;
			}

			location = touch.LocationInView(this);

			return Math.Abs(location.X - _touchStart.X) <= maxTapDistance &&
				   Math.Abs(location.Y - _touchStart.Y) <= maxTapDistance;
		}

		RectangleF AlignVertical(RectangleF rect)
		{
			var frameSize = Frame.Size;
			var height = Lines == 1 ? Font.LineHeight : SizeThatFits(frameSize).Height;

			if (height < frameSize.Height)
			{
				if (_verticalAlignment == UIControlContentVerticalAlignment.Top)
				{
					rect.Height = height;
				}
				else if (_verticalAlignment == UIControlContentVerticalAlignment.Bottom)
				{
					rect = new RectangleF(rect.X, rect.Bottom - height, rect.Width, height);
				}
			}

			return rect;
		}

		public override SizeF SizeThatFits(SizeF size)
		{
			// Prior to calculating the text size, reduce the padding, and then add the padding back in the AddInsets method.
			var adjustedWidth = size.Width - TextInsets.Left - TextInsets.Right;
			var adjustedHeight = size.Height - TextInsets.Top - TextInsets.Bottom;
			var requestedSize = base.SizeThatFits(new SizeF(adjustedWidth, adjustedHeight));

			// Let's be sure the label is not larger than the container
			return AddInsets(new Size()
			{
				Width = nfloat.Min(requestedSize.Width, size.Width),
				Height = nfloat.Min(requestedSize.Height, size.Height),
			});
		}

		SizeF AddInsets(SizeF size) => new SizeF(
			width: size.Width + TextInsets.Left + TextInsets.Right,
			height: size.Height + TextInsets.Top + TextInsets.Bottom);

		[UnconditionalSuppressMessage("Memory", "MEM0002", Justification = IUIViewLifeCycleEvents.UnconditionalSuppressMessage)]
		EventHandler _movedToWindow;
		event EventHandler IUIViewLifeCycleEvents.MovedToWindow
		{
			add => _movedToWindow += value;
			remove => _movedToWindow -= value;
		}

		public override void MovedToWindow()
		{
			base.MovedToWindow();
			_movedToWindow?.Invoke(this, EventArgs.Empty);
		}

		internal void Disconnect()
		{
			_hyperLinkHelper?.Invalidate();
			_hyperLinkHelper = null;
		}
	}
}