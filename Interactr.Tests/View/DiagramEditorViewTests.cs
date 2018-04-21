using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.View;
using Interactr.View.Framework;
using Interactr.Window;
using NUnit.Framework;

namespace Interactr.Tests.View
{
    public class DiagramEditorViewTests
    {
        private TestableDiagramEditorView _diagramEditorView;

        [SetUp]
        public void Before()
        {
            _diagramEditorView = new TestableDiagramEditorView();
        }

        [Test]
        public void SwitchViewsOnTab()
        {
            var expectedSeqDiagVisible = !_diagramEditorView.SeqDiagView.IsVisible;
            var expectedCommDiagVisible = !_diagramEditorView.CommDiagView.IsVisible;

            // Press TAB
            var tabPressedData = new KeyEventData(KeyEvent.KEY_PRESSED, KeyEvent.VK_TAB, ' ');
            _diagramEditorView.RunOnKeyEvent(tabPressedData);

            var actualSeqDiagVisible = _diagramEditorView.SeqDiagView.IsVisible;
            var actualCommDiagVisible = _diagramEditorView.CommDiagView.IsVisible;

            Assert.AreEqual(expectedSeqDiagVisible, actualSeqDiagVisible);
            Assert.AreEqual(expectedCommDiagVisible, actualCommDiagVisible);
        }

        private class TestableDiagramEditorView : DiagramEditorView
        {
            public bool RunOnKeyEvent(KeyEventData eventData)
            {
                return OnKeyEvent(eventData);
            }
        }
    }
}
