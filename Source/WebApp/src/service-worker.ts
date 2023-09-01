/// <reference lib="webworker" />
/* eslint-disable no-restricted-globals */

// This service worker can be customized!
// See https://developers.google.com/web/tools/workbox/modules
// for the list of available Workbox modules, or add any other
// code you'd like.
// You can also remove this file if you'd prefer not to use a
// service worker, and the Workbox build step will be skipped.

import { clientsClaim } from 'workbox-core';
import { ExpirationPlugin } from 'workbox-expiration';
import { precacheAndRoute, createHandlerBoundToURL } from 'workbox-precaching';
import { registerRoute } from 'workbox-routing';
import { NetworkFirst, NetworkOnly } from 'workbox-strategies';
import { BackgroundSyncPlugin } from 'workbox-background-sync';

declare const self: ServiceWorkerGlobalScope;

clientsClaim();

// Precache all of the assets generated by your build process.
// Their URLs are injected into the manifest variable below.
// This variable must be present somewhere in your service worker file,
// even if you decide not to use precaching. See https://cra.link/PWA
precacheAndRoute(self.__WB_MANIFEST);

// Set up App Shell-style routing, so that all navigation requests
// are fulfilled with your index.html shell. Learn more at
// https://developers.google.com/web/fundamentals/architecture/app-shell
const fileExtensionRegexp = new RegExp('/[^/?]+\\.[^/]+$');
registerRoute(
  // Return false to exempt requests from being fulfilled by index.html.
  ({ request, url }: { request: Request; url: URL }) => {
    // If this isn't a navigation, skip.
    if (request.mode !== 'navigate') {
      return false;
    }

    // If this is a URL that starts with /_, skip.
    if (url.pathname.startsWith('/_')) {
      return false;
    }

    // If this looks like a URL for a resource, because it contains
    // a file extension, skip.
    if (url.pathname.match(fileExtensionRegexp)) {
      return false;
    }

    // Return true to signal that we want to use the handler.
    return true;
  },
  createHandlerBoundToURL(process.env.PUBLIC_URL + '/index.html')
);

// An example runtime caching route for requests that aren't handled by the
// precache, in this case same-origin .png requests like those from in public/
registerRoute(
  // Add in any other file extensions or routing criteria as needed.
  ({ url }) => url.origin === self.location.origin && (url.pathname.endsWith('.png') || url.pathname.endsWith('.ico')),
  // Customize this strategy as needed, e.g., by changing to CacheFirst.
  new NetworkFirst({
    cacheName: 'images',
    plugins: [
      // Ensure that once this runtime cache reaches a maximum size the
      // least-recently used images are removed.
      new ExpirationPlugin({ maxEntries: 50 }),
    ],
  })
);

// This allows the web app to trigger skipWaiting via
// registration.waiting.postMessage({type: 'SKIP_WAITING'})
self.addEventListener('message', (event) => {
  if (event.data && event.data.type === 'SKIP_WAITING') {
    self.skipWaiting();
  }
});

const guid = '[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?';
const cachedRoutes = [
  {
    route: "team",
    cacheName: "teamList"
  },
  {
    route: "team/members",
    cacheName: "memberList"
  },
  {
    route: "team/:id",
    cacheName: "teamData"
  },
  {
    route: "pole",
    cacheName: "poleList"
  },
  {
    route: "pole/:id",
    cacheName: "poleData"
  },
  {
    route: "repair/history/pole/:id",
    cacheName: "poleHistory"
  },
  {
    route: "repair/history/team/:id",
    cacheName: "teamHistory"
  },
  {
    route: "user",
    cacheName: "userList"
  },
  {
    route: "user/:id",
    cacheName: "userData"
  },
  {
    route: "notification/message",
    cacheName: "notificationList"
  }
]
for (let r of cachedRoutes) {
  const route = `.*/api/${r.route.replace(':id', guid)}`
  registerRoute(
    ({ url }) => url.href.match(new RegExp(route)),
    new NetworkFirst({
      cacheName: r.cacheName,
      plugins: [
        new ExpirationPlugin({ maxEntries: 50 })
      ]
    }),
    'GET'
  )
}

const bgSyncPlugin = new BackgroundSyncPlugin('requestQueue', {
  maxRetentionTime: 24 * 60 // Retry for max of 24 Hours (specified in minutes)
});

const repairStartRoute = new RegExp(".*/api/repair/start");
registerRoute(
  ({ url }) => url.href.match(repairStartRoute),
  new NetworkOnly({
    plugins: [bgSyncPlugin]
  }),
  'POST'
);

const repairEndRoute = new RegExp(".*/api/repair/end");
registerRoute(
  ({ url }) => url.href.match(repairEndRoute),
  new NetworkOnly({
    plugins: [bgSyncPlugin]
  }),
  'PUT'
);