using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Kipa_plus.Filters
{
    internal class AddResourcesToViewFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //var styles = SetResource("Kipa-plus.wwwroot.css.jquery.bonsai.min.css");
            //styles += SetResource("Kipa_plus.wwwroot.css.site.min.css");

            //var scripts = SetResource("Kipa_plus.wwwroot.js.jquery.qubit.min.js");
            //scripts += SetResource("Kipa_plus.wwwroot.js.jquery.bonsai.min.js");
            //scripts += SetResource("Kipa_plus.wwwroot.js.site.min.js");

            //var controller = context.Controller as Controller;
            //controller.ViewData["Styles"] = styles;
            //controller.ViewData["Scripts"] = scripts;
        }

        private static string SetResource(string resourceName)
        {
            var resourceStream = typeof(AddResourcesToViewFilter).Assembly.GetManifestResourceStream(resourceName);

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var resource = reader.ReadToEnd();
                return resource;
            }
        }
    }
}
