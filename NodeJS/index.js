const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 80 });

console.log("Hello");
wss.on('connection', function connection(ws) {
  ws.on('message', function incoming(message) {
    console.log('received: %s', message);
  });

  ws.send('something');
});