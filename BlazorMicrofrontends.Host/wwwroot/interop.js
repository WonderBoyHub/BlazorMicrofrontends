// BlazorMicrofrontends.Host JavaScript interop functions

/**
 * Loads a JavaScript script dynamically
 * @param {string} url - URL of the script to load
 * @param {boolean} async - Whether to load the script asynchronously
 * @param {string} id - ID to assign to the script element
 * @returns {Promise} A promise that resolves when the script is loaded
 */
export function loadScript(url, async = true, id = null) {
    return new Promise((resolve, reject) => {
        // Check if script is already loaded
        const existingScript = document.querySelector(`script[src="${url}"]`);
        if (existingScript) {
            resolve();
            return;
        }

        const script = document.createElement('script');
        script.src = url;
        script.async = async;
        
        if (id) {
            script.id = id;
        }
        
        script.onload = () => resolve();
        script.onerror = (error) => reject(new Error(`Failed to load script: ${url}`));
        
        document.head.appendChild(script);
    });
}

/**
 * Loads a CSS stylesheet dynamically
 * @param {string} url - URL of the stylesheet to load
 * @param {string} id - ID to assign to the link element
 * @returns {Promise} A promise that resolves when the stylesheet is loaded
 */
export function loadCss(url, id = null) {
    return new Promise((resolve, reject) => {
        // Check if stylesheet is already loaded
        const existingLink = document.querySelector(`link[href="${url}"]`);
        if (existingLink) {
            resolve();
            return;
        }

        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = url;
        
        if (id) {
            link.id = id;
        }
        
        link.onload = () => resolve();
        link.onerror = () => reject(new Error(`Failed to load stylesheet: ${url}`));
        
        document.head.appendChild(link);
    });
}

/**
 * Creates a global error handler for microfrontends
 * @param {function} errorCallback - Callback to invoke with error information
 * @returns {void}
 */
export function setupErrorHandler(errorCallback) {
    window.addEventListener('error', (event) => {
        // Only handle errors from microfrontends
        if (event.filename && event.filename.includes('microfrontend')) {
            errorCallback({
                message: event.message,
                source: event.filename,
                lineNumber: event.lineno,
                columnNumber: event.colno
            });
            
            // Prevent default browser error handling
            event.preventDefault();
        }
    });
    
    window.addEventListener('unhandledrejection', (event) => {
        // Handle promise rejections
        errorCallback({
            message: event.reason?.message || 'Unhandled promise rejection',
            source: 'Promise',
            detail: event.reason
        });
        
        // Prevent default browser error handling
        event.preventDefault();
    });
} 