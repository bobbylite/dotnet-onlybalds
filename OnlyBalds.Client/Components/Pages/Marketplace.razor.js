/**
 * Handles the onLoad event.
 */
export const onLoad = () => {
    $('.navbar-collapse').collapse('hide');
}

/**
/**
 * Initializes the Ecwid marketplace widget.
 * @param {String} elementId - The ID of the element to host the marketplace widget.
 */
export const initializeMarketplace = (elementId) => {
    $(document).ready(async function() {
        const currentUrl = window.location.href;
        if (!currentUrl.includes("#ecwid")) {
            // Force a full page reload with the "#ecwid" fragment
            window.location.href = currentUrl + "#ecwid";
            window.location.reload();
            return;
        }

        const storeId = '103074754';
        const scriptUrl = `https://app.ecwid.com/script.js?${storeId}&data_platform=code&data_date=${Date.now()}`;
        let container = document.getElementById(elementId);

        // Clear the container content to ensure re-initialization on navigation
        if (container) {
            container.innerHTML = '';  // Clears previous widget content
            // Remove the data-v-app attribute if it exists
            if (container.hasAttribute('data-v-app')) {
                container.removeAttribute('data-v-app');
            }
        } else {
            console.error(`Container with ID "${elementId}" not found.`);
            return;
        }

        // Function to initialize the marketplace widget after script loads
        const loadWidget = () => {
            xProductBrowser(
                "categoriesPerRow=3",
                "views=grid(20,3) list(60) table(60)",
                "categoryView=grid",
                "searchView=list",
                `id=${elementId}`
            );
            console.log("Marketplace initialized.");
        };

        // Load the Ecwid script dynamically if it’s not already loaded
        const loadScript = () => {
            return new Promise((resolve, reject) => {
                const scriptElement = document.createElement('script');
                scriptElement.src = scriptUrl;
                scriptElement.charset = 'utf-8';
                scriptElement.setAttribute('data-cfasync', 'false');

                scriptElement.onload = resolve;
                scriptElement.onerror = reject;
                document.head.appendChild(scriptElement);
            });
        };

        const existingScript = document.querySelector(`script[src^="https://app.ecwid.com/script.js?${storeId}"]`);

        try {
            if (existingScript) {
                loadWidget();
                return;
            }
            await loadScript();
            loadWidget();
        } catch (error) {
            console.error("Failed to load the Ecwid script:", error);
        }
    });
};

/**
 * Unloads the Ecwid account components.
 */
export const unloadEcwidAccountComponents = () => {
    console.log(`location changed -> ${location}`);
}

/**
 * Determines if the browser is a mobile browser.
 * @returns True if the browser is a mobile browser, false otherwise.
 */
export const isMobileBrowser = () => {
    return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
}

// Suppress ResizeObserver errors globally
const resizeObserverErrorHandler = (error) => {
    if (error.message && error.message.includes('ResizeObserver loop')) {
        return;
    }
};

window.addEventListener('error', (event) => {
    if (event.message && event.message.includes('ResizeObserver loop')) {
        event.stopImmediatePropagation();
    }
});
window.addEventListener('error', resizeObserverErrorHandler);
window.addEventListener('unhandledrejection', resizeObserverErrorHandler);