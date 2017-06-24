const express = require('express')
const app = express()

app.use(express.static('Client'));
app.listen(80, function () {
  console.log('Static hosting on port 80!')
})

const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 8080 });

console.log("Hello");
wss.on('connection', function connection(ws) {
  ws.on('message', function incoming(message) {
    console.log('received: %s', message);
  });

  console.log("New client");
});
