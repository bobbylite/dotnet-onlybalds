export const onLoad = () => {
    $('.navbar-collapse').collapse('hide');
    // Get all nav links
    var navLinks = document.querySelectorAll('.navbar-nav .nav-link');

    // Loop through each nav link
    navLinks.forEach(function(navLink) {
        // Check if the href attribute of the nav link is "Marketplace"
        if (navLink.getAttribute('href') === "Marketplace") {
            // Add a class to indicate selection (assuming you have CSS for this)
            navLink.classList.add('active');
            
            // If your tab is linked to some content and you want to trigger its selection,
            // you might need to handle it accordingly (e.g., show/hide content)
            // For now, let's just log that the tab is selected
            console.log("Marketplace tab selected!");
        }
    });
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
    // Get all nav links
    var navLinks = document.querySelectorAll('.navbar-nav .nav-link');

    // Loop through each nav link
    navLinks.forEach(function(navLink) {
        // Check if the href attribute of the nav link is "Marketplace"
        if (navLink.getAttribute('href') === "Marketplace") {
            // Remove the class that indicates selection
            navLink.classList.remove('active');
        }
    });
}