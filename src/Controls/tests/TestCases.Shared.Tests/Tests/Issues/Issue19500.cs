#if TEST_FAILS_ON_CATALYST // Since this test verifies the scrolling behavior in the editor control and the App.ScrollDown function is inconsistent, relying on screenshot verification causes flakiness in CI. It needs to be validated using an alternative approach, or else it would be better to consider this for manual testing.
// Issue for reenable: https://github.com/dotnet/maui/issues/29025
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues
{
	public class Issue19500 : _IssuesUITest
	{
		public override string Issue => "[iOS] Editor is not be able to scroll if IsReadOnly is true";

		public Issue19500(TestDevice device) : base(device)
		{
		}

		[Test]
		[Category(UITestCategories.Editor)]
		public void TextInEditorShouldScroll()
		{
			_ = App.WaitForElement("editor");
			App.ScrollDown("editor");

			// The test passes if the text inside the editor scrolls down
			VerifyScreenshot();
		}
	}
}
#endif