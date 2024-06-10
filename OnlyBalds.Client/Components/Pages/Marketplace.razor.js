export const onLoad = () => {
    $('.navbar-collapse').collapse('hide');

    let navLinks = document.querySelectorAll('.navbar-nav .nav-link');

    navLinks.forEach(function(navLink) {
        if (navLink.getAttribute('href') === "Marketplace") {
            navLink.classList.add('active');
            console.log("Marketplace tab selected.");
        }
    });
}

export const downloadMarketplace = (elementId, dotnetHelper) => {
    let storeId = '103074754';
    let scriptUrl = 'https://app.ecwid.com/script.js?' + storeId + '&data_platform=code&data_date=' + Date.now();

    let scripts = Array.from(document.getElementsByTagName('script'));
    let scriptElement = scripts.find(s => s.src === scriptUrl);

    if (!scriptElement) {
        let script = document.createElement('script');
        script.src = scriptUrl;
        script.charset = 'utf-8';
        script.setAttribute('data-cfasync', 'false');
        let container = document.getElementById(elementId);
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
    let navLinks = document.querySelectorAll('.navbar-nav .nav-link');

    navLinks.forEach(function(navLink) {
        if (navLink.getAttribute('href') === "Marketplace") {
            navLink.classList.remove('active');
        }
    });
}

export const unloadEcwidAccountComponents = (location, dotnetHelper) => {
    console.log(`location changed -> ${location}`);
}