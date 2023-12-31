﻿using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace PrintLib
{
    public class PageRangeDocumentPaginator : DocumentPaginator
    {
        private readonly int _startIndex;
        private readonly int _endIndex;
        private readonly DocumentPaginator _paginator;
        public PageRangeDocumentPaginator(DocumentPaginator paginator, PageRange pageRange)
        {
            _startIndex = pageRange.PageFrom - 1;
            _endIndex = pageRange.PageTo - 1;
            _paginator = paginator;

            // Adjust the _endIndex
            _endIndex = Math.Min(_endIndex, _paginator.PageCount - 1);
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            var page = _paginator.GetPage(pageNumber + _startIndex);

            // Create a new ContainerVisual as a new parent for page children
            var cv = new ContainerVisual();
            if (page.Visual is FixedPage fp)
            {
                foreach (var child in fp.Children)
                {
                    // Make a shallow clone of the child using reflection
                    var childClone = (UIElement?)child.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(child, null);

                    // Setting the parent of the cloned child to the created ContainerVisual by using Reflection.
                    // WARNING: If we use Add and Remove methods on the FixedPage.Children, for some reason it will
                    //          throw an exception concerning event handlers after the printing job has finished.
                    var parentField = childClone?.GetType().GetField("_parent", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (parentField != null)
                    {
                        parentField.SetValue(childClone, null);
                        cv.Children.Add(childClone);
                    }
                }

                return new DocumentPage(cv, page.Size, page.BleedBox, page.ContentBox);
            }

            return page;
        }

        public override bool IsPageCountValid => true;

        public override int PageCount
        {
            get
            {
                if (_startIndex > _paginator.PageCount - 1)
                {
                    return 0;
                }

                if (_startIndex > _endIndex)
                {
                    return 0;
                }

                return _endIndex - _startIndex + 1;
            }
        }

        public override Size PageSize
        {
            get { return _paginator.PageSize; }
            set { _paginator.PageSize = value; }
        }

        public override IDocumentPaginatorSource Source => _paginator.Source;
    }
}
