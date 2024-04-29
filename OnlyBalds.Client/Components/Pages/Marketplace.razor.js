export const onLoad = () => {
    $('.navbar-collapse').collapse('hide');
}

export const downloadMarketplace = (elementId, dotnetHelper) => {
    var scriptUrl = 'https://app.ecwid.com/script.js?103074754&data_platform=code&data_date=2024-04-29';
    var scripts = Array.from(document.getElementsByTagName('script'));
    var scriptElement = scripts.find(s => s.src === scriptUrl);

    if (!scriptElement) {
        var script = document.createElement('script');
        script.src = scriptUrl;
        script.charset = 'utf-8';
        script.setAttribute('data-cfasync', 'false');
        var container = document.getElementById(elementId);
        container.appendChild(script);
        
        script.onload = function() {
            xProductBrowser("categoriesPerRow=3","views=grid(20,3) list(60) table(60)","categoryView=grid","searchView=list","id=my-store-103074754");
            dotnetHelper.invokeMethod('CompleteDownloading');
        };
    } else {
        xProductBrowser("categoriesPerRow=3","views=grid(20,3) list(60) table(60)","categoryView=grid","searchView=list","id=my-store-103074754");
        dotnetHelper.invokeMethod('CompleteDownloading');
    }

    console.log("Marketplace downloaded.");
}