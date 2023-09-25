// using System.Collections.Generic;
// using System;
// using Avalonia;
//
// namespace H.Infrastructure.Controls
// {
//     /// <summary>
//     /// http://www.hardcodet.net/2009/06/finding-elements-in-wpf-tree-both-ways
//     /// </summary>
//     public static class TreeHelper
//     {
//         public static T TryFindParent<T>(this AvaloniaObject child) where T : AvaloniaObject
//         {
//             var parentObject = GetParentObject(child);
//             if (parentObject == null)
//             {
//                 return null;
//             }
//
//             var parent = parentObject as T;
//             if (parent != null)
//             {
//                 return parent;
//             }
//
//             return TryFindParent<T>(parentObject);
//         }
//
//         public static AvaloniaObject GetParentObject(this AvaloniaObject child)
//         {
//             if (child == null)
//             {
//                 return null;
//             }
//
//             var contentElement = child as ContentElement;
//             if (contentElement != null)
//             {
//                 var parent = ContentOperations.GetParent(contentElement);
//                 if (parent != null)
//                 {
//                     return parent;
//                 }
//
//                 var fce = contentElement as FrameworkContentElement;
//                 return fce?.Parent;
//             }
//
//             var frameworkElement = child as FrameworkElement;
//             if (frameworkElement != null)
//             {
//                 var parent = frameworkElement.Parent;
//                 if (parent != null)
//                 {
//                     return parent;
//                 }
//             }
//
//             return VisualTreeHelper.GetParent(child);
//         }
//
//         public static IEnumerable<T> FindChildren<T>(this AvaloniaObject source) where T : AvaloniaObject
//         {
//             if (source != null)
//             {
//                 var children = GetChildObjects(source);
//                 foreach (var child in children)
//                 {
//                     if (child is T variable)
//                     {
//                         yield return variable;
//                     }
//
//                     foreach (var descendant in FindChildren<T>(child))
//                     {
//                         yield return descendant;
//                     }
//                 }
//             }
//         }
//
//         public static IEnumerable<AvaloniaObject> GetChildObjects(this AvaloniaObject parent)
//         {
//             if (parent == null)
//             {
//                 yield break;
//             }
//
//             if (parent is ContentElement || parent is FrameworkElement)
//             {
//                 foreach (var obj in LogicalTreeHelper.GetChildren(parent))
//                 {
//                     var depObj = obj as AvaloniaObject;
//                     if (depObj != null)
//                     {
//                         yield return (AvaloniaObject) obj;
//                     }
//                 }
//             }
//             else
//             {
//                 var count = VisualTreeHelper.GetChildrenCount(parent);
//                 for (var i = 0; i < count; i++)
//                 {
//                     yield return VisualTreeHelper.GetChild(parent, i);
//                 }
//             }
//         }
//
//         public static T TryFindFromPoint<T>(UIElement reference, Point point) where T : AvaloniaObject
//         {
//             var element = reference.InputHitTest(point) as AvaloniaObject;
//
//             if (element == null)
//             {
//                 return null;
//             }
//
//             if (element is T variable)
//             {
//                 return variable;
//             }
//
//             return TryFindParent<T>(element);
//         }
//
//         /// <summary>
//         /// Recursively traverses a xaml visual tree and finds a single instance of the object to look for. Returns null if nothing is found.
//         /// </summary>
//         /// <typeparam name="T">The parent on which the method will be called.</typeparam>
//         /// <param name="childObject">The child object that the method will look for.</param>
//         /// <returns></returns>
//         public static T GetChildOfType<T>(this AvaloniaObject childObject)
//         where T : AvaloniaObject
//         {
//             if (childObject == null)
//             {
//                 return null;
//             }
//
//             for (int i = 0; i < VisualTreeHelper.GetChildrenCount(childObject); i++)
//             {
//                 var child = VisualTreeHelper.GetChild(childObject, i);
//
//                 var result = (child as T) ?? GetChildOfType<T>(child);
//
//                 if (result != null)
//                 {
//                     return result;
//                 }
//             }
//             return null;
//         }
//
//     }
// }