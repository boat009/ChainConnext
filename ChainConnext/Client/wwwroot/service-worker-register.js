const serviceWorkerFileName = 'service-worker.js';
const swInstalledEvent = 'installed';
const staticCachePrefix = 'blazor-cache-v';
const updateAlertMessage = 'Update available. Reload the page when convenient.';
const blazorAssembly = 'ChainConnext.Client';
const blazorInstallMethod = 'PWAInstallable';

window.updateAvailable = new Promise(function (resolve, reject) {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register(serviceWorkerFileName)
            .then(function (registration) {
                console.log('Registration successful, scope is:', registration.scope);
                registration.onupdatefound = () => {
                    const installingWorker = registration.installing;
                    installingWorker.onstatechange = () => {
                        switch (installingWorker.state) {
                            case swInstalledEvent:
                                if (navigator.serviceWorker.controller) {
                                    resolve(true);
                                } else {
                                    resolve(false);
                                }
                                break;
                            default:
                        }
                    };
                };
            })
            .catch(error =>
                console.log('Service worker registration failed, error:', error));
    }
});

window['updateAvailable']
    .then(isAvailable => {
        if (isAvailable) {
            alert(updateAlertMessage);
            const cacheKeys = await caches.keys();
            var offline_cache = cacheKeys.filter(key => key.startsWith('offline-cache-'));
            var key_arr = offline_cache.toString().split(',');
            for (var i = 0; i < key_arr.length; i++) {
                console.log(key_arr[i].toString());
                caches.delete(key_arr[i].toString());
            }
            //console.log(updateAlertMessage);
        }
    });

function showAddToHomeScreen() {
    DotNet.invokeMethodAsync(blazorAssembly, blazorInstallMethod)
        .then(function () { }, function (er) { setTimeout(showAddToHomeScreen, 1000); });
}

window.BlazorPWA = {
    installPWA: function () {
        if (window.PWADeferredPrompt) {
            window.PWADeferredPrompt.prompt();
            window.PWADeferredPrompt.userChoice
                .then(function (choiceResult) {
                    window.PWADeferredPrompt = null;
                });
        }
    }
};

window.addEventListener('beforeinstallprompt', function (e) {
    // Prevent Chrome 67 and earlier from automatically showing the prompt
    e.preventDefault();
    // Stash the event so it can be triggered later.
    window.PWADeferredPrompt = e;

    showAddToHomeScreen();
});
