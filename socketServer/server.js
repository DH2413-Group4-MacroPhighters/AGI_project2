const express = require('express');
const app = new express;
const http = require('http');
const server = http.createServer(app);

const logger = require("morgan")
const path = require("path");
const socketHandler = require("./socketHandler.js");

app.engine('html', require('ejs').renderFile);
app.set('view engine', 'html');

var CONFIG = require('../config.json');

const port = process.env.PORT || parseInt(CONFIG.PortServer);
const localIP = CONFIG.LocalIP;
const host = CONFIG.HostIP;
const portWs = CONFIG.PortWs;

app.get('/', (req, res) => {
    res.render(path.join(__dirname,  '..',  'frontEnd',  'index.html'), {host:host, port:portWs});
});

app.use(logger("short"))

server.listen(port, () => {
    console.log('listening on http://' + localIP + ':' + port);
});

socketHandler.Start(port)
