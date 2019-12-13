'use strict';

self.addEventListener('install', function (event) {
    console.log('installing');
    self.skipWaiting();
});

self.addEventListener('activate', function (event) {
    console.log('active');
    event.waitUntil(clients.claim());
});

// Respond to a server push with a user notification
self.addEventListener('push', function (event) {
    console.log('push', event.data);
    if (event.data) {
        const { title, lang = 'en', body, tag, timestamp, requireInteraction, actions, image } = event.data.json();

        const promiseChain = self.registration.showNotification(title, {
            lang,
            body,
            requireInteraction,
            tag: tag || undefined,
            timestamp: timestamp ? Date.parse(timestamp) : undefined,
            actions: actions || undefined,
            image: image || undefined,
            badge: 'https://coderr.io/images/favicon.png',
            icon: 'https://coderr.io/images/nuget-icon.jpg'
        });

        // Ensure the toast notification is displayed before exiting this function
        event.waitUntil(promiseChain);
    }
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();

    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true })
            .then(function (clientList) {
                if (clientList.length > 0) {
                    let client = clientList[0];

                    for (let i = 0; i < clientList.length; i++) {
                        if (clientList[i].focused) {
                            client = clientList[i];
                        }
                    }

                    return client.focus();
                }

                return clients.openWindow('/');
            })
    );
});

self.addEventListener('pushsubscriptionchange', function (event) {
    console.log('pushsubscriptionchange');
    event.waitUntil(
        Promise.all([
            Promise.resolve(event.oldSubscription ? deleteSubscription(event.oldSubscription) : true),
            Promise.resolve(event.newSubscription ? event.newSubscription : subscribePush(registration))
                .then(function (sub) { return saveSubscription(sub); })
        ])
    );
});
