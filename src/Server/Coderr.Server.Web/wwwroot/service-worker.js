'use strict';

self.addEventListener('install', function (event) {
    self.skipWaiting();
});

self.addEventListener('activate', function (event) {
    event.waitUntil(clients.claim());
});

// Respond to a server push with a user notification
self.addEventListener('push', function (event) {
    if (event.data) {
        const { title, lang = 'en', badge, body, tag, timestamp, requireInteraction, actions, image, data } = event.data.json();

        const promiseChain = self.registration.showNotification(title, {
            lang,
            body,
            data,
            requireInteraction,
            tag: tag || undefined,
            timestamp: timestamp ? Date.parse(timestamp) : undefined,
            actions: actions || undefined,
            image: image || undefined,
            icon: '/favicon-32x32.png',
            badge: badge || undefined
        });

        // Ensure the toast notification is displayed before exiting this function
        event.waitUntil(promiseChain);
    }
});

self.addEventListener('notificationclick', function (event) {
    var notification = event.notification;
    var action = event.action;
    notification.close();

    // When data have the url attached, just visit it.
    var storedUrl = notification.data[action + 'Url'];
    if (storedUrl) {
        clients.openWindow(storedUrl);
        return;
    }

    var url = '/discover/incidents/' + notification.data.applicationId + '/incident/' + notification.data.incidentId;
    if (action === 'AssignToMe') {
        url = '/discover/assign/incident/' + notification.data.incidentId;
    }

    clients.openWindow(url);
    //event.waitUntil(
    //    clients.matchAll({ type: 'window', includeUncontrolled: true })
    //        .then(function (clientList) {
    //            if (clientList.length > 0) {
    //                let client = clientList[0];

    //                for (let i = 0; i < clientList.length; i++) {
    //                    if (clientList[i].focused) {
    //                        client = clientList[i];
    //                    }
    //                }

    //                return client.focus();
    //            }


    //            console.log('clicked', event);
    //            return clients.openWindow('/');
    //        })
    //);
});

self.addEventListener('pushsubscriptionchange', function (event) {
    event.waitUntil(
        Promise.all([
            Promise.resolve(event.oldSubscription ? deleteSubscription(event.oldSubscription) : true),
            Promise.resolve(event.newSubscription ? event.newSubscription : subscribePush(registration))
                .then(function (sub) { return saveSubscription(sub); })
        ])
    );
});
