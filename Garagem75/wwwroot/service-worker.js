const CACHE_NAME = "garagem75-cache-v1";

const urlsToCache = [
    "/",
    "/css/site.css",
    "/manifest.json"
];

self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                return cache.addAll(urlsToCache);
            })
    );
});

self.addEventListener('fetch', event => {
    // 1. IGNORAR TUDO QUE NÃO SEJA GET (POST do Edit, Delete, etc)
    if (event.request.method !== 'GET') {
        return;
    }

    // 2. IGNORAR NAVEGAÇÃO DO MVC E CHAMADAS DE API
    const url = event.request.url;
    if (url.includes('/OrdemServicos') ||
        url.includes('/Clientes') ||
        url.includes('/Veiculos') ||
        url.includes('/api/')) {
        return;
    }

    // 3. COMPORTAMENTO PADRÃO (Apenas para arquivos estáticos do Blazor)
    event.respondWith(
        caches.match(event.request).then(response => {
            return response || fetch(event.request);
        })
    );
});
self.addEventListener("activate", event => {
    event.waitUntil(self.clients.claim());
});