// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
//const CACHE_VERSION = 1.0;
//const cacheNamePrefix = 'offline-cache-';
//const cacheName = `${cacheNamePrefix}${CACHE_VERSION}`;

self.addEventListener('fetch', () => { });
