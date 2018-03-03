﻿using Interactr.View.Controls;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;
using Microsoft.Reactive.Testing;

namespace Interactr.Tests.View.Controls
{
    [TestFixture]
    public class UIElementTests : ReactiveTest
    {
        [Test]
        public void TestTreeStructure()
        {
            UIElement elem1 = new UIElement();
            UIElement elem2 = new UIElement();
            Assert.IsNull(elem2.Parent);
            elem1.Children.Add(elem2);
            Assert.AreEqual(elem2.Parent, elem1);
            elem2.Children.Remove(elem1);
            Assert.IsNull(elem2.Parent);
        }

        [Test]
        public void TestFocus()
        {
            UIElement elem1 = new UIElement();
            UIElement elem2 = new UIElement();

            Assert.IsNull(UIElement.FocusedElement);
            
            elem1.Focus();
            Assert.AreEqual(elem1, UIElement.FocusedElement);
            Assert.IsTrue(elem1.IsFocused);
            Assert.IsFalse(elem2.IsFocused);

            elem2.Focus();
            Assert.AreEqual(elem2, UIElement.FocusedElement);
            Assert.IsTrue(elem2.IsFocused);
            Assert.IsFalse(elem1.IsFocused);
        }

        [Test]
        public void TestFocusObservable()
        {
            //Setup
            var scheduler = new TestScheduler();
            UIElement elem1 = new UIElement();
            UIElement elem2 = new UIElement();

            //Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => elem1.Focus());
            scheduler.Schedule(TimeSpan.FromTicks(20), () => elem2.Focus());
            var actual = scheduler.Start(() => elem1.FocusChanged, created: 0, subscribed: 0, disposed: 100);

            //Assert
            var expected = new[]
            {
                OnNext(1, false),
                OnNext(10, true),
                OnNext(20, false)
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TranslatePointToTest()
        {
            UIElement root = new UIElement
            {
                Position = new Point(0, 0),
                Width = 500,
                Height = 500
            };
            UIElement child1 = new UIElement
            {
                Position = new Point(10, 15),
                Width = 200,
                Height = 200
            };
            root.Children.Add(child1);
            UIElement child2 = new UIElement
            {
                Position = new Point(220, 220),
                Width = 270,
                Height = 95
            };
            root.Children.Add(child2);
            UIElement child3 = new UIElement
            {
                Position = new Point(220, 105),
                Width = 270,
                Height = 95
            };
            root.Children.Add(child3);
            UIElement child4 = new UIElement
            {
                Position = new Point(20, 20),
                Width = 50,
                Height = 50
            };
            child1.Children.Add(child4);

            Point testPointUp = new Point(25, 25);
            Point testPointDown = new Point(25, 25);
            Point testPointDown2 = new Point(50, 50);

            Point testPointUpInRoot = child4.TranslatePointTo(root, testPointUp);
            Point testPointUpInChild1 = child4.TranslatePointTo(child1, testPointUp);

            Point testPointDownInChild4 = root.TranslatePointTo(child4, testPointDown);
            Point testPointDown2InChild4 = root.TranslatePointTo(child4, testPointDown2);


            Assert.AreEqual(testPointUpInRoot, new Point(55, 55));
            Assert.AreEqual(testPointUpInChild1, new Point(45, 45));

            Assert.AreEqual(testPointDownInChild4, new Point(-5, -5));
            Assert.AreEqual(testPointDown2InChild4, new Point(20, 20));

        }
    }
}