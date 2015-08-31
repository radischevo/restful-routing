using System.Web.Mvc;
using Machine.Specifications;
using RestfulRouting.Format;

namespace RestfulRouting.Spec.Format
{
    public class format_result_base
    {
        private Establish context = () =>
                                        {
                                            _formatCollection = new MultiMimeFormat();
                                            _format = "html";
                                            _acceptTypes = new string[] {};
                                        };

        Because of = () => _result = _formatCollection.ResultFor(_format, _acceptTypes);

        protected static MultiMimeFormat _formatCollection;
        protected static ActionResult _result;
        protected static string _format;
        protected static string[] _acceptTypes;
    }

    public class format_result_with_matching_format : format_result_base
    {
        Establish context = () => _formatCollection.Html(() => new ContentResult());

        It returns_the_associated_action_result = () => _result.ShouldBeOfType<ContentResult>();
    }

    public class format_result_with_unknown_format : format_result_base
    {
        It returns_not_found_result = () => {
                                                _result.ShouldBeOfType<HttpStatusCodeResult>();
                                                ((HttpStatusCodeResult) _result).StatusCode.ShouldEqual(406);
                                            };
    }

    public class format_result_with_default_result_and_unknown_format : format_result_base
    {
        Establish context = () => _formatCollection.Any(() => new ViewResult());

        It returns_the_default_result = () => _result.ShouldBeOfType<ViewResult>();
    }

    public class format_result_with_no_format_and_accept_types_application_json : format_result_base
    {
        Establish context = () =>
                                {
                                    _acceptTypes = new[] {"application/json"};
                                    _formatCollection.Json(() => new JsonResult());
                                    _format = null;
                                };

        // pending
        It returns_json = () => _result.ShouldBeOfType<JsonResult>();
    }

    public class format_result_with_no_format_and_no_accept_types : format_result_base
    {
        Establish context = () => _format = null;

        It returns_406_not_acceptable = () =>
                                            {
                                                _result.ShouldBeOfType<HttpStatusCodeResult>();
                                                ((HttpStatusCodeResult) _result).StatusCode.ShouldEqual(406);
                                            };
    }
}