const express = require('express');
const app = express();
const http = require('http');
const server = http.createServer(app);

const logger = require("morgan")
const path = require("path");
const socketHandler = require("./socketHandler.js");

const port = process.env.PORT || 3000;


app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname,  '..',  'frontEnd',  'index.html'));
});
app.use(logger("short"))

server.listen(port, () => {
    console.log('listening on http://localhost:'+port);
});

socketHandler.Start(port)
