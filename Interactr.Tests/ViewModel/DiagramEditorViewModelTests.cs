﻿using Interactr.Model;
using Interactr.ViewModel;
using NUnit.Framework;

namespace Interactr.Tests.ViewModel
{
    [TestFixture]
    public class DiagramEditorViewModelTests
    {
        private DiagramEditorViewModel _mainViewModel;

        [SetUp]
        public void Before()
        {
            _mainViewModel = new DiagramEditorViewModel(new Diagram());
        }

        [Test]
        public void SequenceDiagramViewVisibilityChangesWithSwitchViews()
        {
            bool expected = !_mainViewModel.SeqDiagramVM.IsVisible;
            _mainViewModel.SwitchViews();
            bool actual = _mainViewModel.SeqDiagramVM.IsVisible;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CommunicationDiagramViewVisibilityChangesWithSwitchViews()
        {
            bool expected = !_mainViewModel.CommDiagramVM.IsVisible;
            _mainViewModel.SwitchViews();
            bool actual = _mainViewModel.CommDiagramVM.IsVisible;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SequenceDiagramViewVisibilityStaysTheSameWithSwitchViews()
        {
            bool expected = _mainViewModel.SeqDiagramVM.IsVisible;
            _mainViewModel.SwitchViews();
            _mainViewModel.SwitchViews();
            bool actual = _mainViewModel.SeqDiagramVM.IsVisible;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CommunicationDiagramViewVisibilityStaysTheSameWithSwitchViews()
        {
            bool expected = _mainViewModel.CommDiagramVM.IsVisible;
            _mainViewModel.SwitchViews();
            _mainViewModel.SwitchViews();
            bool actual = _mainViewModel.CommDiagramVM.IsVisible;

            Assert.AreEqual(expected, actual);
        }
    }
}