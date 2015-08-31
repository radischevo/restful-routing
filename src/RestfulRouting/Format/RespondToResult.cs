using System;
using System.Web.Mvc;

namespace RestfulRouting.Format
{
    public class RespondToResult : ActionResult
    {
        private readonly MultiMimeFormat _format;

        public RespondToResult(MultiMimeFormat format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            _format = format;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var format = Convert.ToString(context.RouteData.Values["format"]);
            var acceptTypes = context.HttpContext.Request.AcceptTypes;
            var result = _format.ResultFor(format, acceptTypes);

            result.ExecuteResult(context);
        }
    }
}