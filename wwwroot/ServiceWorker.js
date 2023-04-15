self.addEventListener('fetch', function (event) { });
self.addEventListener('push', function (e) {
    var body;
    var title;
    var refurl;

    if (e.data) {
        body = e.data.json().message;
        title = e.data.json().title;
        refurl = e.data.json().refurl;
    } else {
        title = "Kisa-Kuikka ilmoitus";
        body = "Virhe ladatessa ilmoitusta";
    }

    var options = {
        body: body,
        
        vibrate: [100, 50, 100],
        data: {
            dateOfArrival: Date.now(),
            refurl: refurl
        }
    };
    e.waitUntil(
        self.registration.showNotification(title, options)
    );
});
self.addEventListener('notificationclick', function (e) {
    var notification = e.notification;

    clients.openWindow(notification.data.refurl);
    notification.close();
});
