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

app.get('/map.png', (req, res) => {
    res.sendFile(path.join(__dirname,  '..',  'frontEnd',  'map.png'));
});

app.get('/tree.png', (req, res) => {
    res.sendFile(path.join(__dirname,  '..',  'frontEnd',  'tree.png'));
});

app.use(logger("short"))

server.listen(port, () => {
    console.log('listening on http://' + localIP + ':' + port);
    console.log('Go to http://' + host + ':' + port);
});

socketHandler.Start(port)
