window.getWindowDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

async function ClearCache() {

    const cacheKeys = await caches.keys();
    //console.log(cacheKeys.toString());
    var offline_cache = cacheKeys.filter(key => key.startsWith('offline-cache-'));
    //var offline_cache = cacheKeys.filter(key => key.includes('resources'));

    var key_arr = offline_cache.toString().split(',');
    for (var i = 0; i < key_arr.length; i++) {
        console.log(key_arr[i].toString());
        caches.delete(key_arr[i].toString());
    }
    //console.log(offline_cache.toString());
    //const del_key = offline_cache.toString();
    //if (offline_cache.length > 0) {
    //    console.log('cache exist');
    //}
    //else {
    //    console.log('no cache exist');
    //}
    //caches.has(del_key).then(function (hasCache) {
    //    if (!hasCache) {
    //        console.log('no cache exist');
    //    } else {
    //        console.log('cache exist');
    //        //caches.delete(del_key);
    //        //console.log('delete cache success');
    //    }
    //}).catch(function () {
    //    // Handle exception here.
    //    console.log('call ClearCache error');
    //});
}