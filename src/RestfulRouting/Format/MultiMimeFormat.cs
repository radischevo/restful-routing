using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace RestfulRouting.Format
{
    public class MultiMimeFormat
    {
        private static MimeTypeList _mimeTypes;

        static MultiMimeFormat()
        {
            _mimeTypes = new MimeTypeList();
            _mimeTypes.InitializeDefaults();
        }

        private readonly ICollection<FormatSelector> _selectors = new List<FormatSelector>();
        private FormatSelector _default;

        public static MimeTypeList MimeTypes
        {
            get
            {
                return _mimeTypes;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _mimeTypes = value;
            }
        }

        public ActionResult ResultFor(string format, string[] acceptTypes)
        {
            var mimeTypes = _mimeTypes.Parse(acceptTypes);
            var selector = _selectors.FirstOrDefault(s => s.Match(format, mimeTypes)) ?? _default;
            if (selector == null)
            {
                return new HttpStatusCodeResult(406, "Not Acceptable");
            }

            return selector.GetResult();
        }

        public IResultFormatConfiguration Mime(string format, Func<ActionResult> responder)
        {
            var selector = new FormatSelector(format, responder);
            _selectors.Add(selector);

            return new FormatConfiguration(this, selector);
        }

        public IResultFormatConfiguration Html(Func<ActionResult> responder)
        {
            return Mime("html", responder);
        }

        public IResultFormatConfiguration Json(Func<ActionResult> responder)
        {
            return Mime("json", responder);
        }

        public IResultFormatConfiguration Javascript(Func<ActionResult> responder)
        {
            return Mime("js", responder);
        }

        public IResultFormatConfiguration Xml(Func<ActionResult> responder)
        {
            return Mime("xml", responder);
        }

        public IResultFormatConfiguration Text(Func<ActionResult> responder)
        {
            return Mime("text", responder);
        }

        public void Any(Func<ActionResult> responder)
        {
            Mime("any", responder).IsDefault();
        }

        private sealed class FormatSelector
        {
            private readonly string _format;
            private readonly Func<ActionResult> _resultFactory;

            public FormatSelector(string format, Func<ActionResult> resultFactory)
            {
                _format = format;
                _resultFactory = resultFactory;
            }

            public ActionResult GetResult()
            {
                return _resultFactory();
            }

            public bool Match(string format, IEnumerable<MimeType> mimeTypes)
            {
                if (String.Equals(format, _format))
                {
                    return true;
                }

                return mimeTypes.Any(mimeType => String.Equals(_format, mimeType.Format, StringComparison.OrdinalIgnoreCase));
            }
        }

        private sealed class FormatConfiguration : IResultFormatConfiguration
        {
            private readonly MultiMimeFormat _format;
            private readonly FormatSelector _selector;

            public FormatConfiguration(MultiMimeFormat format, FormatSelector selector)
            {
                _format = format;
                _selector = selector;
            }

            public void IsDefault()
            {
                if (_format._default != null)
                {
                    throw new InvalidOperationException("Default format has already been set for this result.");
                }

                _format._default = _selector;
            }
        }
    }
}