using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace xampl.Utils
{
    public class ToastUtils
    {
        public static void BindData(dynamic viewBag, ITempDataDictionary tempData)
        {
            viewBag.ToastMessage = tempData["ToastMessage"];
            viewBag.ToastIsAlert = tempData["ToastIsAlert"];
        }

        public static void SetData(ITempDataDictionary tempData, string message, bool isAlert = false)
        {
            tempData["ToastMessage"] = message;
            tempData["ToastIsAlert"] = isAlert;
        }
    }
}
