using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Interactr.View.Controls;
using Interactr.View.Framework;
using Microsoft.Reactive.Testing;

namespace Interactr.Tests.View.Framework
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
            elem1.Children.Remove(elem2);
            Assert.IsNull(elem2.Parent);
        }

        [Test]
        public void TestFocus()
        {
            UIElement elem1 = new UIElement();
            UIElement elem2 = new UIElement();


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
            // Setup
            var scheduler = new TestScheduler();
            UIElement elem1 = new UIElement();
            UIElement elem2 = new UIElement();

            // Define actions
            scheduler.Schedule(TimeSpan.FromTicks(10), () => elem1.Focus());
            scheduler.Schedule(TimeSpan.FromTicks(20), () => elem2.Focus());
            var actual = scheduler.Start(() => elem1.FocusChanged, created: 0, subscribed: 0, disposed: 100);

            // Assert
            var expected = new[]
            {
                OnNext(1, false),
                OnNext(10, true),
                OnNext(20, false)
            };
            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [Test]
        public void TestFindElementAt()
        {
            UIElement elem1 = new UIElement();
            UIElement elem2 = new UIElement
            {
                Position = new Point(10, 10),
                Width = 10,
                Height = 10
            };
            elem1.Children.Add(elem2);

            Assert.AreEqual(elem1, elem1.FindElementAt(new Point(5, 5)));
            Assert.AreEqual(elem2, elem1.FindElementAt(new Point(12, 12)));
            Assert.AreEqual(elem1, elem1.FindElementAt(new Point(20, 20)));
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
                Position = new Point(10, 10),
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

        [Test]
        public void TestValidateLayout()
        {
            TestableUIElement parent = new TestableUIElement
            {
                Width = 100,
                Height = 100
            };

            UIElement child1 = new UIElement
            {
                Width = 200,
                Height = 200
            };
            parent.Children.Add(child1);

            UIElement child2 = new UIElement
            {
                Width = 100,
                Height = 100
            };
            parent.Children.Add(child2);

            UIElement child3 = new UIElement
            {
                Position = new Point(90, 90),
                Width = 50,
                Height = 50
            };
            parent.Children.Add(child3);

            UIElement child4 = new UIElement
            {
                Width = 50,
                Height = 50
            };
            parent.Children.Add(child4);

            parent.RunValidateLayout();

            Assert.AreEqual(100, child1.Width);
            Assert.AreEqual(100, child1.Height);

            Assert.AreEqual(100, child2.Width);
            Assert.AreEqual(100, child2.Height);

            Assert.AreEqual(10, child3.Width);
            Assert.AreEqual(10, child3.Height);

            Assert.AreEqual(50, child4.Width);
            Assert.AreEqual(50, child4.Height);
        }

        [Test]
        public void TestGetDecendantsOneLevelDown()
        {
            // Build a UIElement tree with two children one level down the root element.

            UIElement root = new UIElement();
            UIElement childElement = new UIElement();
            UIElement childElement2 = new UIElement();
            root.Children.Add(childElement);
            root.Children.Add(childElement2);

            IEnumerable<UIElement> decendants = root.GetDecendants();

            // The number of decendants is correct.
            Assert.AreEqual(2, decendants.Count());

            // The decendants contain the expected elements.
            Assert.True(decendants.Contains(childElement));
            Assert.True(decendants.Contains(childElement2));
        }

        [Test]
        public void TestGetDecendantsMultipleLevelsDown()
        {
            // Build a UIElement tree with two children one level down the root element.
            UIElement root = new UIElement();
            UIElement childElement = new UIElement();
            UIElement childElement2 = new UIElement();
            root.Children.Add(childElement);
            root.Children.Add(childElement2);

            // Add elements to the two levels down.
            UIElement childElement3 = new UIElement();
            UIElement childElement4 = new UIElement();
            childElement.Children.Add(childElement3);
            childElement.Children.Add(childElement4);

            // Add an element 3 levels down.
            UIElement childElement5 = new UIElement();
            childElement3.Children.Add(childElement5);

            IEnumerable<UIElement> decendants = root.GetDecendants();

            // The number of decendants is correct.
            Assert.AreEqual(5, decendants.Count());

            // The decendants contain the expected elements.
            Assert.True(decendants.Contains(childElement));
            Assert.True(decendants.Contains(childElement2));
            Assert.True(decendants.Contains(childElement3));
            Assert.True(decendants.Contains(childElement4));
            Assert.True(decendants.Contains(childElement5));
        }

        [Test]
        public void AbsolutePositionElementItSelfChanged()
        {
            var scheduler = new TestScheduler();

            var root = new UIElement();
            var element = new TestableUIElement();

            // Add element to root as a child.
            root.Children.Add(element);

            // Change the position of the element.
            scheduler.Schedule(TimeSpan.FromTicks(10), () => element.Position = new Point(86, 4));

            var expected = new[]
            {
                ReactiveTest.OnNext(1, new Point(0, 0)),
                ReactiveTest.OnNext(10, new Point(86, 4))
            };

            var actual = scheduler.Start(() => element.AbsolutePositionChanged, 0, 0, 1000).Messages;
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void AbsolutePositionParentChanged()
        {
            var scheduler = new TestScheduler();

            var root = new UIElement();
            var element = new RectangleView();
            var child = new Button();

            // Add element to root as a child.
            root.Children.Add(element);
            element.Children.Add(child);

            // Change the position of the element.
            scheduler.Schedule(TimeSpan.FromTicks(10), () => element.Position = new Point(86, 4));

            var expected = new[]
            {
                ReactiveTest.OnNext(1, new Point(0, 0)),
                ReactiveTest.OnNext(10, new Point(86, 4))
            };
            
            var actual = scheduler.Start(() => child.AbsolutePositionChanged, 0, 0, 1000).Messages;
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void ObserveParentWidth()
        {
            var scheduler = new TestScheduler();

            var parent = new UIElement();
            var child = new RectangleView();
            var observable = child.ParentChanged.Where(t => t != null && child.IsVisible).Select(p => p.WidthChanged).Switch();
            
            // Setup parent child relationship
            parent.Children.Add(child);

            // Change the width of the parent.
            scheduler.Schedule(TimeSpan.FromTicks(10), () => parent.Width = 500);

            var expected = new[]
            {
                ReactiveTest.OnNext(1, 0),
                ReactiveTest.OnNext(10, 500)
            };

            var actual = scheduler.Start(() => observable, 0, 0, 1000).Messages;
            ReactiveAssert.AreElementsEqual(expected, actual);
        }

        [Test]
        public void AbsolutePositionGrandparentChanged()
        {
            var scheduler = new TestScheduler();

            var root = new UIElement();
            var element = new UIElement();
            var child = new UIElement();
            var grandChild = new UIElement();

            // Add element to root as a child.
            root.Children.Add(element);
            element.Children.Add(child);
            child.Children.Add(grandChild);

            // Change the position of the element.
            scheduler.Schedule(TimeSpan.FromTicks(10), () => element.Position = new Point(10, 10));

            // Check if the absolute position of the grandChild changes.
            var expected = new[]
            {
                ReactiveTest.OnNext(1, new Point(0, 0)),
                ReactiveTest.OnNext(10, new Point(10, 10))
            };

            var actual = scheduler.Start(() => grandChild.AbsolutePositionChanged, 0, 0, 1000).Messages;
            ReactiveAssert.AreElementsEqual(expected, actual);
        }


        class TestableUIElement : UIElement
        {
            public void RunValidateLayout()
            {
                ValidateLayout();
            }
        }
    }
}