using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutoresV2.Utilities
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceController = controller.ControllerType.Namespace; //namepace del controllador por ejemplo v1

            var versionApi = namespaceController.Split(".").Last();//obtner la version del api 
            controller.ApiExplorer.GroupName = versionApi; // agrupando por el nombre del namespace. 

        }
    }
}
