export const onLoad = () => {
    $('.navbar-collapse').collapse('hide');

    const config = {
        childList: true,
        subtree: true
    };

    const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            mutation.addedNodes.forEach((node) => {
                let classList = node.classList;

                if (classList === undefined) {
                    return;
                }

                $('.ec-store.ec-store__confirmation-page:gt(0)').remove();
            });
        });
    });

    observer.observe(document.body, config);

    return {
        disconnect: () => observer.disconnect()
    };
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
            xProductBrowser("id=my-store-103074754");
            dotnetHelper.invokeMethod('CompleteDownloading');
        };
    } else {
        dotnetHelper.invokeMethod('CompleteDownloading');
    }

    console.log("Marketplace downloaded.");
}

export const dispose = () => {
    var scripts = Array.from(document.getElementsByTagName('script'));
    var scriptUrl = 'https://app.ecwid.com/script.js?103074754&data_platform=code&data_date=2024-04-29';
    var scriptElement = scripts.find(s => s.src === scriptUrl);

    if (scriptElement) {
        scriptElement.remove();
        console.log("Marketplace disposed.");
    }
}